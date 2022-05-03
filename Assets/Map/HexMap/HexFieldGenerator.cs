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
        static CellList neighbourCells; //Временная заглушка

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

        //Функция получения всех водных клеток в лист HexCell
        private static List<HexCell> CreateWaterCellsList(CellList cells)
        {
            List<HexCell> waterList = new List<HexCell>();
            foreach (var cell in cells)
                if (cell.Type == CellType.water)
                    waterList.Add(cell);
            return waterList;
        }

        //Функция получения всех клеток суши
        private static List<HexCell> CreateTerrainCellsList(CellList cells)
        {
            List<HexCell> terrainList = new List<HexCell>();
            foreach (var cell in cells)
                if (cell.Type == CellType.terrain)
                    terrainList.Add(cell);
            return terrainList;
        }

        private static int GetPercent(int number, int percent) { return (number * percent) / 100; }

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
            int curveNum = GlobalVariables.generationSettings.mixingBiomesCount;
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
            cells = DeleteFakeRivers(cells); 
            
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

        //Функция генерации одного острова
        private static void GenerateIsland(ref CellList cells, ref List<HexCell> waterList, int startCell)
        {
            neighbourCells = cells.GetNeighbours(startCell);
            neighbourCells.Add(cells[startCell], 0, 0);
            int islandsCellsCount = UnityEngine.Random.Range(0, neighbourCells.Length);
            cells[startCell].SetTypeAndTexture(CellType.terrain);
            waterList.Remove(cells[startCell]);
            for (int i = 0; i < islandsCellsCount; i++)
            {
                cells[neighbourCells[i].CellIndex].SetTypeAndTexture(CellType.terrain);
                cells[neighbourCells[i].CellIndex].Elevation = 1;
                waterList.Remove(cells[neighbourCells[i].CellIndex]);
            }
        }

        //Функция генерации островов
        private static CellList GenerateIslands(CellList cells)
        {
            List<HexCell> waterList = CreateWaterCellsList(cells);
            int startCell = waterList[UnityEngine.Random.Range(0, waterList.Count)].CellIndex;
            int islandsCount = UnityEngine.Random.Range(waterList.Count / 6, waterList.Count);

            while (islandsCount != 0)
            {
                GenerateIsland(ref cells, ref waterList, startCell);
                startCell = waterList[UnityEngine.Random.Range(0, waterList.Count)].CellIndex;
                islandsCount--;
            }
            return cells;
        }

        //Функция получения следующей клетки
        private static int ChooseNextMainlandCell(int nextCell, int tryCount)
        {
            nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
            if (neighbourCells[nextCell].Type == CellType.terrain)
                nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
            else
                tryCount++;
            if (tryCount > neighbourCells.Count())
                nextCell = UnityEngine.Random.Range(0, neighbourCells.Count());
            return nextCell;
        }

        //Функция генерации одного материка
        private static void GenerateMainland(CellList cells, List<HexCell> waterList, int startCell)
        {
            int tryCount = 0;
            int maxCount = UnityEngine.Random.Range(cells.CellCountX * cells.CellCountZ - 100, cells.CellCountX * cells.CellCountZ);
            while (maxCount != 0)
            {
                int nextCell = startCell;
                neighbourCells = cells.GetNeighbours(nextCell);
                neighbourCells.Add(cells[startCell], 0, 0);
                nextCell = ChooseNextMainlandCell(nextCell, tryCount);
                int index = neighbourCells[nextCell].coords.MakeIndex(cells.CellCountX);
                cells[index].SetTypeAndTexture(CellType.terrain);
                int rndEvaluate = UnityEngine.Random.Range(0, 1);
                cells[index].Elevation = rndEvaluate;
                waterList.Remove(cells[index]);
                startCell = index;
                maxCount--;
                if (tryCount == 3)
                    startCell = waterList[UnityEngine.Random.Range(0, waterList.Count)].CellIndex;
            }
        }

        //Функция генерации материкового ландшафта
        private static CellList GenerateMainlands(CellList cells)
        {
            List<HexCell> waterList = CreateWaterCellsList(cells);

            int mainlandCount = GlobalVariables.generationSettings.mainlandsCount;
            int startCell = UnityEngine.Random.Range(0, waterList.Count);
            for (int i = 0; i < mainlandCount; i++)
            {
                GenerateMainland(cells, waterList, startCell);
                startCell = waterList[UnityEngine.Random.Range(0, waterList.Count)].CellIndex;
            }
            return cells;
        }

        private static CellList GenerateTransition(CellList cells, int startCell, ref int maxCount)
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

        //Функция генерации горного биома
        private static void GenerateRockBiome(ref CellList cells, ref List<HexCell> terrainList, ref int startCell, ref int maxCount)
        {
            List<HexCell> startRockCells = new List<HexCell>();
            int rockRangeCellsCount = UnityEngine.Random.Range(3, 5);
            int nextCell = UnityEngine.Random.Range(0, cells[startCell].neighbours.Length);

            while (rockRangeCellsCount >= 0)
            {
                if (cells[startCell].neighbours[nextCell].Type == CellType.terrain)
                {
                    cells[cells[startCell].neighbours[nextCell].CellIndex].SetTypeAndTexture(CellType.rock);
                    int Evaluate = 3;
                    cells[cells[startCell].neighbours[nextCell].CellIndex].Elevation = Evaluate;
                    cells[cells[startCell].neighbours[nextCell].CellIndex].Texture = CellTexture.winter_1;
                    terrainList.Remove(cells[startCell].neighbours[nextCell]);

                    startRockCells.Add(cells[startCell].neighbours[nextCell]);

                    cells[startCell].SetTypeAndTexture(CellType.rock);
                    cells[startCell].Texture = CellTexture.winter_1;
                    terrainList.Remove(cells[startCell]);
                    startRockCells.Add(cells[startCell]);
                    cells[startCell].Elevation = Evaluate;

                    rockRangeCellsCount -= 2;
                    maxCount -= 2;

                    nextCell = UnityEngine.Random.Range(0, cells[startCell].neighbours.Length);
                    startCell = cells[startCell].neighbours[nextCell].CellIndex;
                }
                else
                    nextCell = UnityEngine.Random.Range(0, cells[startCell].neighbours.Length);

            }
            foreach (var cell in startRockCells)
                GenerateTransition(cells, cell.CellIndex, ref maxCount);
        }

        //Функция генерации горного рельефа
        private static CellList GenerateRock(CellList cells)
        {
            List<HexCell> terrainList = CreateTerrainCellsList(cells);
            int maxCount = GetPercent(terrainList.Count, GlobalVariables.generationSettings.rockProcent);
            int startCell = terrainList[UnityEngine.Random.Range(0, terrainList.Count)].CellIndex;

            while (maxCount >= 0)
            {
                GenerateRockBiome(ref cells, ref terrainList, ref startCell, ref maxCount);
                startCell = terrainList[UnityEngine.Random.Range(0, terrainList.Count)].CellIndex;
            }
            cells = GenerateTrueRock(cells);
            return cells;
        }
    }
}
