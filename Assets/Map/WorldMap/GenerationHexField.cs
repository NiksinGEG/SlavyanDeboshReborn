using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Map.WorldMap
{
    public class GenerationHexField
    {
        public struct neighbourCells
        {
            public int leftNeighbour;
            public int rightNeighbour;
            public int topNeighbour;
            public int bottomNeighbour;
            public neighbourCells(int leftNeighbour, int rightNeighbour, int topNeighbour, int bottomNeighbour)
            {
                this.leftNeighbour = leftNeighbour;
                this.rightNeighbour = rightNeighbour;
                this.topNeighbour = topNeighbour;
                this.bottomNeighbour = bottomNeighbour;
            }
        }
        private static neighbourCells getNeighbourCells(int i, int width)
        {
            neighbourCells nCells = new neighbourCells(i - 1, i + 1, i - width, i + width);
            return nCells;
        }

        public static HexCell[] GenerateHexMap(HexCell[] cells, System.Random rndSeed, int width, int height)
        {
            //cells = GenerateRock(cells, rndSeed);
            neighbourCells nCells = new neighbourCells();

            nCells = getNeighbourCells(564, width);

            cells[564].color = cells[0].neighboorColor;
            cells[nCells.leftNeighbour].color = cells[0].neighboorColor;
            cells[nCells.rightNeighbour].color = cells[0].neighboorColor;
            cells[nCells.topNeighbour].color = cells[0].neighboorColor;
            cells[nCells.bottomNeighbour].color = cells[0].neighboorColor;

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
