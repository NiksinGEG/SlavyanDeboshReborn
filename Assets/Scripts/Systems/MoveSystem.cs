﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Scripts;

public class MoveSystem : IECSSystem
{
    public MoveSystem(ECSService s) : base(s) { }

    private Quaternion rotation;
    private bool isTurned = true;

    public override void Init()
    {
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Movable>();
        foreach (var _c in components)
        {
            Movable c = (Movable)_c;
            c.Position = c.gameObject.GetComponent<Transform>().position;
        }
    }

    public override void Run()
    {
        float eps = 0.15f;
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Movable>();

        foreach (var _c in components)
        {
            Movable c = (Movable)_c;
            if (c.WayCells == null || c.WayCells.Count == 0)
                return;
            if((Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - c.WayCells[0].transform.position.x) < eps &&
                Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - c.WayCells[0].transform.position.z) < eps) ||
                (c.t > 0.5 && c.WayCells.Count > 2) || (c.t >= 1)) //Путь не пустой, но объект на ближайшей точке пути
                {
                    c.WalkedCell = c.WayCells[0];
                    c.WayCells.Remove(c.WalkedCell);
                    c.t = 0;
                }
            else
            {
                if (c.WayCells.Count == 1 || c.WalkedCell == null)
                {

                    rotation = Quaternion.LookRotation(c.WayCells[0].transform.position);
                    //rotation.x = c.gameObject.transform.rotation.x;
                    if (Mathf.Abs(c.gameObject.transform.rotation.y - rotation.y) > 0 &&
                        Mathf.Abs(c.gameObject.transform.rotation.z - rotation.z) > 0)
                        isTurned = true;
                    else
                        isTurned = false;


                    if (isTurned)
                    {
                        c.gameObject.transform.position = Vector3.MoveTowards(c.gameObject.transform.position, c.WayCells[0].transform.position, c.MoveSpeed);
                    }
                    else
                    {
                        c.gameObject.transform.rotation = Quaternion.RotateTowards(c.gameObject.transform.rotation, rotation, 0.1f);
                    }
                }
                else
                {
                    Vector3 start = c.WalkedCell.transform.position;
                    Vector3 end = c.WayCells[1].transform.position;
                    Vector3 middle = c.WayCells[0].transform.position;

                    c.gameObject.transform.position = Bezier.GetPoint(start, middle, end, c.t);
                    c.t += c.MoveSpeed * 0.05f;
                }
            }
        }
    }
}
