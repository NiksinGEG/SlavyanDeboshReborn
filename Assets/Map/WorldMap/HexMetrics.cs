using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMetrics : MonoBehaviour
{
    public const float outerRadius = 10f;   //Внешний радиус шестиугольника
    public const float innerRadius = outerRadius * 0.866025404f;    //Внутренний радиус шестиугольника
	public const float elevationStep = 3f; //Шаг высоты клетки
	public const int chunkSizeX = 3, chunkSizeZ = 5;

	//Эта хуйня составная, поэтому забиндим положение фигур внутри
	//Возможные ориентации внутри шестиугольника
	public static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};


}
