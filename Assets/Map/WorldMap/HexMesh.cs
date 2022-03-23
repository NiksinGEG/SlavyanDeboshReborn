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

		public void Triangulate(HexCell[] cells)
		{
			hexMesh.Clear();
			vertices.Clear();
			triangles.Clear();
			for (int i = 0; i < cells.Length; i++)
			{
				Triangulate(cells[i]);
			}
			hexMesh.vertices = vertices.ToArray();
			hexMesh.triangles = triangles.ToArray();
			hexMesh.RecalculateNormals();

			hexCollider.sharedMesh = hexMesh;
		}

		void Triangulate(HexCell cell)
		{
			Vector3 center = cell.transform.localPosition;
			for (int i = 0; i < 6; i++)
				AddTriangle(
					center,
					center + HexMetrics.corners[i],
					center + HexMetrics.corners[i + 1]
				);
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

		void Awake()
		{
			GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
			hexCollider = gameObject.AddComponent<MeshCollider>();
			hexMesh.name = "Hex Mesh";
			vertices = new List<Vector3>();
			triangles = new List<int>();
		}
	}
}
