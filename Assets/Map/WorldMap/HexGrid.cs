using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Map.WorldMap
{
    public class HexGrid : MonoBehaviour
    {
        public int width = 6;
        public int height = 6;

        public HexCell cell_prefab;

		HexCell[] cells;
		HexMesh hexMesh;

		[ContextMenu("Generate game field")]
		void Awake()
		{
			hexMesh = GetComponentInChildren<HexMesh>();
			cells = new HexCell[height * width];
			for (int z = 0, i = 0; z < height; z++)
			{
				for (int x = 0; x < width; x++)
				{
					CreateCell(x, z, i++);
				}
			}
		}

		void Start()
		{
			hexMesh.Triangulate(cells);
		}

		void CreateCell(int x, int z, int i)
		{
			Vector3 position;
			position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
			position.y = 0f;
			position.z = z * (HexMetrics.outerRadius * 1.5f);

			HexCell cell = cells[i] = Instantiate<HexCell>(cell_prefab);
			cell.transform.SetParent(transform, false);
			cell.transform.localPosition = position;
		}
	}
}
