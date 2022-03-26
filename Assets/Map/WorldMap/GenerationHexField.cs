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
                cells[i].color = cells[i].neighboorColor;
            }
            return cells;
        }
        private static int GetFirstNumber(int a)
        {
            double b = Convert.ToDouble(a);
            while (b > 100)
                b /= 10;
            a = Convert.ToInt16(Math.Truncate(b));
            return a;
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
            cells = aboba.GenerateRock(cells, rndSeed);
            cells = aboba.SwitchBorderColors(cells, width);

            return cells;
        }



        private HexCell[] GenerateRock(HexCell[] cells, System.Random rndSeed)
        {         
            int startX = rndSeed.Next(cells.Length);
            cells[startX].color = cells[startX].rockColor;
            return cells;
        }
    }
}
