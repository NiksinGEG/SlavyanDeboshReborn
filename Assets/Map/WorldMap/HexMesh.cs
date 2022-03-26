using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Map.WorldMap
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
		Mesh hexMesh;
		MeshCollider hexCollider;

		List<Vector3> vertices;
		List<int> triangles;
		List<Color> colors;

		public void Triangulate(HexCell[] cells, int width) //От параметра width надо будет избавиться
		{
			print("Triangulate start...");
			hexMesh.Clear();
			vertices.Clear();
			triangles.Clear();
			colors.Clear();
			for (int i = 0; i < cells.Length; i++)
			{
				Triangulate(cells[i]);
			}
			TriangulateConnections(cells, width);
			hexMesh.vertices = vertices.ToArray();
			hexMesh.triangles = triangles.ToArray();
			hexMesh.colors = colors.ToArray();
			hexMesh.RecalculateNormals();

			hexCollider.sharedMesh = hexMesh;
			print("Triangulate end...");
		}

		void Triangulate(HexCell cell)
		{
			Vector3 center = cell.transform.localPosition;
			for (int i = 0; i < 6; i++)
			{
				AddTriangle(
					center,
					center + HexMetrics.corners[i],
					center + HexMetrics.corners[i + 1]
				);
				AddTriangleColor(cell.CellColor);
			}
		}

		void TriangulateConnections(HexCell[] cells, int width) //От параметра width надо будет избавиться
		{
			for(int i = 0; i < cells.Length; i++)
            {
				foreach(var nei in GetNeighbours(cells, i, width))
                {
					int ind = IndexFromHexCoords(nei.coords.x, nei.coords.z, width);
					int our_vertind = i * 18;
					int nei_vertind = ind * 18;
					int dir = cells[i].GetDirection(nei);
					if (dir == -1)
						continue;
					Vector3 v1 = vertices[1 + (dir * 3) + our_vertind];
					int antidir = nei.GetDirection(cells[i]);
					Vector3 v2 = vertices[1 + (antidir * 3) + nei_vertind];
					Vector3 v3 = vertices[2 + (antidir * 3) + nei_vertind];
					AddTriangle(v1, v2, v3);
					AddTriangleColor(Color.yellow);
                }
            }
        }

		void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			int vertexIndex = vertices.Count;
			vertices.Add(v1);
			vertices.Add(v2);
			vertices.Add(v3);
			triangles.Add(vertexIndex);
			triangles.Add(vertexIndex + 1);
			triangles.Add(vertexIndex + 2);
		}
		void AddTriangleColor(Color color)
        {
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
        }

		void Awake()
		{
			GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
			hexCollider = gameObject.AddComponent<MeshCollider>();
			hexMesh.name = "Hex Mesh";
			vertices = new List<Vector3>();
			triangles = new List<int>();
			colors = new List<Color>();
		}

		public List<HexCell> GetNeighbours(HexCell[] cells, int index, int width) //От параметра width надо будет избавиться
        {
			List<HexCell> neighbours = new List<HexCell>();
			HexCell cur_cell = cells[index];
			HexCoords cur_coords = cur_cell.coords;
			for(int i = 0; i < 6; i++)
            {
				HexCoords nei_coords = new HexCoords(-1, -1);
				switch(i)
                {
					case 0:
						nei_coords = new HexCoords(cur_coords.x, cur_coords.z + 1);
						break;
					case 1:
						nei_coords = new HexCoords(cur_coords.x + 1, cur_coords.z);
						break;
					case 2:
						nei_coords = new HexCoords(cur_coords.x + 1, cur_coords.z - 1);
						break;
					case 3:
						nei_coords = new HexCoords(cur_coords.x, cur_coords.z - 1);
						break;
					case 4:
						nei_coords = new HexCoords(cur_coords.x - 1, cur_coords.z);
						break;
					case 5:
						nei_coords = new HexCoords(cur_coords.x - 1, cur_coords.z + 1);
						break;

				}
				int nei_index = IndexFromHexCoords(nei_coords.x, nei_coords.z, width);
				if (nei_index >= 0 && nei_index < cells.Length)
					neighbours.Add(cells[nei_index]);
            }
			return neighbours;
		}

		int IndexFromHexCoords(int x, int z, int mapWidth)
        {
			return x + z * mapWidth + z / 2;
        }
	}
}
