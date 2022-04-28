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
        static int biomesMaxCellCount;
        static int maxChance;

        static int winterMaxChance;
        static int taigaMaxChance;
        static int standartMaxChance;
        static int tropicMaxChance;
        static int desertMaxChance;

        static int winterMinChance;
        static int taigaMinChance;
        static int standartMinChance;
        static int tropicMinChance;
        static int desertMinChance;


        static int terrainCells;

        static CellList neighbourCells;

        static void SetChance(CellList cells)
        {
            int tmp = 1;
            maxChance = tmp;
            for(int i = 0; i <= cells.CellCountZ / 2; i++)
                for (int j = 0; j < cells.CellCountX; j++)
                {
                    cells.cells[i * cells.CellCountX + j].SpawnChance = i + 1;
                    if(maxChance < i + 1)
                        maxChance = i + 1;
                }

            for(int i = cells.CellCountZ - 1; i >= cells.CellCountZ / 2; i--)
            {
                for (int j = 0; j < cells.CellCountX; j++)
                {
                    cells.cells[i * cells.CellCountX + j].SpawnChance = tmp;
                    if (maxChance < tmp)
                        maxChance = tmp;
                }
                tmp++;
            }
            Debug.Log($"Max spawn chance: {maxChance}");
        }

        static void SetBiomesChance()
        {
            int tmp = maxChance;
            desertMaxChance = tmp;
            desertMinChance = tmp - biomesMaxCellCount;
            tmp -= biomesMaxCellCount;

            tropicMaxChance = tmp + 2;
            tropicMinChance = tmp - biomesMaxCellCount;
            tmp -= biomesMaxCellCount;

            standartMaxChance = tmp + 2;
            standartMinChance = tmp - biomesMaxCellCount - 1;
            tmp -= biomesMaxCellCount;

            taigaMaxChance = tmp;
            taigaMinChance = tmp - biomesMaxCellCount - 3;
            tmp -= biomesMaxCellCount;

            winterMaxChance = tmp - 2;
            winterMinChance = 0;
            tmp -= biomesMaxCellCount;

            Debug.Log($"Spawn chances \n" +
                $"Desert Max: {desertMaxChance}\n" +
                $"Desert Min: {desertMinChance}\n" +
                $"\nTropic Max: {tropicMaxChance}\n" +
                $"Tropic Min: {tropicMinChance}\n" +
                $"\nStandart Max: {standartMaxChance}\n" +
                $"Standart Min: {standartMinChance}\n" +
                $"\nTaiga Max: {taigaMaxChance}\n" +
                $"Taiga Min: {taigaMinChance}\n" +
                $"\nWinter Max: {winterMaxChance}\n" +
                $"Winter Min: {winterMinChance}");
            
        }

        static CellList GenerateTrueRock(CellList cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].CellType == HexCell.CellTypes.terrain && cells[i].Elevation != 0)
                    cells[i].CellType = HexCell.CellTypes.rock;

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

        static CellList DeleteFakeRivers(CellList cells)
        {
            foreach(var cell in cells)
                if(cell.CellType == HexCell.CellTypes.water)
                {
                    int count = 0;
                    var neighbourCells = cells.GetNeighbours(cell.CellIndex);
                    foreach (var nCell in neighbourCells)
                       if (nCell.CellType == HexCell.CellTypes.terrain)
                           count++;
                    if(count >= 4 && count <= 6)
                    {
                        cell.CellType = HexCell.CellTypes.terrain;
                        cell.Elevation = 0;
                    }
                }
            return cells;
        }
        public static CellList GenerateHexMap(CellList cells)
        {
            //Генерируем сначала водный рельеф
            cells = GenerateStartTerrain(cells);
            
            //Генерируем остальное
            //Сначала материковая часть
            cells = GenerateMainlands(cells);
            //Острова
            cells = GenerateIslands(cells);
            cells = DeleteFakeRivers(cells);
            
            //Горы. Сначала нужно получить количество сгенерированных клеток terrain
            GetTerrainCellsCount(cells);
            cells = GenerateRock(cells);

            //После удаления дерьма нужно 
            //Берем "экватор" и устанавливаем шансы на спавн. Чем больше, тем теплее)
            SetChance(cells);
            //Установка количества клеток для каждой климатической зоны
            biomesMaxCellCount = maxChance / 5; //Всего пока что 5 биомов. Думаю достаточно.
            if (biomesMaxCellCount == 0)
                biomesMaxCellCount = 1;         //Это на случай если карта мелкая
            SetBiomesChance();

            //Немного пляжных клеток
            cells = GenerateBeachSells(cells);

            //Короч чтобы все "подводные клетки" были под водой, опускаем их на один уровень
            foreach (var cell in cells)
                if (cell.CellType == HexCell.CellTypes.water)
                    cell.Elevation = -1;

            return cells;
        }
        private static CellList GenerateStartTerrain(CellList cells)
        {
            for(int i = 0; i < cells.Length; i++)
            {
                cells[i].CellType = HexCell.CellTypes.water;
                cells[i].Elevation = 0;
            }
            return cells;
        }

        private static CellList GenerateIslands(CellList cells)
        {
            int startCell = UnityEngine.Random.Range(0, cells.Length);
            while(cells[startCell].CellType == HexCell.CellTypes.terrain)
                startCell = UnityEngine.Random.Range(0, cells.Length);
            int islandsCount = UnityEngine.Random.Range(0, 10);
            
            while(islandsCount != 0)
            {
                neighbourCells = cells.GetNeighbours(startCell);
                neighbourCells.Add(cells[startCell], 0, 0);
                int islandsCellsCount = UnityEngine.Random.Range(0, neighbourCells.Length);
                cells[startCell].CellType = HexCell.CellTypes.terrain;
                for (int i = 0; i < islandsCellsCount; i++)
                {
                    int index = neighbourCells[i].coords.MakeIndex(cells.CellCountX);
                    int rndEvaluate = UnityEngine.Random.Range(0, 1);
                    cells[index].CellType = HexCell.CellTypes.terrain;
                    cells[index].Elevation = rndEvaluate;
                }
                startCell = UnityEngine.Random.Range(0, cells.Length);
                while (cells[startCell].CellType == HexCell.CellTypes.terrain)
                    startCell = UnityEngine.Random.Range(0, cells.Length);
                islandsCount--;
            }
            return cells;
        }
        private static CellList GenerateMainlands(CellList cells)
        {
            int mainlandCount = UnityEngine.Random.Range(3, 5);
            int startCell = UnityEngine.Random.Range(0, cells.Length);
            int tryCount = 0;
            for (int i = 0; i < mainlandCount; i++)
            {
                int maxCount = UnityEngine.Random.Range(cells.CellCountX * cells.CellCountZ - 100, cells.CellCountX * cells.CellCountZ);
                terrainCells = maxCount;
                while (maxCount != 0)
                {
                    int nextCell = startCell;
                    neighbourCells = cells.GetNeighbours(nextCell);
                    neighbourCells.Add(cells[startCell], 0, 0);
                    nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
                    
                    if (neighbourCells[nextCell].CellType == HexCell.CellTypes.terrain)
                        nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
                    else
                        tryCount++;
                    if(tryCount > neighbourCells.Count())
                        nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
                    int index = neighbourCells[nextCell].coords.MakeIndex(cells.CellCountX);
                    cells[index].CellType = HexCell.CellTypes.terrain;
                    int rndEvaluate = UnityEngine.Random.Range(0, 1);
                    cells[index].Elevation = rndEvaluate;
                    startCell = index;
                    maxCount--;
                    if (tryCount == 3)
                        startCell = UnityEngine.Random.Range(0, cells.Length);
                }
                while(cells[startCell].CellType == HexCell.CellTypes.terrain)
                    startCell = UnityEngine.Random.Range(0, cells.Length);

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
                    neighbourCells.Add(cell, 0, 0);
                    foreach (var tCell in neighbourCells)
                        if (tCell.CellType == HexCell.CellTypes.terrain)
                        {
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

        private static CellList GenerateRock(CellList cells)
        {
            int maxCount = UnityEngine.Random.Range(0, terrainCells / 20);
            int startCell = UnityEngine.Random.Range(0, cells.Length);
            while (cells[startCell].CellType == HexCell.CellTypes.water)
                startCell = UnityEngine.Random.Range(0, cells.Length);
            int tryCount = 0;
            while (maxCount != 0)
            {

                neighbourCells = cells.GetNeighbours(startCell);
                neighbourCells.Add(cells[startCell], 0, 0);
                int nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
                if(neighbourCells[nextCell].CellType == HexCell.CellTypes.terrain)
                {
                    tryCount = 0;
                    int index = neighbourCells[nextCell].coords.MakeIndex(cells.CellCountX);
                    cells[index].CellType = HexCell.CellTypes.rock;
                    int rndEvaluate = UnityEngine.Random.Range(3, 4);
                    cells[index].Elevation = rndEvaluate;
                    cells = GenerateTransition(cells, index);
                    startCell = index;
                    maxCount--;
                }
                tryCount++;
                if (tryCount == 3)
                {
                    startCell = UnityEngine.Random.Range(0, cells.Length);
                    while (cells[startCell].CellType == HexCell.CellTypes.water)
                        startCell = UnityEngine.Random.Range(0, cells.Length);
                }               
            }
            cells = GenerateTrueRock(cells);
            return cells;
        }
    }
}
