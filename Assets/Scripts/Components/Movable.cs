using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Movable : IECSComponent
{
    public Vector3 position;
    [SerializeField] public float movSpeed = 1.0f;
}
