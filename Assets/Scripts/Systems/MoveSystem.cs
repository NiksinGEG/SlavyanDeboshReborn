using System;
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
    private float t = 0;
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

        foreach (var _c in components)
      {
            Movable c = (Movable)_c;
            try
            {
                if (c.WayCells != null || c.WayCells.Count != 0)
                {
                    Vector3 start, finish, middle = c.gameObject.GetComponent<Transform>().position;
                    if (Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - c.WayCells[0].transform.position.x) > eps ||
                        Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - c.WayCells[0].transform.position.z) > eps)
                    {
                        start = middle;
                        finish = c.WayCells[0].transform.position;
                        middle = (finish + c.WayCells[1].transform.position) * 0.5f;
                        t += 0.1f;
                        Debug.Log($"Step {t}. Bezie position: {Bezier.GetPoint(start, middle, finish, t)}");
                        c.gameObject.GetComponent<Transform>().position = Vector3.MoveTowards(c.gameObject.transform.position, Bezier.GetPoint(start, middle, finish, t), c.MoveSpeed);
                    }
                    else
                    {
                        c.WayCells.Remove(c.WayCells[0]);
                        t = 0;
                    }

                }
            }
            catch
            {
                c.WayCells = null;
            }
        }
    }
}
