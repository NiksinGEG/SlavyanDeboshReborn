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
        public struct BiomeChances
        {
            public static int winterMaxChance;
            public static int taigaMaxChance;
            public static int standartMaxChance;
            public static int tropicMaxChance;
            public static int desertMaxChance;

            public static int winterMinChance;
            public static int taigaMinChance;
            public static int standartMinChance;
            public static int tropicMinChance;
            public static int desertMinChance;
        }


        static int terrainCells;

        static CellList neighbourCells;

        private static int GetPercent(int number, int percent)
        {
            return (number * percent) / 100;
        }

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
            BiomeChances.desertMaxChance = tmp;
            BiomeChances.desertMinChance = tmp - biomesMaxCellCount;
            tmp -= biomesMaxCellCount;

            BiomeChances.tropicMaxChance = tmp + BiomeChances.desertMaxChance - BiomeChances.desertMinChance - 2;
            BiomeChances.tropicMinChance = tmp - biomesMaxCellCount;
            tmp -= biomesMaxCellCount;

            BiomeChances.standartMaxChance = tmp + 2;
            BiomeChances.standartMinChance = tmp - biomesMaxCellCount - 1;
            tmp -= biomesMaxCellCount;

            BiomeChances.taigaMaxChance = tmp;
            BiomeChances.taigaMinChance = tmp - biomesMaxCellCount - 3;
            tmp -= biomesMaxCellCount;

            BiomeChances.winterMaxChance = tmp - 2;
            BiomeChances.winterMinChance = 0;
        }

        static CellList GenerateTrueRock(CellList cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].Type == CellType.terrain && cells[i].Elevation != 0)
                {
                    cells[i].SetTypeAndTexture(CellType.rock);
                    if (cells[i].Elevation == 2)
                        cells[i].Texture = CellTexture.winter_rock;
                }           
            }
            return cells;
        }
        
        static void GetTerrainCellsCount(CellList cells)
        {
            terrainCells = 0;
            for (int i = 0; i < cells.Length; i++)
                if (cells[i].Type != CellType.water)
                    terrainCells++;
        }

        static CellList DeleteFakeRivers(CellList cells)
        {
            foreach(var cell in cells)
                if(cell.Type == CellType.water)
                {
                    int count = 0;
                    var neighbourCells = cells.GetNeighbours(cell.CellIndex);
                    foreach (var nCell in neighbourCells)
                       if (nCell.Type == CellType.terrain)
                           count++;
                    if(count >= 4 && count <= 6)
                    {
                        cell.SetTypeAndTexture(CellType.terrain);
                        cell.Elevation = 0;
                    }
                }
            return cells;
        }

        private static void GenerateCurve(HexCell cell, int chooseBiome)
        {
            cell.SetTypeAndTexture((CellType)chooseBiome);
            int curveNum = GlobalVariables.convertor.mixingBiomesCount;
            while(curveNum >= 0)
            {
                foreach (var nCell in cell.neighbours)
                    if(nCell.Type != CellType.rock && nCell.Type != CellType.water)
                        nCell.SetTypeAndTexture(cell.Type);
                curveNum--;
                
                cell = cell.neighbours[UnityEngine.Random.Range(0, cell.neighbours.Length)];
                while (cell.Type == CellType.water || cell.Type == CellType.rock)
                    cell = cell.neighbours[UnityEngine.Random.Range(0, cell.neighbours.Length)];
            }
        }

        private static void GenerateBiomes(CellList cells)
        {
            foreach (var cell in cells)
            {
                if (cell.Type != CellType.water && cell.Type != CellType.rock)
                {
                    if ((BiomeChances.desertMaxChance - (BiomeChances.tropicMaxChance - BiomeChances.desertMinChance)) <= cell.SpawnChance && BiomeChances.desertMaxChance >= cell.SpawnChance)
                    {
                        cell.SetTypeAndTexture(CellType.sand);
                    }
                    if(BiomeChances.tropicMaxChance >= cell.SpawnChance && BiomeChances.desertMinChance <= cell.SpawnChance)
                    {
                           int chooseBiome = UnityEngine.Random.Range(1, 7);
                           if (chooseBiome == 1 || chooseBiome == 6)
                               GenerateCurve(cell, chooseBiome);
                    }
                    if (BiomeChances.desertMinChance >= cell.SpawnChance && BiomeChances.tropicMinChance <= cell.SpawnChance)
                        cell.SetTypeAndTexture(CellType.tropic);
                    if(BiomeChances.tropicMinChance <= cell.SpawnChance && BiomeChances.standartMaxChance >= cell.SpawnChance)
                    {
                        int chooseBiome = UnityEngine.Random.Range(1, 3);
                        if (chooseBiome == 1 || chooseBiome == 2)
                            GenerateCurve(cell, chooseBiome);
                    }
                    if (BiomeChances.tropicMinChance >= cell.SpawnChance && BiomeChances.taigaMaxChance <= cell.SpawnChance)
                        cell.SetTypeAndTexture(CellType.terrain);
                    if (BiomeChances.standartMinChance <= cell.SpawnChance && BiomeChances.taigaMaxChance >= cell.SpawnChance)
                    {
                        int chooseBiome = UnityEngine.Random.Range(2, 4);
                        if (chooseBiome == 2 || chooseBiome == 3)
                            GenerateCurve(cell, chooseBiome);
                    }
                    if (BiomeChances.winterMaxChance <= cell.SpawnChance && BiomeChances.standartMinChance >= cell.SpawnChance)
                        cell.SetTypeAndTexture(CellType.taiga);
                    if(BiomeChances.taigaMinChance <= cell.SpawnChance && BiomeChances.winterMaxChance >= cell.SpawnChance)
                    {
                        int chooseBiome = UnityEngine.Random.Range(3, 5);
                        if (chooseBiome == 3 || chooseBiome == 4)
                            GenerateCurve(cell, chooseBiome);
                    }
                    if (BiomeChances.taigaMinChance >= cell.SpawnChance && BiomeChances.winterMinChance <= cell.SpawnChance)
                        cell.SetTypeAndTexture(CellType.winter);
                }

            }
           
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
            SetBiomesChance();                  //Установка значений, при которых возможен спавн того или иного биома
            GenerateBiomes(cells);



            //Немного пляжных клеток
            //cells = GenerateBeachSells(cells);

            //Короч чтобы все "подводные клетки" были под водой, опускаем их на один уровень
            foreach (var cell in cells)
                if (cell.Type == CellType.water)
                    cell.Elevation = -1;

            return cells;
        }
        private static CellList GenerateStartTerrain(CellList cells)
        {
            for(int i = 0; i < cells.Length; i++)
            {
                cells[i].SetTypeAndTexture(CellType.water);
                cells[i].Elevation = 0;
            }
            return cells;
        }

        private static CellList GenerateIslands(CellList cells)
        {
            int startCell = UnityEngine.Random.Range(0, cells.Length);
            while(cells[startCell].Type == CellType.terrain)
                startCell = UnityEngine.Random.Range(0, cells.Length);
            int islandsCount = UnityEngine.Random.Range(0, 10);
            
            while(islandsCount != 0)
            {
                neighbourCells = cells.GetNeighbours(startCell);
                neighbourCells.Add(cells[startCell], 0, 0);
                int islandsCellsCount = UnityEngine.Random.Range(0, neighbourCells.Length);
                cells[startCell].SetTypeAndTexture(CellType.terrain);

                for (int i = 0; i < islandsCellsCount; i++)
                {
                    int index = neighbourCells[i].coords.MakeIndex(cells.CellCountX);
                    int rndEvaluate = UnityEngine.Random.Range(0, 1);
                    cells[index].SetTypeAndTexture(CellType.terrain);
                    cells[index].Elevation = rndEvaluate;
                }
                startCell = UnityEngine.Random.Range(0, cells.Length);
                while (cells[startCell].Type == CellType.terrain)
                    startCell = UnityEngine.Random.Range(0, cells.Length);
                islandsCount--;
            }
            return cells;
        }
        private static CellList GenerateMainlands(CellList cells)
        {
            int mainlandCount = GlobalVariables.generationSettings.mainlandsCount;
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
                    
                    if (neighbourCells[nextCell].Type == CellType.terrain)
                        nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
                    else
                        tryCount++;
                    if(tryCount > neighbourCells.Count())
                        nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
                    int index = neighbourCells[nextCell].coords.MakeIndex(cells.CellCountX);
                    cells[index].SetTypeAndTexture(CellType.terrain);
                    int rndEvaluate = UnityEngine.Random.Range(0, 1);
                    cells[index].Elevation = rndEvaluate;
                    startCell = index;
                    maxCount--;
                    if (tryCount == 3)
                        startCell = UnityEngine.Random.Range(0, cells.Length);
                }
                while(cells[startCell].Type == CellType.terrain)
                    startCell = UnityEngine.Random.Range(0, cells.Length);

            }
            return cells;
        }
        private static CellList GenerateBeachSells(CellList cells)
        {
            foreach(var cell in cells)
            {
                if(cell.Type == CellType.water)
                {
                    neighbourCells = cells.GetNeighbours(cell.CellIndex);
                    neighbourCells.Add(cell, 0, 0);
                    foreach (var tCell in neighbourCells)
                        if (tCell.Type == CellType.terrain)
                        {
                            cells[tCell.CellIndex].SetTypeAndTexture(CellType.sand);
                        }

                }
            }
            return cells;
        }
        private static CellList GenerateTransition(CellList cells, int startCell, int maxCount)
        {
            neighbourCells = cells.GetNeighbours(startCell);
            int minRockEleation = cells[startCell].Elevation;
            foreach (var cell in neighbourCells)
                if (cell.Type == CellType.rock && minRockEleation > cell.Elevation)
                    minRockEleation = cell.Elevation;
            foreach (var cell in neighbourCells)
            {
                if(cell.Type == CellType.terrain)
                {
                    int index = cell.coords.MakeIndex(cells.CellCountX);
                    cells[index].Elevation = minRockEleation - 1;
                    maxCount--;
                }
            }
            CellList neighbourTerrainCells;
            foreach(var cell in neighbourCells)
            {
                if(cell.Type == CellType.terrain)
                {
                    int indexCellElevation = cell.Elevation;
                    int index = cell.coords.MakeIndex(cells.CellCountX);
                    neighbourTerrainCells = cells.GetNeighbours(index);
                    foreach(var tCell in neighbourTerrainCells)
                        if (tCell.Elevation < cell.Elevation && tCell.Type == CellType.terrain)
                        {
                            int tIndex = tCell.coords.MakeIndex(cells.CellCountX);
                            cells[tIndex].Elevation = indexCellElevation - 1;
                            maxCount--;
                        }
                }
            }
            return cells;
        }

        private static CellList GenerateRock(CellList cells)
        {
            int maxCount = GetPercent(terrainCells, GlobalVariables.generationSettings.rockProcent);
            int startCell = UnityEngine.Random.Range(0, cells.Length);
            while (cells[startCell].Type == CellType.water)
                startCell = UnityEngine.Random.Range(0, cells.Length);
            int tryCount = 0;
            while (maxCount >= 0)
            {

                neighbourCells = cells.GetNeighbours(startCell);
                neighbourCells.Add(cells[startCell], 0, 0);
                int nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
                if(neighbourCells[nextCell].Type == CellType.terrain)
                {
                    tryCount = 0;
                    int index = neighbourCells[nextCell].coords.MakeIndex(cells.CellCountX);
                    cells[index].SetTypeAndTexture(CellType.rock);
                    int rndEvaluate = UnityEngine.Random.Range(3, 4);
                    cells[index].Elevation = rndEvaluate;
                    cells[index].Texture = CellTexture.winter_1;
                    cells = GenerateTransition(cells, index, maxCount);
                    startCell = index;
                    maxCount--;
                }
                tryCount++;
                if (tryCount == 3)
                {
                    startCell = UnityEngine.Random.Range(0, cells.Length);
                    while (cells[startCell].Type == CellType.water)
                        startCell = UnityEngine.Random.Range(0, cells.Length);
                }               
            }
            cells = GenerateTrueRock(cells);
            return cells;
        }
    }
}
