using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;

public class SelectionSystem : IECSSystem
{
    public SelectionSystem(ECSService s) : base(s) { }

    private void SetCoords(Movable comp)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            comp.position = HexCoords.FromHitToCoords(hit);
        }
    }

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
                {
                    Movable mov_c = c.gameObject.GetComponent<Movable>();
                    if (mov_c != null)
                    {
                        SetCoords(mov_c);
                        mov_c.isChoosen = !mov_c.isChoosen;
                    }

                }
            }

            else
                c.WhileDeselected.Invoke();
            
        }
    }

    public override void Init()
    {
        
    }
}
