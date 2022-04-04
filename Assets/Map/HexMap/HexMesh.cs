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
		static Color SplatColor1 = new Color(1f, 0f, 0f);
		static Color SplatColor2 = new Color(0f, 1f, 0f);
		static Color SplatColor3 = new Color(0f, 0f, 1f);

		Mesh hexMesh;
		MeshCollider hexCollider;

		List<Vector3> vertices;
		List<int> triangles;
		List<Color> colors;

		List<Vector3> types; //Да-да, именно лист флоатских векторов, именно с ними и только с ними работает шейдер 

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

		public void Triangulate(CellList cells) 
		{
			hexMesh.Clear();
			vertices.Clear();
			triangles.Clear();
			colors.Clear();
			types.Clear();
			for (int i = 0; i < cells.Length; i++)
				Triangulate(cells[i], cells);
			hexMesh.vertices = vertices.ToArray();
			hexMesh.triangles = triangles.ToArray();
			hexMesh.colors = colors.ToArray();
			
			hexMesh.SetUVs(2, types); //Передача типов текстур шейдеру (тип текстуры == её индекс в массиве текстур)
			
			hexMesh.RecalculateNormals();

			hexCollider.sharedMesh = hexMesh;
			print($"Triangulate end...");
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
			//AddTriangleColor(cell.CellColor);
			AddTriangleColor(SplatColor1);

			Vector3 type1 = new Vector3((float)cell.CellType, (float)cell.CellType, (float)cell.CellType);
			AddTriangleType(type1);

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
			if(cell.GetDirection(neighbour) != -1)
            {
				if (cell.Bridges.Length == 0)
					cell.Bridges = new bool[6];
				cell.Bridges[cell.GetDirection(neighbour)] = true;
			}

			Color bridgeColor = (cell.CellColor + neighbour.CellColor) * 0.5f;
			//AddQuadColor(cell.CellColor, bridgeColor);
			Vector3 type2 = new Vector3(type1.x, type1.y, type1.z);
			if (neighbour.GetDirection(cell) != -1)
			{
				if (neighbour.Bridges.Length == 0 || !neighbour.Bridges[neighbour.GetDirection(cell)])
				{
					type2 = new Vector3((float)cell.CellType, (float)neighbour.CellType, (float)cell.CellType);
					AddQuadColor(SplatColor1, (SplatColor1 + SplatColor2) / 2.0f);
				}
				else
				{
					type2 = new Vector3((float)neighbour.CellType, (float)cell.CellType, (float)cell.CellType);
					AddQuadColor(SplatColor2, (SplatColor1 + SplatColor2) / 2.0f);
				}
			}
			else
				AddQuadColor(SplatColor1, SplatColor1);

			AddQuadType(type2);

            Vector3 v5 = center + HexMetrics.GetFirstCorner(direction);
			//Это я сам высчитал, вахуе что это сработало, ебать я математег
            v5.y = (cell.Elevation * HexMetrics.elevationStep + neighbour.Elevation * HexMetrics.elevationStep + prevNeighbour.Elevation * HexMetrics.elevationStep) / 3f;
			Vector3 v6 = center + HexMetrics.GetSecondCorner(direction);
			v6.y = (cell.Elevation * HexMetrics.elevationStep + neighbour.Elevation * HexMetrics.elevationStep + nextNeighbour.Elevation * HexMetrics.elevationStep) / 3f;

			AddTriangle(v1, v5, v3);
			/*AddTriangleColor(
				cell.CellColor,
				(cell.CellColor + prevNeighbour.CellColor + neighbour.CellColor) / 3f,
				bridgeColor
			);*/

			if(cell.Triangles.Length == 0)
            {
				cell.Triangles = new int[6];
            }
			if (neighbour.Triangles.Length == 0)
				neighbour.Triangles = new int[6];
			if (prevNeighbour.Triangles.Length == 0)
				prevNeighbour.Triangles = new int[6];
			if (nextNeighbour.Triangles.Length == 0)
				nextNeighbour.Triangles = new int[6];
			if (cell.Triangles[(int)direction] == 0)
            {
				AddTriangleColor(SplatColor1, (SplatColor1 + SplatColor2 + SplatColor3) / 3.0f, (SplatColor1 + SplatColor2) / 2.0f);
            }
			else if(cell.Triangles[(int)direction] == 1)
            {
				AddTriangleColor(SplatColor2, (SplatColor1 + SplatColor2 + SplatColor3) / 3.0f, (SplatColor2 + SplatColor3) / 2.0f);
			}
			else
            {
				AddTriangleColor(SplatColor3, (SplatColor1 + SplatColor2 + SplatColor3) / 3.0f, (SplatColor1 + SplatColor3) / 2.0f);
			}

			cell.Triangles[(int)direction] += 1;
			neighbour.Triangles[neighbour.GetDirection(cell)] += 1;
			prevNeighbour.Triangles[prevNeighbour.GetDirection(cell)] += 1;

			//AddTriangleColor(SplatColor1, (SplatColor1 + SplatColor2 + SplatColor3) / 3.0f, (SplatColor1 + SplatColor2) / 2.0f);

			var type3 = new Vector3((float)cell.CellType, (float)prevNeighbour.CellType, (float)neighbour.CellType);
			AddTriangleType(type3);

			AddTriangle(v2, v4, v6);
			/*AddTriangleColor(
				cell.CellColor,
				bridgeColor,
				(cell.CellColor + neighbour.CellColor + nextNeighbour.CellColor) / 3f
			);*/
			AddTriangleColor(SplatColor1, (SplatColor1 + SplatColor2) / 2.0f, (SplatColor1 + SplatColor2 + SplatColor3) / 3.0f);

			var type4 = new Vector3((float)cell.CellType, (float)neighbour.CellType, (float)nextNeighbour.CellType);
			AddTriangleType(type4);
		}

		void AddTriangleType(Vector3 type_vec)
        {
			types.Add(type_vec);
			types.Add(type_vec);
			types.Add(type_vec);
        }

		void AddQuadType(Vector3 type_vec)
        {
			types.Add(type_vec);
			types.Add(type_vec);
			types.Add(type_vec);
			types.Add(type_vec);
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

			types = new List<Vector3>();
		}
	}
}
