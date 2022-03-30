using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Map.WorldMap
{
    public class HexGridChunk : MonoBehaviour
    {
        public CellList cells;
        HexMesh hexMesh;
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

    // Update is called once per frame
        void Start()
        {
            hexMesh.Triangulate(cells);
        }
}

}
