using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnComponent : IECSComponent
{
    [SerializeField] public Vector3 pos;
    [SerializeField] public GameObject obj;
    [SerializeField] public bool spawn = false;
    [SerializeField] public int spawnTime;
    void Spawn(Vector3 pos, GameObject obj)
    {
        this.pos = pos;
        this.obj = obj;
        this.spawnTime = 0;
        spawn = true;
    }

    public void _Spawn()
    {
        spawnTime = 0;
        spawn = true;
    }
}
