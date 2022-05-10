using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityEngine;
using Assets.Scripts.Logger;

namespace Assets.Map.WorldMap
{
    public static class HexFieldGenerator
    {
        private static float _baseHeight = 100f;
        private static float _roughness = 1f;
        private static bool _raiseToSecondPower = true;
        private static int _smoothCycles = 1;

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

        public static float[,] GenerateHexMap(float[,] weightMatrix)
        {
            /*
            //Инициализация логгера и счетчика
            Loger.getInstance();
            Loger.DeleteLog("Logs\\HexMapGenerator.log");
            Stopwatch sw = new Stopwatch();


            //Генерируем сначала водный рельеф

            Loger.Log("Logs\\HexMapGenerator.log", $"{DateTime.Now} - GenerateStartTerrain: START");
            sw.Start();
            cells = GenerateStartTerrain(cells);
            sw.Stop();
            Loger.Log("Logs\\HexMapGenerator.log", $"{DateTime.Now} - GenerateStartTerrain: FINISH({sw.ElapsedMilliseconds}ms)");

            //Генерируем остальное
            //Сначала материковая часть

            Loger.Log("Logs\\HexMapGenerator.log", $"{DateTime.Now} - GenerateMainlands: START");
            sw.Start();
            cells = GenerateMainlands(cells);
            sw.Stop();
            Loger.Log("Logs\\HexMapGenerator.log", $"{DateTime.Now} - GenerateMainlands: FINISH({sw.ElapsedMilliseconds}ms)");

            
            Loger.Log("Logs\\HexMapGenerator.log", $"{DateTime.Now} - DeleteFakeRivers: START");
            sw.Start();
            cells = DeleteFakeRivers(cells);
            sw.Stop();
            Loger.Log("Logs\\HexMapGenerator.log", $"{DateTime.Now} - DeleteFakeRivers: FINISH({sw.ElapsedMilliseconds}ms)");


            Loger.Log("Logs\\HexMapGenerator.log", $"{DateTime.Now} - GenerateRock: START");
            sw.Start();
            cells = GenerateRock(cells);
            sw.Stop();
            Loger.Log("Logs\\HexMapGenerator.log", $"{DateTime.Now} - GenerateRock: FINISH({sw.ElapsedMilliseconds}ms)");

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
                    cell.Elevation = -1;*/
            InitHeight(weightMatrix);
            AddRandomToAngles(weightMatrix);
            GenerateDiamondSquareTerrain(weightMatrix);

            //FromHeightToTexture(cells);
            return weightMatrix;
        }

        private static void InitHeight(float[,] weightMatrix)
        {
            for (int i = 0; i < GlobalVariables.generationSettings.terrainChunkCountX * 3; i++)
                for (int j = 0; j < GlobalVariables.generationSettings.terrainChunkCountY * 3; j++)
                    weightMatrix[i, j] = float.MinValue;
        }

        private static void AddRandomToAngles(float[,] weightMatrix)
        {
            float initVal = GlobalVariables.generationSettings.terrainChunkCountX - 1;

            weightMatrix[0, 0] = UnityEngine.Random.Range(-initVal, initVal);
            weightMatrix[0, GlobalVariables.generationSettings.terrainChunkCountY - 1] = UnityEngine.Random.Range(-initVal, initVal);
            weightMatrix[GlobalVariables.generationSettings.terrainChunkCountX - 1, 0] = UnityEngine.Random.Range(-initVal, initVal);
            weightMatrix[GlobalVariables.generationSettings.terrainChunkCountX - 1, GlobalVariables.generationSettings.terrainChunkCountY - 1] = UnityEngine.Random.Range(-initVal, initVal);
            UnityEngine.Debug.Log("AMOGUS!");
        }

        private static void RaiseToSecondPower(float[,] weightMatrix)
        {
            for (int i = 0; i < GlobalVariables.generationSettings.terrainChunkCountX * 3; i++)
                for (int j = 0; j < GlobalVariables.generationSettings.terrainChunkCountY * 3; j++)
                    weightMatrix[i, j] = (float)Math.Pow(weightMatrix[i, j], 2);
        }

        private static void GenerateDiamondSquareTerrain(float[,] weightMatrix)
        {
            for (int i = 0; i < GlobalVariables.generationSettings.terrainChunkCountX * 3; i++)
                for (int j = 0; j < GlobalVariables.generationSettings.terrainChunkCountY * 3; j++)
                    weightMatrix[i, j] = GetHeight(weightMatrix, i, j);

            if (_raiseToSecondPower)
                RaiseToSecondPower(weightMatrix);

            Normalize(weightMatrix);

        }

        private static float GetHeight(float[,] weightMatrix, int x, int y)
        {
            if (x < 0 || x > GlobalVariables.generationSettings.terrainChunkCountX * 3 - 1 || y < 0 || y > GlobalVariables.generationSettings.terrainChunkCountY * 3 - 1)
                return 0;
            if (weightMatrix[x, y] != float.MinValue)
                return weightMatrix[x, y];
            var baseSize = 1;
            while (((x & baseSize) == 0) && ((y & baseSize) == 0))
                baseSize <<= 1;

            if (((x & baseSize) != 0) && ((y & baseSize) != 0))
                return SquareStep(weightMatrix, x, y, baseSize * 2);
            else
                return DiamondStep(weightMatrix, x, y, baseSize * 2);
        }

        public static float SquareStep(float[,] weightMatrix, int x, int y, int currMax)
        {
            weightMatrix[x, y] = Displace((GetHeight(weightMatrix, x - currMax / 2, y - currMax / 2) +
                     GetHeight(weightMatrix, x + currMax / 2, y - currMax / 2) +
                     GetHeight(weightMatrix, x + currMax / 2, y + currMax / 2) +
                     GetHeight(weightMatrix, x - currMax / 2, y + currMax / 2)) / 4,
                            _roughness, currMax);

            return weightMatrix[x, y];
        }

        public static float DiamondStep(float[,] weightMatrix, int x, int y, int currMax)
        {
            var halfSize = currMax / 2;

            weightMatrix[x, y] = Displace((GetHeight(weightMatrix, x, y - halfSize) +
                         GetHeight(weightMatrix, x + halfSize, y) +
                         GetHeight(weightMatrix, x, y + halfSize) +
                         GetHeight(weightMatrix, x - halfSize, y)) / 4,
                                _roughness, currMax);

            return weightMatrix[x, y];
        }

        public static float Displace(float val, float roughness, int currMax)
        {
            var diff = Math.Max(0.5f, Math.Min(val / _baseHeight, 1f));
            var rnd = (float)(UnityEngine.Random.Range(0f, 1f) * 2f - 1f);
            return val + rnd * roughness * (float)currMax;
        }

        private static void Normalize(float[,] weightMatrix)
        {
            var max = weightMatrix[0, 0];

            for (int i = 0; i < GlobalVariables.generationSettings.terrainChunkCountX * 3; i++)
                for (int j = 0; j < GlobalVariables.generationSettings.terrainChunkCountY * 3; j++)
                {
                    if (weightMatrix[i, j] > max)
                    {
                        max = weightMatrix[i, j];
                    }
                }
            if (max == float.MinValue) return;

            for (int i = 0; i < GlobalVariables.generationSettings.terrainChunkCountX * 3; i++)
                for (int j = 0; j < GlobalVariables.generationSettings.terrainChunkCountY * 3; j++)
                {
                    weightMatrix[i, j] /= max;
                }
        }    
    }
}
