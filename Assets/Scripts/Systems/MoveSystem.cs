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
    public MoveSystem(ECSService s) : base(s) { }

    private bool isTurned = true;
    private Quaternion fromRotation;
    private Quaternion toRotation;
    
    private Vector3 point;

    public override void Init()
    {

    }

    public override void Run()
    {
        float eps = 0.15f;
        ECSFilter f = new ECSFilter(Service);
        List<Movable> components = f.GetComponents<Movable>();
        foreach (var c in components)
        {
            var ms = c.MoveSpeed * Time.deltaTime;
            var rs = c.RotationSpeed * Time.deltaTime;
            if (c.WayCells == null || c.WayCells.Count == 0)
                return;
            if ((Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - c.WayCells[0].x) < eps &&
                Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - c.WayCells[0].z) < eps) ||
                (c.t > 0.5 && c.WayCells.Count > 2) || (c.t >= 1)) //Путь не пустой, но объект на ближайшей точке пути
                {
                    c.WalkedCell = c.WayCells[0];
                    if (c.t > 0.5 && c.WayCells.Count > 2)
                        c.WalkedCell = c.transform.position;
                    c.WayCells.RemoveAt(0);
                    c.t = 0;
                }
            else
            {
                if (c.WayCells.Count == 1 || c.WalkedCell == null)
                {
                    point = c.WayCells[0];
                    point.y = c.gameObject.transform.position.y;

                    fromRotation = c.gameObject.transform.rotation;
                    toRotation = Quaternion.LookRotation(point - c.gameObject.transform.position);

                    if (Quaternion.Angle(fromRotation, toRotation) < 0.00001f)
                        isTurned = true;
                    else
                        isTurned = false;

                    if (isTurned)
                        c.gameObject.transform.position = Vector3.MoveTowards(c.gameObject.transform.position, point, ms);
                    else
                        c.gameObject.transform.rotation = Quaternion.RotateTowards(fromRotation, toRotation, rs);
                }
                else
                {
                    Vector3 start = c.WalkedCell;
                    Vector3 end = c.WayCells[1];
                    Vector3 middle = c.WayCells[0];

                    point = Bezier.GetPoint(start, middle, end, c.t);
                    point.y = c.gameObject.transform.position.y;
                    fromRotation = c.gameObject.transform.rotation;
                    toRotation = Quaternion.LookRotation(point - c.gameObject.transform.position);
                    
                    if (toRotation.w == 1)
                        toRotation = fromRotation;

                    if (Quaternion.Angle(fromRotation, toRotation) < 20.0f && Quaternion.Angle(fromRotation, toRotation) > -1.0f )
                        isTurned = true;
                    else
                        isTurned = false;

                    if (isTurned)
                    {
                        c.gameObject.transform.rotation = toRotation;
                        c.gameObject.transform.position = point;
                        c.t += ms * 0.05f;
                    }
                    else
                        c.gameObject.transform.rotation = Quaternion.RotateTowards(fromRotation, toRotation, rs);
                }
            }
        }
    }
}
