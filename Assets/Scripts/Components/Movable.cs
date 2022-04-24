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

    public float RotationSpeed;
    public float MoveSpeed;
    
    public List<HexCell> WayCells = null;
    public int MoveRadius;


    public float t;
    public HexCell WalkedCell;

    public enum MoveType {swim, move, swimAndMove };
    public MoveType moveType;

}
