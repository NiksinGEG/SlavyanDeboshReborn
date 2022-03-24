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
		public Color defaultColor = Color.white;
		public Color chosenColor = Color.cyan;

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
			hexMesh.Triangulate(cells, width);
		}

		void RedrawEvent(object sender, EventArgs e)
        {
			hexMesh.Triangulate(cells, width);
        }

		void CreateCell(int x, int z, int i)
		{
			Vector3 position = new Vector3(
				(x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f),
				0f,
				z * (HexMetrics.outerRadius * 1.5f));

			HexCell cell = cells[i] = Instantiate<HexCell>(cell_prefab);
			cell.transform.SetParent(transform, false);
			cell.transform.localPosition = position;
			cell.coords = HexCoords.FromOffset(x, z);
			cell.name = $"HexCell {cell.coords}";
			cell.color = defaultColor;

			cell.MouseLeftClick += RedrawEvent;

			if (i == 2)
				cell.Elevation = 1;
			if (i == 4)
				cell.Elevation = -2;
		}

		void Update()
		{
			if (Input.GetMouseButton(0))
			{
				HandleInput();
			}
		}

		void HandleInput()
		{
			Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(inputRay, out hit))
			{
				TouchCell(hit.point);
			}
		}

		void TouchCell(Vector3 position)
		{
			position = transform.InverseTransformPoint(position);
			/*print("touched at " + position);
			HexCoords coords = HexCoords.FromPosition(position);
			print("touched cell " + coords);
			int index = coords.x + coords.z * width + coords.z / 2;
			HexCell tmp = cells[index];
			tmp.color = chosenColor;
			hexMesh.Triangulate(cells);*/
			foreach (var cell in cells)
            {
				if(cell.coords.EqualsTo(HexCoords.FromPosition(position)))
                {
					print("touched at " + position);
					print("touched cell " + cell.coords);
					//cell.Choose();
					cell.MouseLeftClick.Invoke(cell, new HexCellEventArgs(position));
					//hexMesh.Triangulate(cells);
					break;
                }
			}
		}
	}
}
