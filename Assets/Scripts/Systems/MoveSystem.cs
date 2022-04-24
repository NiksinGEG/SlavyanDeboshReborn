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
    private float angle;

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
                    point = c.WayCells[0].transform.position;
                    point.y = c.gameObject.transform.position.y;
                    fromRotation = c.gameObject.transform.rotation;
                    toRotation = Quaternion.LookRotation(point - c.gameObject.transform.position);

                    if ((Mathf.Abs(Mathf.Abs(toRotation.x) - Mathf.Abs(fromRotation.x)) < 0.00351f) &&
                        (Mathf.Abs(Mathf.Abs(toRotation.y) - Mathf.Abs(fromRotation.y)) < 0.00351f) &&
                        (Mathf.Abs(Mathf.Abs(toRotation.z) - Mathf.Abs(fromRotation.z)) < 0.00351f) &&
                        (Mathf.Abs(Mathf.Abs(toRotation.w) - Mathf.Abs(fromRotation.w)) < 0.00351f))
                        isTurned = true;
                    else
                        isTurned = false;

                    if (isTurned)
                    {
                        c.gameObject.transform.position = Vector3.MoveTowards(c.gameObject.transform.position, c.WayCells[0].transform.position, c.MoveSpeed);
                    }
                    else
                    {
                        angle = Quaternion.Angle(fromRotation, toRotation);
                        if(angle > 0f)
                            c.gameObject.transform.rotation = Quaternion.RotateTowards(fromRotation, toRotation, c.RotationSpeed);
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
