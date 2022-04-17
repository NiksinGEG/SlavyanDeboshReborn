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

    int i = 1;
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

    private void Travel(Movable c)
    {
        float eps = 0.5f;
        foreach(var cell in c.travel)
            //while (Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - cell.transform.position.x) > eps || Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - cell.transform.position.z) > eps)
                c.gameObject.GetComponent<Transform>().position = (cell.transform.position - c.gameObject.GetComponent<Transform>().position) * c.movSpeed;
            //if(c.gameObject.GetComponent<Transform>().position != cell.transform.position)                       
            //while (Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - cell.transform.position.x) > eps || Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - cell.transform.position.z) > eps)

    }

    public override void Run()
    {
        float eps = 0.15f;
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Movable>();
        foreach(var _c in components)
        {
            Movable c = (Movable)_c;
            if (c.travel != null)
            {
                if(i < c.travel.Count)
                    c.gameObject.GetComponent<Transform>().position = Vector3.Lerp(c.gameObject.GetComponent<Transform>().position, c.travel[i].transform.position, Time.deltaTime * c.movSpeed);
                if(Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - c.travel[i].transform.position.x) > eps || Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - c.travel[i].transform.position.z) > eps)
                    i++;
                if(i == c.travel.Count)
                {
                    i = 1;
                    c.travel = null;
                }
            }
        }
    }
}
