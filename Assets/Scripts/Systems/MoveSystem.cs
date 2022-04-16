using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Movable>();
        foreach(var _c in components)
        {
            Movable c = (Movable)_c;
            //Vector3 pos = c.gameObject.GetComponent<Transform>().position;
            if(c.isChoosen)
            {
                if(c.gameObject.GetComponent<Transform>().position != c.position)
                    c.gameObject.GetComponent<Transform>().position = Vector3.Lerp(c.gameObject.GetComponent<Transform>().position, c.position, 0.15f);

                //yield return null;
                c.isChoosen = !c.isChoosen;
            }      
        }
    }
}
