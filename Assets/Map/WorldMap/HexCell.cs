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

    public class HexCell : MonoBehaviour
    {
        public HexCoords coords;
        public Color color;
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

        public HexCell() { MouseLeftClick += Choose; }

        public void Choose(object sender, HexCellEventArgs e)
        {
            color = Color.cyan;
        }
    }
}
