using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Map.WorldMap
{
    public class GenerationHexField
    {
        public List<HexCell> neighbourCells;
        int IndexFromHexCoords(int x, int z, int mapWidth)
        {
            return x + z * mapWidth + z / 2;
        }

        HexCell[] SwitchBorderColors(HexCell[] cells, int width)
        {
            int i = 0;
            foreach (HexCell cell in neighbourCells)
            {
                i = IndexFromHexCoords(cell.coords.x, cell.coords.z, width);
                cells[i].CellColor = cells[i].neighboorColor;
            }
            return cells;
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
            }
            return neighbours;
        }

        public static HexCell[] GenerateHexMap(HexCell[] cells, System.Random rndSeed, int width, int height)
        {
            //cells = GenerateRock(cells, rndSeed);

            GenerationHexField aboba = new GenerationHexField();
            cells = aboba.GenerateRock(cells, rndSeed, width);
            //cells = aboba.SwitchBorderColors(cells, width);

            return cells;
        }



        private HexCell[] GenerateRock(HexCell[] cells, System.Random rndSeed, int width)
        {
            int maxCount = rndSeed.Next(200);
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
                    int rndEvaluate = rndSeed.Next(0, 5);
                    cells[index].Elevation = rndEvaluate;
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
