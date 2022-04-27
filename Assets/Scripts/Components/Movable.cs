using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;

public class Movable : IECSComponent
{
    public float RotationSpeed;
    public float MoveSpeed;

    public List<Vector3> WayCells = null;
    public int MoveRadius;


    public float t;
    public Vector3 WalkedCell;

    public enum MoveType {swim, move, swimAndMove };
    public MoveType moveType;

}
