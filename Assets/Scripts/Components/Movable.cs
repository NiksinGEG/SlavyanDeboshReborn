using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;

public class Movable : IECSComponent
{
    public Vector3 Position;
    public float MoveSpeed = 0.000000001f;
    public List<HexCell> WayCells = null;
}
