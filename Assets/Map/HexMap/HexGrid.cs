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
			chunkCountX = 35;//35
			chunkCountZ = 21;//35

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
			cell.Type = CellType.water;

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
			for (int i = 0; i < cellCountX * cellCountZ; i++)
				for (int j = 0; j < cellCountX * cellCountZ; j++)
					weightMatrix[i, j] = 10000; //Сделал все элементы матрицы "бесконечностью"

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
			return weightMatrix;
        }

		private int[,] CreateWaterMatrix(int[,] weightMatrix)
        {
			for (int i = 0; i < cellCountX; i++)
			{
				for (int j = 0; j < cellCountZ; j++)
				{
					if (i != j)
					{
						foreach (var cell in cells[j * cellCountZ + i].neighbours)
						{
							if(cell.Type != CellType.water)
                            {
								weightMatrix[j * cellCountZ + i, cell.CellIndex] = 10000;
								weightMatrix[cell.CellIndex, j * cellCountZ + i] = 10000;
                            }
						}
					}
				}
			}
			return weightMatrix;
        }

		private int[,] CreateTerrainMatrix(int[,] weightMatrix)
		{
			for (int i = 0; i < cellCountX; i++)
			{
				for (int j = 0; j < cellCountZ; j++)
				{
					if (i != j)
					{
						foreach (var cell in cells[j * cellCountZ + i].neighbours)
						{
							if (cell.Type == CellType.water)
							{
								weightMatrix[j * cellCountZ + i, cell.CellIndex] = 10000;
								weightMatrix[cell.CellIndex, j * cellCountZ + i] = 10000;
							}
						}
					}
				}
			}
			return weightMatrix;
		}

		const int INF = 100000;
		
		private void InitSupportArrays(int[] D, int[] V, int[] T, int n) 
		{ 
			 for (int i = 0; i<n; i++)
			{
				D[i] = INF;
				V[i] = 0;
				T[i] = -1;
			}
		}

		static int[] DikstraAlg(int[,] G, int n, int[] D, int[] V, int[] T, int S, int F)
		{
			V[S] = 1; //1. Начальная
			int min, i_min;
			D[S] = 0; //установка
			T[S] = 0; //...
			int y = S; //...
			do {
				for (int x = 0; x < n; x++)
				{
					if (G[y, x] != 0 && V[x] == 0)
					{
						if (D[y] + G[y, x] < D[x])
                        {
							D[x] = D[y] + G[y, x];
							T[x] = y;
                        }
					}
				}
				min = INF; //3. Поиск минимального d(xi) и...
				i_min = INF;
				for (int x = 0; x < n; x++)
					if (V[x] == 0)
						if (D[x] < min)
						{
							min = D[x];
							i_min = x;
						}
				if (min != INF)
				{
					//T[i_min] = y;
					y = i_min;
					V[i_min] = 1;
				}
				if (y == F) //4. Проверка на окончание алгоритма
                    break;
            } while (min != INF) ;
			int vertex = F - 1;
			Stack<int> stack = new Stack<int>();
			while(vertex != 0)
            {
				stack.Push(vertex);
				vertex = T[vertex];
            }
			int pathSize = stack.Count - 1;
			int[] path = new int[pathSize];
			for (int i = pathSize, j = 0; i != 0; i--, j++)
			{
				path[j] = stack.Pop();
			}
			return T;
		}
        
		private List<HexCell> AddPathOnTravelList(int[] T,int t, int v, int type)
        {
			List<HexCell> path = new List<HexCell>();
			path.Add(cells[t]);
			int tmp = v;
			while (T[v] != 0)
            {
				path.Add(cells[v]);
				v = T[v]; 

            }
			List<HexCell> truePath = new List<HexCell>();
			for(int i = path.Count - 1; i > 0; i--)
            {
				switch(type)
                {
					case 0:
						if (path[i].Type != CellType.water)
							return null;
						break;
					case 1:
						if (path[i].Type == CellType.water)
							return null;
						break;

				}
				truePath.Add(path[i]);
			}
			if(path.Count == 2)
            {
				foreach (var cell in cells[t].neighbours)
					if (cell.CellIndex == tmp)
						return truePath;
				return null;
            }

			return truePath;
        }

		public List<HexCell> GetWay(int type, HexCell startCell, HexCell endCell)
		{
			int[,] weightMatrix = new int[cellCountX * cellCountZ, cellCountX * cellCountZ];
			weightMatrix = CreateWeghtMatrix(weightMatrix);
			switch(type)
            {
				case 0:
                    weightMatrix = CreateWaterMatrix(weightMatrix);
					break;
				case 1:
					weightMatrix = CreateTerrainMatrix(weightMatrix);
					break;
				case 2:
					break;
            }
			int[] D = new int[cellCountX * cellCountZ];
			int[] V = new int[cellCountX * cellCountZ];
			int[] T = new int[cellCountX * cellCountZ];
			InitSupportArrays(D, V, T, cellCountX * cellCountZ);

			T = DikstraAlg(weightMatrix, cellCountX * cellCountZ, D, V, T, startCell.CellIndex, endCell.CellIndex);
			
			return AddPathOnTravelList(T,startCell.CellIndex, endCell.CellIndex, type);
		}
	}
	
}
