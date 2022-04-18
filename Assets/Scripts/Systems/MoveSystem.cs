﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;

public class MoveSystem : IECSSystem
{
    public MoveSystem(ECSService s) : base(s) { }

    public override void Init()
    {
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Movable>();
        foreach (var _c in components)
        {
            Movable c = (Movable)_c;
            c.position = c.gameObject.GetComponent<Transform>().position;
        }
    }

    public override void Run()
    {
        float eps = 0.15f;
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Movable>();
        foreach(var _c in components)
        {
            Movable c = (Movable)_c;
            try
            {
                if (c.travel != null)
                {
                    if (Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - c.travel[0].transform.position.x) > eps || 
                        Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - c.travel[0].transform.position.z) > eps)
                    {
                        c.gameObject.GetComponent<Transform>().position = Vector3.MoveTowards(c.gameObject.transform.position, c.travel[0].transform.position, 0.25f)/*Lerp(c.gameObject.transform.position, c.travel[0].transform.position, c.movSpeed * Time.deltaTime)*/;
                    }
                    else
                        c.travel.Remove(c.travel[0]);
                }
            }
            catch
            {
                c.travel = null;
            }
        }
    }
}
