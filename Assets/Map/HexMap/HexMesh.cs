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

		public struct EdgeVertices
		{

			public Vector3 v1, v2, v3, v4;
			public EdgeVertices(Vector3 corner1, Vector3 corner2)
			{
				v1 = corner1;
				v2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
				v3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
				v4 = corner2;
			}
		}


		/*void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
		{
			AddTriangle(center, edge.v1, edge.v2);
			AddTriangleColor(color);
			AddTriangle(center, edge.v2, edge.v3);
			AddTriangleColor(color);
			AddTriangle(center, edge.v3, edge.v4);
			AddTriangleColor(color);
		}*/

		public void Triangulate(CellList cells) 
		{
			hexMesh.Clear();
			vertices.Clear();
			triangles.Clear();
			colors.Clear();
			for (int i = 0; i < cells.Length; i++)
			{
					Triangulate(cells[i], cells);
			}
			//TriangulateConnections(cells);
			hexMesh.vertices = vertices.ToArray();
			hexMesh.triangles = triangles.ToArray();
			hexMesh.colors = colors.ToArray();
			hexMesh.RecalculateNormals();

			hexCollider.sharedMesh = hexMesh;
			print($"Triangulate end... {triangles.Count};{hexMesh.triangles.Length}");
		}

		void Triangulate(HexCell cell, CellList cells)
		{
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				Triangulate(d, cell, cells);
			}
		}

		void Triangulate(HexDirection direction, HexCell cell, CellList cells)
		{
			Vector3 center = cell.transform.localPosition;
			Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
			Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

			AddTriangle(center, v1, v2);
			AddTriangleColor(cell.CellColor);

			HexCell neighbour = cell.GetNeighbour((int)direction) ?? cell;
			HexCell prevNeighbour = cell.GetNeighbour((int)direction - 1 < 0 ? (int)HexDirection.NW : (int)direction - 1) ?? cell;
			HexCell nextNeighbour = cell.GetNeighbour((int)direction + 1 > 5 ? (int)HexDirection.NE : (int)direction + 1) ?? cell;

			Vector3 bridge = HexMetrics.GetBridge(direction);
			Vector3 v3 = v1 + bridge;
			Vector3 v4 = v2 + bridge;

			/*Поднятие "мостов"*/
			v3.y -= (cell.Elevation * HexMetrics.elevationStep - neighbour.Elevation * HexMetrics.elevationStep) * 0.5f;
			v4.y -= (cell.Elevation * HexMetrics.elevationStep - neighbour.Elevation * HexMetrics.elevationStep) * 0.5f;

			AddQuad(v1, v2, v3, v4);

			Color bridgeColor = (cell.CellColor + neighbour.CellColor) * 0.5f;
			AddQuadColor(cell.CellColor, bridgeColor);

            Vector3 v5 = center + HexMetrics.GetFirstCorner(direction);
			//Это я сам высчитал, вахуе что это сработало, ебать я математег
            v5.y = (cell.Elevation * HexMetrics.elevationStep + neighbour.Elevation * HexMetrics.elevationStep + prevNeighbour.Elevation * HexMetrics.elevationStep) / 3f;
			Vector3 v6 = center + HexMetrics.GetSecondCorner(direction);
			v6.y = (cell.Elevation * HexMetrics.elevationStep + neighbour.Elevation * HexMetrics.elevationStep + nextNeighbour.Elevation * HexMetrics.elevationStep) / 3f;

			AddTriangle(v1, v5, v3);
			AddTriangleColor(
				cell.CellColor,
				(cell.CellColor + prevNeighbour.CellColor + neighbour.CellColor) / 3f,
				bridgeColor
			);
			AddTriangle(v2, v4, v6);
			AddTriangleColor(
				cell.CellColor,
				bridgeColor,
				(cell.CellColor + neighbour.CellColor + nextNeighbour.CellColor) / 3f
			);
		}

		void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
		{
			int vertexIndex = vertices.Count;
			vertices.Add(v1);
			vertices.Add(v2);
			vertices.Add(v3);
			vertices.Add(v4);
			triangles.Add(vertexIndex);
			triangles.Add(vertexIndex + 2);
			triangles.Add(vertexIndex + 1);
			triangles.Add(vertexIndex + 1);
			triangles.Add(vertexIndex + 2);
			triangles.Add(vertexIndex + 3);
		}

		void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
		{
			colors.Add(c1);
			colors.Add(c2);
			colors.Add(c3);
			colors.Add(c4);
		}
		void AddQuadColor(Color c1, Color c2)
		{
			colors.Add(c1);
			colors.Add(c1);
			colors.Add(c2);
			colors.Add(c2);
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
					//AddTriangleColor(Color.yellow);
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
		void AddTriangleColor(Color c1, Color c2, Color c3)
		{
			colors.Add(c1);
			colors.Add(c2);
			colors.Add(c3);
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
