using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Map.WorldMap
{
    public class GenerationHexField : MonoBehaviour
    {
        public int terrainCells;
        public List<HexCell> neighbourCells;
        int IndexFromHexCoords(int x, int z, int mapWidth)
        {
            return x + z * mapWidth + z / 2;
        }

        List<HexCell> GetNeighboursCell(HexCell[] cells, int index, int width) //От параметра width надо будет избавиться
        {
            List<HexCell> neighbours = new List<HexCell>();
            HexCell cur_cell = cells[index];
            HexCoords cur_coords = cur_cell.coords;
            for (int i = 0; i < 6; i++)
            {
                HexCoords nei_coords = cur_coords;
                switch (i)
                {
                    case 0:
                        nei_coords.x += 1;
                        break;
                    case 1:
                        nei_coords.x -= 1;
                        break;
                    case 2:
                        nei_coords.z += 1;
                        break;
                    case 3:
                        nei_coords.z -= 1;
                        break;
                    case 4:
                        nei_coords.x += 1;
                        nei_coords.z -= 1;
                        break;
                    case 5:
                        nei_coords.x -= 1;
                        nei_coords.z += 1;
                        break;

                }
                int nei_index = IndexFromHexCoords(nei_coords.x, nei_coords.z, width);
                if (nei_index >= 0 && nei_index < cells.Length)
                    neighbours.Add(cells[nei_index]);
                neighbours.Add(cells[index]);
            }
            return neighbours;
        }

        public static HexCell[] GenerateHexMap(HexCell[] cells, System.Random rndSeed, int width, int height)
        {
            //cells = GenerateRock(cells, rndSeed);

            GenerationHexField aboba = new GenerationHexField();
            //Генерируем сначала водный рельеф
            cells = aboba.GenerateStartTerrain(cells, rndSeed, width);
            
            //Генерируем остальное
            //Сначала материковая часть
            cells = aboba.GenerateMainlands(cells, rndSeed, width, height);
            //Острова
            cells = aboba.GenerateIslands(cells, rndSeed, width);
            //cells = aboba.GenerateRock(cells, rndSeed, width);
            //cells = aboba.SwitchBorderColors(cells, width);

            //Тут будет всякая хрень для украшательств
            for(int i = 0; i < cells.Length; i++)
            {
                //aboba.AddFeature(cells[i].transform.localPosition);
            }

            return cells;
        }
        private HexCell[] GenerateStartTerrain(HexCell[] cells, System.Random rndSeed, int width)
        {
            for(int i = 0; i < cells.Length; i++)
            {
                int rndElevation = rndSeed.Next(-2, 0);
                cells[i].CellColor = cells[i].waterColor;
                cells[i].Elevation = rndElevation;
            }
            return cells;
        }
        private HexCell[] GenerateIslands(HexCell[] cells, System.Random rndSeed, int width)
        {
            int startCell = rndSeed.Next(cells.Length);
            while(cells[startCell].CellColor == cells[startCell].terrainColor)
                startCell = rndSeed.Next(cells.Length);
            int islandsCount = rndSeed.Next(5, 10);
            
            while(islandsCount != 0)
            {
                neighbourCells = GetNeighboursCell(cells, startCell, width);
                neighbourCells.Add(cells[startCell]);
                int islandsCellsCount = rndSeed.Next(2, neighbourCells.Count);
                for(int i = 0; i < islandsCellsCount; i++)
                {
                    int index = IndexFromHexCoords(neighbourCells[i].coords.x, neighbourCells[i].coords.z, width);
                    int rndEvaluate = rndSeed.Next(0, 1);
                    cells[index].CellColor = cells[index].terrainColor;
                    cells[index].Elevation = rndEvaluate;
                }
                startCell = rndSeed.Next(cells.Length);
                while (cells[startCell].CellColor == cells[startCell].terrainColor)
                    startCell = rndSeed.Next(cells.Length);
                islandsCount--;
            }
            return cells;
        }
            private HexCell[] GenerateMainlands(HexCell[] cells, System.Random rndSeed, int width, int height)
        {
            int mainlandCount = rndSeed.Next(2, 4);
            int startCell = rndSeed.Next(cells.Length);
            int tryCount = 0;
            for (int i = 0; i < mainlandCount; i++)
            {
                int maxCount = rndSeed.Next(width * height - 100, width * height);
                terrainCells = maxCount;
                while (maxCount != 0)
                {
                    int nextCell = startCell;
                    neighbourCells = GetNeighboursCell(cells, nextCell, width);
                    nextCell = rndSeed.Next(neighbourCells.Count());
                    if (neighbourCells[nextCell].CellColor == neighbourCells[nextCell].terrainColor)
                        nextCell = rndSeed.Next(neighbourCells.Count());
                    else
                        tryCount++;
                    if(tryCount > neighbourCells.Count())
                        nextCell = rndSeed.Next(neighbourCells.Count());
                    int index = IndexFromHexCoords(neighbourCells[nextCell].coords.x, neighbourCells[nextCell].coords.z, width);
                    cells[index].CellColor = cells[0].terrainColor;
                    int rndEvaluate = rndSeed.Next(0, 1);
                    cells[index].Elevation = rndEvaluate;
                    startCell = index;
                    maxCount--;
                    if (tryCount == 3)
                        startCell = rndSeed.Next(cells.Length);
                }
                while(cells[startCell].CellColor == cells[startCell].terrainColor)
                    startCell = rndSeed.Next(cells.Length);

            }
            return cells;
        }

        private HexCell[] GenerateTransition(HexCell[] cells,int startCell, int width)
        {
            int tryCount = 0;
            int count = 0;
            while(count != 6)
            {
                neighbourCells = GetNeighboursCell(cells, startCell, width);
                int minRockEleation = cells[startCell].Elevation;
                foreach (var cell in neighbourCells)
                {
                    if (cell.CellColor == cell.rockColor && minRockEleation > cell.Elevation)
                        minRockEleation = cell.Elevation;
                }

                foreach (var cell in neighbourCells)
                    if (cell.CellColor == cell.terrainColor && cell.Elevation <= cells[startCell].Elevation)
                    {
                        tryCount = 0;
                        int index = IndexFromHexCoords(cell.coords.x, cell.coords.z, width);
                        cells[index].Elevation = minRockEleation - 1;
                    }
                foreach (var cell in neighbourCells)
                    if (cell.CellColor == cell.terrainColor && cell.Elevation == cells[startCell].Elevation - 1 && cell.Elevation > 0)
                    {
                        tryCount = 0;
                        startCell = IndexFromHexCoords(cell.coords.x, cell.coords.z, width);
                    }
                tryCount++;
                count++;
                Console.WriteLine($"tryCount = {tryCount}");
                if (tryCount == 5)
                    return cells;
            }
            return cells;
        }

        private HexCell[] GenerateRock(HexCell[] cells, System.Random rndSeed, int width)
        {
            int maxCount = rndSeed.Next(10, cells.Length / 2);
            int startCell = rndSeed.Next(cells.Length);
            int tryCount = 0;
            while (maxCount != 0)
            {

                neighbourCells = GetNeighboursCell(cells, startCell, width);
                int nextCell = rndSeed.Next(neighbourCells.Count());
                if(neighbourCells[nextCell].CellColor == cells[0].terrainColor)
                {
                    tryCount = 0;
                    int index = IndexFromHexCoords(neighbourCells[nextCell].coords.x, neighbourCells[nextCell].coords.z, width);
                    cells[index].CellColor = cells[0].rockColor;
                    int rndEvaluate = rndSeed.Next(3, 5);
                    cells[index].Elevation = rndEvaluate;
                    cells = GenerateTransition(cells, startCell, width);
                    startCell = index;
                    maxCount--;
                }
                tryCount++;
                if(tryCount == 5)
                    startCell = rndSeed.Next(cells.Length);
            }
            //int startX = rndSeed.Next(cells.Length);
            //cells[startX].color = cells[startX].rockColor;
            return cells;
        }
    }
}
