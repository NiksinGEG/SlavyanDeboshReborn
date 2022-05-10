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
    public enum CellTexture { water,tropic_1, tropic_2, tropic_3, tropic_4, tropic_5, terrain_1, 
       terrain_2, terrain_3, terrain_4, terrain_5,sand_1, sand_2, sand_3, sand_4, sand_5,
       taiga_1, taiga_2, taiga_3, taiga_4, taiga_5, winter_1, winter_2, winter_3, winter_4, winter_5, 
       winter_rock, rock, tropic_dirt, forest_dirt}
    public enum CellType { water, tropic, terrain, taiga, winter, rock, sand, forest_dirt, tropic_dirt}

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


        [SerializeField] public float Height { get; set; }
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

        public void SetTypeAndTexture(CellType cellType)
        {
            this.Type = cellType;
            switch ((int)this.Type)
            {
                case 0:
                    this.Texture = (CellTexture)(UnityEngine.Random.Range(11, 15));
                    break;
                case 1:
                    this.Texture = (CellTexture)(UnityEngine.Random.Range(1, 5));
                    break;
                case 2:
                    this.Texture = (CellTexture)(UnityEngine.Random.Range(6, 10));
                    break;
                case 3:
                    this.Texture = (CellTexture)(UnityEngine.Random.Range(16, 20));
                    break;
                case 4:
                    this.Texture = (CellTexture)(UnityEngine.Random.Range(21, 25));
                    break;
                case 5:
                    this.Texture = CellTexture.rock;
                    break;
                case 6:
                    this.Texture = (CellTexture)(UnityEngine.Random.Range(12, 16));
                    break;
                case 7:
                    this.Texture = CellTexture.forest_dirt;
                    break;
                case 8:
                    this.Texture = CellTexture.tropic_dirt;
                    break;
            }      
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
