using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;

public class Movable : IECSComponent
{
    public Vector3 position;
    [SerializeField] public float movSpeed = 0.000000001f;
    public List<HexCell> travel = null;
}
