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
        Serializer s = new Serializer(input);

        RotationSpeed = s.GetFloat("RotationSpeed");
        MoveSpeed = s.GetFloat("MoveSpeed");
        string[] s_way = s.GetArray("WayCells");
        foreach(var s_point in s_way)
        {
            string s_p = s_point;
            s_p = s_p.Substring(1, s_p.Length - 2);
            string[] split = s_p.Split(',');
            WayCells.Add(new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2])));
        }
        MoveRadius = s.GetInt("MoveRadius");
        t = s.GetFloat("t");
        moveType = (MoveType)s.GetInt("moveType");
    }
}
