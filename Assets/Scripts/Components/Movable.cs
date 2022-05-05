using System;
using System.Globalization;
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
        s.SerializeField("moveType", (int)moveType);
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
            string[] split = s_point.Substring(1, s_point.Length - 2).Split(',');
            WayCells.Add(new Vector3(float.Parse(split[0], CultureInfo.InvariantCulture), float.Parse(split[1], CultureInfo.InvariantCulture), float.Parse(split[2], CultureInfo.InvariantCulture)));
        }
        MoveRadius = s.GetInt("MoveRadius");
        t = s.GetFloat("t");
        moveType = (MoveType)s.GetInt("moveType");
    }
}
