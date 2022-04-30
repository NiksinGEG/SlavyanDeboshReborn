using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Scripts.Systems.Net;

public class Movable : IECSComponent
{
    public float RotationSpeed;
    public float MoveSpeed;

    public List<Vector3> WayCells = null;
    public int MoveRadius;

    public float t;

    public enum MoveType { swim, move, swimAndMove };
    public MoveType moveType;

    public override string Serialize()
    {
        Serializer s = new Serializer();
        s.SerializeField("RotationSpeed", RotationSpeed);
        s.SerializeField("MoveSpeed", MoveSpeed);
        s.SerializeList("WayCells", WayCells);
        s.SerializeField("MoveRadius", MoveRadius);
        s.SerializeField("t", t);
        s.SerializeField("moveType", moveType);
        return s.ToString();
    }

    public override void Set(string input)
    {
        
    }
}
