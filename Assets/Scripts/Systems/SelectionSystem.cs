using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSystem : IECSSystem
{
    public SelectionSystem(ECSService s) : base(s) { }

    public override void Run()
    {
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Selectable>();
        foreach(var _c in components)
        {
            Selectable c = (Selectable)_c;
            //Debug.Log(c.name);
            if (c.IsSelected)
            {
                c.WhileSelected.Invoke();
                if (Input.GetMouseButton(0))
                    c.MouseLKM.Invoke();
                if (Input.GetMouseButton(1))
                    c.MouseRKM.Invoke();
            }

            else
                c.WhileDeselected.Invoke();
            
        }
    }

    public override void Init()
    {
        
    }
}
