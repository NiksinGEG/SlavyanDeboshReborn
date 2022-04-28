using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Map.WorldMap
{
    public class HexCellEventArgs : EventArgs
    {
        public Vector3 localPosition;
        public HexCellEventArgs(Vector3 localPosition)
        {
            this.localPosition = localPosition;
        }
    }
    public enum HexDirection
    {
        NE, E, SE, SW, W, NW
    }
    public enum CellTexture { water, terrain, rock, sand, dirt }
    public enum CellType { water, terrain, rock, sand, dirt }
    public static class HexDirectionExtensions
    {
        public static HexDirection Previous(this HexDirection direction)
        {
            return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
        }
        public static HexDirection Next(this HexDirection direction)
        {
            return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
        }
        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }
    }

    public class HexCell : MonoBehaviour
    {

        

        [SerializeField] public int spawnChance;
        public int SpawnChance { get; set; }

        public CellType Type { get; set; }
        public CellTexture Texture { get; set; }
        

        public List<Vector3> vertices;
        public CellList neighbours;
        public bool[] Bridges = new bool[6];

        public int CellIndex { get; set; }

        [SerializeField] public HexCoords coords;

        public EventHandler<HexCellEventArgs> MouseLeftClick;

        private int elevation;
        public int Elevation
        {
            get { return elevation; }
            set
            {
                elevation = value;
                Vector3 pos = transform.localPosition;
                pos.y = value * HexMetrics.elevationStep;
                transform.localPosition = pos;
            }
        }

        public HexCell() 
        { 
            MouseLeftClick += Choose; 
            vertices = new List<Vector3>();
            Bridges = new bool[6];
            for (int i = 0; i < Bridges.Length; i++)
                Bridges[i] = false;
        }

        public void Choose(object sender, HexCellEventArgs e)
        {
            
        }

        /// <summary>
        /// Возвращает положение переданной клетки относительно той, для которой вызывается этот метод
        /// </summary>
        /// <param name="to">Клетка, относительное положение которой надо найти</param>
        /// <returns>Для правой верхней возвращает 0 и далее по часовой стрелке (для левой верхней вернёт 5). В случае ошибки (к примеру клетки не соседние) вернёт -1</returns>
        public int GetDirection(HexCell to)
        {
            if (to.coords.y == coords.y - 1 && to.coords.z == coords.z + 1)
                return 0;
            if (to.coords.x == coords.x + 1 && to.coords.y == coords.y - 1)
                return 1;
            if (to.coords.x == coords.x + 1 && to.coords.z == coords.z - 1)
                return 2;
            if (to.coords.y == coords.y + 1 && to.coords.z == coords.z - 1)
                return 3;
            if (to.coords.x == coords.x - 1 && to.coords.y == coords.y + 1)
                return 4;
            if (to.coords.x == coords.x - 1 && to.coords.z == coords.z + 1)
                return 5;
            return -1;
        }

        public HexCell GetNeighbour(int direction)
        {
            HexCoords nei_coords = coords.GetNeighbourCoords(direction);
            foreach (var nei in neighbours)
                if (nei.coords.EqualsTo(nei_coords))
                    return nei;
            return null;
        }
    }
}
