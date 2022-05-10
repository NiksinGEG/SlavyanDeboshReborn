using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Map.WorldMap
{
    public class CellList : IEnumerator<HexCell>, IEnumerable<HexCell>
    {
        int position = -1;
        public HexCell[] cells;
        public int CellCountX { get; private set; }
        public int CellCountZ { get; private set; }
        public int Length
        {
            get{ return cells.Length; }
        }
        public CellList(HexCell[] cells, int countX, int countZ) 
        { 
            this.cells = cells;
            CellCountX = countX;
            CellCountZ = countZ;
        }
        public CellList GetNeighbours(int cellIndex)
        {
            List<HexCell> neighbours = new List<HexCell>();
            for (int i = 0; i < 6; i++)
            {
                HexCoords nei_coords = cells[cellIndex].coords.GetNeighbourCoords(i);
                int nei_index = nei_coords.MakeIndex(CellCountX);
                if (nei_index >= 0 && nei_index < cells.Length)
                    neighbours.Add(cells[nei_index]);
            }
            return new CellList(neighbours.ToArray(), CellCountX, CellCountZ);
        }

        public void Add(HexCell cell, int deltaX, int deltaZ)
        {
            HexCell[] res = new HexCell[cells.Length + 1];
            for (int i = 0; i < cells.Length; i++)
                res[i] = cells[i];
            res[cells.Length] = cell;
            CellCountX += deltaX;
            CellCountZ += deltaZ;
            cells = res;
        }

        public HexCell Get(int x, int z)
        {
            return cells[CellCountX * z + x];
        }

        public HexCell this[int i]
        {
            get {
                try
                {
                    return cells[i] ;
                }
                catch
                {
                    return null;
                }
 }
            set { cells[i] = value; }
        }

        public bool MoveNext()
        {
            position++;
            return position < cells.Length;
        }

        public void Reset()
        {
            position = -1;
        }
        public void Dispose() { }

        public HexCell Current
        {
            get
            {
                if (position == -1 || position >= cells.Length)
                    throw new ArgumentException();
                return cells[position];
            }
        }
        object IEnumerator.Current => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        public IEnumerator<HexCell> GetEnumerator()
        {
            for (int i = 0; i < cells.Length; i++)
                yield return cells[i];
        }
    }
}
