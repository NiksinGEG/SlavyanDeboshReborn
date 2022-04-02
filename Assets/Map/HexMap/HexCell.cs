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
    public static class HexDirectionExtensions
    {
        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }
    }

    public class HexCell : MonoBehaviour
    {
        [SerializeField] int cellIndex;

        public enum CellTypes { water, terrain, rock, sand }
        private CellTypes cellType;
        public CellTypes CellType { get; set; }
        

        public List<Vector3> vertices;

        public int CellIndex { get; set; }

        [SerializeField] public HexCoords coords;

        /*
        //Цвета клеток для дегенерации
        [SerializeField] public Color desertColor = Color.yellow;
        [SerializeField] public Color terrainColor = Color.green;
        [SerializeField] public Color rockColor = Color.gray;
        [SerializeField] public Color waterColor = Color.blue;
        */


        //Тестовый цвет
        [SerializeField] public Color neighboorColor = Color.red;

        public EventHandler<HexCellEventArgs> MouseLeftClick;

        private Color cellColor;
        public Color CellColor
        {
            get { return cellColor; }
            set { cellColor = value; }
        }

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

        public HexCell() { MouseLeftClick += Choose; vertices = new List<Vector3>(); }

        public void Choose(object sender, HexCellEventArgs e)
        {
            cellColor = Color.cyan;
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
    }
}
