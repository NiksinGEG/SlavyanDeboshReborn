using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Map.WorldMap
{
    public static class HexFieldGenerator
    {
        static int terrainCells;
        static CellList neighbourCells;

        static CellList GenerateTrueRock(CellList cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].CellType == HexCell.CellTypes.terrain && cells[i].Elevation != 0)
                {
                    cells[i].CellColor = Color.gray;
                    cells[i].CellType = HexCell.CellTypes.rock;
                }

            }
            return cells;
        }
        static void GetTerrainCellsCount(CellList cells)
        {
            terrainCells = 0;
            for (int i = 0; i < cells.Length; i++)
                if (cells[i].CellType == HexCell.CellTypes.terrain)
                    terrainCells++;
        }

        public static CellList GenerateHexMap(CellList cells, System.Random rndSeed)
        {
            //Генерируем сначала водный рельеф
            cells = GenerateStartTerrain(cells, rndSeed);
            
            //Генерируем остальное
            //Сначала материковая часть
            cells = GenerateMainlands(cells, rndSeed);
            //Острова
            cells = GenerateIslands(cells, rndSeed);
            //Горы. Сначала нужно получить количество сгенерированных клеток terrain
            GetTerrainCellsCount(cells);
            //Перепады высот внутри материков-островов


            cells = GenerateRock(cells, rndSeed);
            //Немного пляжных клеток
            cells = GenerateBeachSells(cells);
            return cells;
        }
        private static CellList GenerateStartTerrain(CellList cells, System.Random rndSeed)
        {
            for(int i = 0; i < cells.Length; i++)
            {
                cells[i].CellType = HexCell.CellTypes.water;
                cells[i].CellColor = Color.blue;
                cells[i].Elevation = 0;
            }
            return cells;
        }

        private static CellList GenerateIslands(CellList cells, System.Random rndSeed)
        {
            int startCell = rndSeed.Next(cells.Length);
            while(cells[startCell].CellType == HexCell.CellTypes.terrain)
                startCell = rndSeed.Next(cells.Length);
            int islandsCount = rndSeed.Next(5, 10);
            
            while(islandsCount != 0)
            {
                neighbourCells = cells.GetNeighbours(startCell);
                int islandsCellsCount = rndSeed.Next(2, neighbourCells.Length);
                cells[startCell].CellColor = Color.green;
                cells[startCell].CellType = HexCell.CellTypes.terrain;
                for (int i = 0; i < islandsCellsCount; i++)
                {
                    int index = neighbourCells[i].coords.MakeIndex(cells.CellCountX);
                    int rndEvaluate = rndSeed.Next(0, 1);
                    cells[index].CellColor = Color.green;
                    cells[index].CellType = HexCell.CellTypes.terrain;
                    cells[index].Elevation = rndEvaluate;
                }
                startCell = rndSeed.Next(cells.Length);
                while (cells[startCell].CellType == HexCell.CellTypes.terrain)
                    startCell = rndSeed.Next(cells.Length);
                islandsCount--;
            }
            return cells;
        }
        private static CellList GenerateMainlands(CellList cells, System.Random rndSeed)
        {
            int mainlandCount = rndSeed.Next(2, 4);
            int startCell = rndSeed.Next(cells.Length);
            int tryCount = 0;
            for (int i = 0; i < mainlandCount; i++)
            {
                int maxCount = rndSeed.Next(cells.CellCountX * cells.CellCountZ - 100, cells.CellCountX * cells.CellCountZ);
                terrainCells = maxCount;
                while (maxCount != 0)
                {
                    int nextCell = startCell;
                    neighbourCells = cells.GetNeighbours(nextCell);
                    nextCell = rndSeed.Next(neighbourCells.Count());
                    
                    if (neighbourCells[nextCell].CellType == HexCell.CellTypes.terrain)
                        nextCell = rndSeed.Next(neighbourCells.Count());
                    else
                        tryCount++;
                    if(tryCount > neighbourCells.Count())
                        nextCell = rndSeed.Next(neighbourCells.Count());
                    int index = neighbourCells[nextCell].coords.MakeIndex(cells.CellCountX);
                    cells[index].CellColor = Color.green;
                    cells[index].CellType = HexCell.CellTypes.terrain;
                    int rndEvaluate = rndSeed.Next(0, 1);
                    cells[index].Elevation = rndEvaluate;
                    startCell = index;
                    maxCount--;
                    if (tryCount == 3)
                        startCell = rndSeed.Next(cells.Length);
                }
                while(cells[startCell].CellType == HexCell.CellTypes.terrain)
                    startCell = rndSeed.Next(cells.Length);

            }
            return cells;
        }
        private static CellList GenerateBeachSells(CellList cells)
        {
            foreach(var cell in cells)
            {
                if(cell.CellType == HexCell.CellTypes.water)
                {
                    neighbourCells = cells.GetNeighbours(cell.CellIndex);
                    foreach (var tCell in neighbourCells)
                        if (tCell.CellType == HexCell.CellTypes.terrain)
                        {
                            cells[tCell.CellIndex].CellColor = Color.yellow;
                            cells[tCell.CellIndex].CellType = HexCell.CellTypes.sand;
                        }

                }
            }
            return cells;
        }
        private static CellList GenerateTransition(CellList cells, int startCell)
        {
            neighbourCells = cells.GetNeighbours(startCell);
            int minRockEleation = cells[startCell].Elevation;
            foreach (var cell in neighbourCells)
                if (cell.CellType == HexCell.CellTypes.rock && minRockEleation > cell.Elevation)
                    minRockEleation = cell.Elevation;
            foreach (var cell in neighbourCells)
            {
                if(cell.CellType == HexCell.CellTypes.terrain)
                {
                    int index = cell.coords.MakeIndex(cells.CellCountX);
                    cells[index].Elevation = minRockEleation - 1;
                }
            }
            CellList neighbourTerrainCells;
            foreach(var cell in neighbourCells)
            {
                if(cell.CellType == HexCell.CellTypes.terrain)
                {
                    int indexCellElevation = cell.Elevation;
                    int index = cell.coords.MakeIndex(cells.CellCountX);
                    neighbourTerrainCells = cells.GetNeighbours(index);
                    foreach(var tCell in neighbourTerrainCells)
                        if (tCell.Elevation < cell.Elevation && tCell.CellType == HexCell.CellTypes.terrain)
                        {
                            int tIndex = tCell.coords.MakeIndex(cells.CellCountX);
                            cells[tIndex].Elevation = indexCellElevation - 1;
                        }
                }
            }
            return cells;
        }

        private static CellList GenerateRock(CellList cells, System.Random rndSeed)
        {
            int maxCount = rndSeed.Next(0, terrainCells / 20);
            int startCell = rndSeed.Next(cells.Length);
            while (cells[startCell].CellType == HexCell.CellTypes.water)
                startCell = rndSeed.Next(cells.Length);
            int tryCount = 0;
            while (maxCount != 0)
            {

                neighbourCells = cells.GetNeighbours(startCell);
                int nextCell = rndSeed.Next(neighbourCells.Count());
                if(neighbourCells[nextCell].CellType == HexCell.CellTypes.terrain)
                {
                    tryCount = 0;
                    int index = neighbourCells[nextCell].coords.MakeIndex(cells.CellCountX);
                    cells[index].CellColor = Color.gray;
                    cells[index].CellType = HexCell.CellTypes.rock;
                    int rndEvaluate = rndSeed.Next(3, 4);
                    cells[index].Elevation = rndEvaluate;
                    cells = GenerateTransition(cells, index);
                    startCell = index;
                    maxCount--;
                }
                tryCount++;
                if (tryCount == 3)
                {
                    startCell = rndSeed.Next(cells.Length);
                    while (cells[startCell].CellType == HexCell.CellTypes.water)
                        startCell = rndSeed.Next(cells.Length);
                }               
            }
            cells = GenerateTrueRock(cells);
            return cells;
        }
    }
}
