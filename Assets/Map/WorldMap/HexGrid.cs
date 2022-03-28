using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Map.WorldMap
{
    public class HexGrid : MonoBehaviour
    {
		public int chunkCountX = 4, chunkCountZ = 3;
		int cellCountX, cellCountZ;
		//public int width;
        //public int height;
	
		public int generationSeed; //Семя для генерации карты. Перекинем потом в хоста
		
		public Color defaultColor = Color.white;
		public Color chosenColor = Color.cyan;

		public HexGridChunk chunkPrefab;
		public HexCell cell_prefab;

		HexGridChunk[] chunks;
		HexCell[] cells;

		[ContextMenu("Generate game field")]
		void Awake()
		{
			System.Random rnd = new System.Random();
			generationSeed = rnd.Next(1, 30000000); //Семя сегенерилось

			cellCountX = chunkCountX * HexMetrics.chunkSizeX;
			cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

			CreateChunks();
			CreateCells();
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
				}
			}
		}
		void CreateCells()
		{
			cells = new HexCell[cellCountZ * cellCountX];
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
			//cell.transform.SetParent(transform, false);
			cell.transform.localPosition = position;
			cell.coords = HexCoords.FromOffset(x, z);
			cell.CellColor = cell.terrainColor;
			cell.CellColor = cell.terrainColor;
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
			/*print("touched at " + position);
			HexCoords coords = HexCoords.FromPosition(position);
			print("touched cell " + coords);
			int index = coords.x + coords.z * width + coords.z / 2;
			HexCell tmp = cells[index];
			tmp.color = chosenColor;
			hexMesh.Triangulate(cells);*/
			foreach (var cell in cells)
            {
				if(cell.coords.EqualsTo(HexCoords.FromPosition(position)))
                {
					print("touched at " + position);
					print("touched cell " + cell.coords);
					//cell.Choose();
					cell.MouseLeftClick.Invoke(cell, new HexCellEventArgs(position));
					//hexMesh.Triangulate(cells);
					break;
                }
			}
		}
	}
}
