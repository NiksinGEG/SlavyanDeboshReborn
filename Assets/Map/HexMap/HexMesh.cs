﻿using System;
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

		public void Triangulate(CellList cells) //От параметра width надо будет избавиться
		{
			hexMesh.Clear();
			vertices.Clear();
			triangles.Clear();
			colors.Clear();
			for (int i = 0; i < cells.Length; i++)
			{
					Triangulate(cells[i]);
			}
			//TriangulateConnections(cells);
			hexMesh.vertices = vertices.ToArray();
			hexMesh.triangles = triangles.ToArray();
			hexMesh.colors = colors.ToArray();
			hexMesh.RecalculateNormals();

			hexCollider.sharedMesh = hexMesh;
			print($"Triangulate end... {triangles.Count};{hexMesh.triangles.Length}");
		}

		public void Triangulate(HexCell cell)
		{
			Vector3 center = cell.transform.localPosition;
			for (int i = 0; i < 6; i++)
			{
				AddTriangle(
					center,
					center + HexMetrics.corners[i],
					center + HexMetrics.corners[i + 1]
				);

				cell.vertices.Add(center);
				cell.vertices.Add(center + HexMetrics.corners[i]);
				cell.vertices.Add(center + HexMetrics.corners[i + 1]);

				AddTriangleColor(cell.CellColor);
			}
		}

		public void TriangulateConnections(CellList cells) 
		{
			for(int i = 0; i < cells.Length; i++)
            {
				foreach(var nei in cells.GetNeighbours(i))
                {
					/*int ind = nei.coords.MakeIndex(cells.CellCountX);
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
					AddTriangleColor(Color.yellow);*/
					var our_vertices = cells[i].vertices;
					var nei_vertices = nei.vertices;
					int dir = cells[i].GetDirection(nei);
					int antidir = nei.GetDirection(cells[i]);
					if (dir == -1 || antidir == -1)
						continue;

					var v1 = our_vertices[1 + (dir * 3)];
					var v2 = nei_vertices[1 + (antidir * 3)];
					var v3 = nei_vertices[2 + (antidir * 3)];
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
	}
}
