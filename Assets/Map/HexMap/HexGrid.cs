using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Map.WorldMap
{
    public class HexGrid : MonoBehaviour
    {
		public int chunkCountX = 100, chunkCountZ = 100;
		public int cellCountX, cellCountZ;
		
	
		public int generationSeed; //Семя для генерации карты. Перекинем потом в хоста
		
		public Color defaultColor = Color.white;
		public Color chosenColor = Color.cyan;

		public HexGridChunk chunkPrefab;
		public HexCell cell_prefab;

		public HexGridChunk[] chunks;
		CellList cells;

		public CellList cellList
        {
			get { return cells; }
			set { cells = value; }
        }

		[ContextMenu("Generate game field")]
		void Awake()
		{
			chunkCountX = 10;//35
			chunkCountZ = 10;//35

			cellCountX = chunkCountX * HexMetrics.chunkSizeX;
			cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

			CreateChunks();
			CreateCells();
			for (int i = 0; i < cells.Length; i++)
				cells[i].neighbours = cells.GetNeighbours(i);

			HexFieldGenerator.GenerateHexMap(cells);
		}
		void CreateChunks()
		{
			chunks = new HexGridChunk[chunkCountX * chunkCountZ];

			for (int z = 0, i = 0; z < chunkCountZ; z++)
			{
				for (int x = 0; x < chunkCountX; x++)
				{
					HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
					chunk.transform.SetParent(transform);
					chunk.ChunkIndex = i - 1;
					chunk.name = $"Index = {chunk.ChunkIndex}";
				}
			}

			Console.WriteLine();
		}
		void CreateCells()
		{
			cells = new CellList(new HexCell[cellCountZ * cellCountX], cellCountX, cellCountZ);
			for (int z = 0, i = 0; z < cellCountZ; z++)
			{
				for (int x = 0; x < cellCountX; x++)
				{
					CreateCell(x, z, i++);
				}
			}
		}

		void CreateCell(int x, int z, int i)
		{
			Vector3 position = new Vector3(
				(x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f),
				0f,
				z * (HexMetrics.outerRadius * 1.5f));
			HexCell cell = cells[i] = Instantiate<HexCell>(cell_prefab);
			//cell.transform.localPosition = position;
			cell.transform.position = position;
			cell.coords = HexCoords.FromOffset(x, z);
			cell.CellType = HexCell.CellTypes.water;

			cell.CellIndex = i;
			cell.name = $"Index = {cell.CellIndex}";
			cell.Elevation = 0;

			cells[i] = cell;


			AddCellToChunk(x, z, cell);
		}
		void AddCellToChunk(int x, int z, HexCell cell)
		{
			int chunkX = x / HexMetrics.chunkSizeX;
			int chunkZ = z / HexMetrics.chunkSizeZ;
			HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

			int localX = x - chunkX * HexMetrics.chunkSizeX;
			int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
			chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
		}
		void Update()
		{
			if (Input.GetMouseButton(0))
			{
				HandleInput();
			}
		}

		void HandleInput()
		{
			Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(inputRay, out hit))
			{
				TouchCell(hit.point);
			}
		}

		void TouchCell(Vector3 position)
		{
			position = transform.InverseTransformPoint(position);
			foreach (var cell in cells)
            {
				if(cell.coords.EqualsTo(HexCoords.FromPosition(position)))
                {
					//cell.Choose();
					cell.MouseLeftClick.Invoke(cell, new HexCellEventArgs(position));
					//hexMesh.Triangulate(cells);
					break;
                }
			}
		}

		public HexCell GetByPosition(Vector3 pos)
        {
			int ind = HexCoords.FromPosition(pos).MakeIndex(cellCountX);
			return cells[ind];
        }
		
		private int[,] CreateWeghtMatrix(int[,] weightMatrix)
        {
			for(int i = 0; i < cellCountX; i++)
            {
				for(int j = 0; j < cellCountZ; j++)
                {
					if(i != j)
                    {
						foreach(var cell in cells[j * cellCountZ + i].neighbours)
                        {
							weightMatrix[j * cellCountZ + i, cell.CellIndex] = 1;
							weightMatrix[cell.CellIndex, j * cellCountZ + i] = 1;
						}
                    }
                }
            }
			for(int i = 5; i >= 0; i--)
					Debug.Log($"{weightMatrix[i, 0]} {weightMatrix[i, 1]} {weightMatrix[i, 2]} {weightMatrix[i, 3]} {weightMatrix[i, 4]}");
			return weightMatrix;
        }

		public List<HexCell> GetWay(Movable c, HexCell startCell, HexCell endCell)
		{
			int[,] weightMatrix = new int[cellCountX * cellCountZ, cellCountX * cellCountZ];
			weightMatrix = CreateWeghtMatrix(weightMatrix);
			List<HexCell> res = new List<HexCell>();
			if (c.IsSwimAndMove)
			{
				res.Add(startCell);
				while (startCell != endCell)
				{
					var neighbours = startCell.neighbours;
					startCell = neighbours[0];
					double min = Mathf.Sqrt(Mathf.Pow(endCell.transform.position.x - startCell.transform.position.x, 2) + Mathf.Pow(endCell.transform.position.z - startCell.transform.position.z, 2));
					foreach (var cell in neighbours)
					{
						double local_min = Mathf.Sqrt(Mathf.Pow(endCell.transform.position.x - cell.transform.position.x, 2) + Mathf.Pow(endCell.transform.position.z - cell.transform.position.z, 2));
						if (local_min < min)
						{
							min = local_min;
							startCell = cell;
						}
					}
					res.Add(startCell);
				}
				return res;
			}
			if (c.IsSwimming)
			{
				return res;
			}
			if (!c.IsSwimming)
			{
				return res;
			}
			return res;
		}
	}
	
}
