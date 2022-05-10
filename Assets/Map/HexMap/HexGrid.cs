using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Assets.Scripts.Path;
using Assets.Scripts.Logger;


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

		void Awake()
		{
			if(GlobalVariables.generationSettings.terrainChunkCountX == 0 || GlobalVariables.generationSettings.terrainChunkCountY == 0)
            {
				chunkCountX = 35;
				chunkCountZ = 21;
            }
			else
            {
				chunkCountX = GlobalVariables.generationSettings.terrainChunkCountX;//35
				chunkCountZ = GlobalVariables.generationSettings.terrainChunkCountY;//35
            }


			cellCountX = chunkCountX * HexMetrics.chunkSizeX;
			cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

			CreateChunks();
			CreateCells();
			for (int i = 0; i < cells.Length; i++)
				cells[i].neighbours = cells.GetNeighbours(i);

			//HexFieldGenerator.GenerateHexMap(cells);
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
	}
}
