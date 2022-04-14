using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Map.WorldMap
{
    public class HexGridChunk : MonoBehaviour
    {
        static Color color1 = new Color(1f, 0f, 0f);
        static Color color2 = new Color(0f, 1f, 0f);
        static Color color3 = new Color(0f, 0f, 1f);

        public CellList cells;
        public HexMesh hexMesh;
        int chunkIndex;
        public int ChunkIndex { get; set; }
        public void AddCell(int index, HexCell cell)
        {
            cells[index] = cell;
            cell.transform.SetParent(transform, false);
        }
        void Awake()
        {
            hexMesh = GetComponentInChildren<HexMesh>();
            cells = new CellList(new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ], HexMetrics.chunkSizeX, HexMetrics.chunkSizeZ);//new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        }

        void Start()
        {
            hexMesh.Triangulate(cells);
        }
}

}
