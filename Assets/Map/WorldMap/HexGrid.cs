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
			cell.coords = position;
			cell.name = $"X = {Math.Round(cell.coords.x)}, Y = {Math.Round(cell.coords.y)}, Z = {Math.Round(cell.coords.z)}";
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
			//Приведение координат хита к позиции гекса
			float off = position.z / (HexMetrics.outerRadius * 3f);
			int x = Mathf.RoundToInt(position.x / (HexMetrics.innerRadius * 2f) - off);
			int y = 0;
			int z = Mathf.RoundToInt(position.z / (HexMetrics.outerRadius * 1.5f));
			print("approximated coords: " + x + "; " + y + "; " + z);
			foreach(var cell in cells)
            {
				if(Mathf.RoundToInt(cell.coords.x) == x && 
					Mathf.RoundToInt(cell.coords.y) == y &&
					Mathf.RoundToInt(cell.coords.z) == z)
                {
					print("touched cell with coords: " + x + "; " + y + "; " + z);
                }
			}
			
			print("touched at " + position);
			Debug.Log("touched at " + position);
		}
	}
}
