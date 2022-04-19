using System;
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
            c.Position = c.gameObject.GetComponent<Transform>().position;
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
                if (c.WayCells != null)
                {
                    if (Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - c.WayCells[0].transform.position.x) > eps || 
                        Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - c.WayCells[0].transform.position.z) > eps)
                    {
                        c.gameObject.GetComponent<Transform>().position = Vector3.MoveTowards(c.gameObject.transform.position, c.WayCells[0].transform.position, c.MoveSpeed);
                    }
                    else
                        c.WayCells.Remove(c.WayCells[0]);
                }
            }
            catch
            {
                c.WayCells = null;
            }
        }
    }
}
