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

        //static HexCell[] GenerateTrueRock(HexCell[] cells)
        static CellList GenerateTrueRock(CellList cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].CellColor == cells[0].terrainColor && cells[i].Elevation != 0)
                    cells[i].CellColor = cells[0].rockColor;
            }
            return cells;
        }
        static void GetTerrainCellsCount(CellList cells)
        {
            terrainCells = 0;
            for (int i = 0; i < cells.Length; i++)
                if (cells[i].CellColor == cells[0].terrainColor)
                    terrainCells++;
        }

        public static CellList GenerateHexMap(CellList cells, System.Random rndSeed, int width, int height)
        {
            //cells = GenerateRock(cells, rndSeed);

            //Генерируем сначала водный рельеф
            cells = GenerateStartTerrain(cells, rndSeed, width);
            
            //Генерируем остальное
            //Сначала материковая часть
            cells = GenerateMainlands(cells, rndSeed, width, height);
            //Острова
            cells = GenerateIslands(cells, rndSeed, width);
            //Горы. Сначала нужно получить количество сгенерированных клеток terrain
            GetTerrainCellsCount(cells);
            cells = GenerateRock(cells, rndSeed, width);
            //Немного пляжных клеток
            cells = GenerateBeachSells(cells, width);
            //cells = aboba.SwitchBorderColors(cells, width);

            //Тут будет всякая хрень для украшательств
            for(int i = 0; i < cells.Length; i++)
            {
                //aboba.AddFeature(cells[i].transform.localPosition);
            }

            return cells;
        }
        private static CellList GenerateStartTerrain(CellList cells, System.Random rndSeed, int width)
        {
            for(int i = 0; i < cells.Length; i++)
            {
                int rndElevation = rndSeed.Next(-2, 0);
                cells[i].CellColor = cells[i].waterColor;
                cells[i].Elevation = 0;
            }
            return cells;
        }
        private static CellList GenerateIslands(CellList cells, System.Random rndSeed, int width)
        {
            int startCell = rndSeed.Next(cells.Length);
            while(cells[startCell].CellColor == cells[startCell].terrainColor)
                startCell = rndSeed.Next(cells.Length);
            int islandsCount = rndSeed.Next(5, 10);
            
            while(islandsCount != 0)
            {
                neighbourCells = cells.GetNeighbours(startCell);
                int islandsCellsCount = rndSeed.Next(2, neighbourCells.Length);
                for(int i = 0; i < islandsCellsCount; i++)
                {
                    int index = neighbourCells[i].coords.MakeIndex(width);
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
        private static CellList GenerateMainlands(CellList cells, System.Random rndSeed, int width, int height)
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
                    neighbourCells = cells.GetNeighbours(nextCell);
                    nextCell = rndSeed.Next(neighbourCells.Count());
                    
                    if (neighbourCells[nextCell].CellColor == neighbourCells[nextCell].terrainColor)
                        nextCell = rndSeed.Next(neighbourCells.Count());
                    else
                        tryCount++;
                    if(tryCount > neighbourCells.Count())
                        nextCell = rndSeed.Next(neighbourCells.Count());
                    int index = neighbourCells[nextCell].coords.MakeIndex(width);
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
        private static CellList GenerateBeachSells(CellList cells, int width)
        {
            foreach(var cell in cells)
            {
                if(cell.CellColor == cell.waterColor)
                {
                    neighbourCells = cells.GetNeighbours(cell.CellIndex);
                    foreach (var tCell in neighbourCells)
                        if (tCell.CellColor == tCell.terrainColor)
                            cells[tCell.CellIndex].CellColor = tCell.desertColor;
                }
            }
            return cells;
        }
        private static CellList GenerateTransition(CellList cells, int startCell, int width)
        {
            neighbourCells = cells.GetNeighbours(startCell);
            int minRockEleation = cells[startCell].Elevation;
            foreach (var cell in neighbourCells)
                if (cell.CellColor == cell.rockColor && minRockEleation > cell.Elevation)
                    minRockEleation = cell.Elevation;
            foreach (var cell in neighbourCells)
            {
                if(cell.CellColor == cell.terrainColor)
                {
                    int index = cell.coords.MakeIndex(width);
                    cells[index].Elevation = minRockEleation - 1;
                }
            }
            CellList neighbourTerrainCells;
            foreach(var cell in neighbourCells)
            {
                if(cell.CellColor == cell.terrainColor)
                {
                    int indexCellElevation = cell.Elevation;
                    int index = cell.coords.MakeIndex(width);
                    neighbourTerrainCells = cells.GetNeighbours(index);
                    foreach(var tCell in neighbourTerrainCells)
                    {
                        if (tCell.Elevation < cell.Elevation && tCell.CellColor == tCell.terrainColor)
                        {
                            int tIndex = tCell.coords.MakeIndex(width);
                            cells[tIndex].Elevation = indexCellElevation - 1;
                        }
                    }
                }
            }
            return cells;
        }

        private static CellList GenerateRock(CellList cells, System.Random rndSeed, int width)
        {
            int maxCount = rndSeed.Next(0, terrainCells / 20);
            int startCell = rndSeed.Next(cells.Length);
            while (cells[startCell].CellColor == cells[startCell].waterColor)
                startCell = rndSeed.Next(cells.Length);
            int tryCount = 0;
            while (maxCount != 0)
            {

                neighbourCells = cells.GetNeighbours(startCell);//GetNeighboursCell(cells, startCell, width);
                int nextCell = rndSeed.Next(neighbourCells.Count());
                if(neighbourCells[nextCell].CellColor == cells[0].terrainColor)
                {
                    tryCount = 0;
                    int index = neighbourCells[nextCell].coords.MakeIndex(width);
                    cells[index].CellColor = cells[0].rockColor;
                    int rndEvaluate = rndSeed.Next(3, 4);
                    cells[index].Elevation = rndEvaluate;
                    cells = GenerateTransition(cells, index, width);
                    startCell = index;
                    maxCount--;
                }
                tryCount++;
                if (tryCount == 3)
                {
                    startCell = rndSeed.Next(cells.Length);
                    while (cells[startCell].CellColor == cells[startCell].waterColor)
                        startCell = rndSeed.Next(cells.Length);
                }               
            }
            cells = GenerateTrueRock(cells);
            return cells;
        }
    }
}
