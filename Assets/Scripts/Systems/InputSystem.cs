using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : IECSSystem
{ 
    public InputSystem(ECSService service) : base(service) { }

    private float LastClickTime = 0f;
    public const float ClickDelay = 0.3f;

    private void Select(Selectable comp)
    {
        if(comp.IsSelected)
        {
            comp.IsSelected = false;
            return;
        }
        ECSFilter filter = new ECSFilter(Service);
        List<Selectable> selectable_comps = filter.GetComponents<Selectable>(s => s.IsSelected);
        foreach (var c in selectable_comps)
            c.IsSelected = false;
        comp.IsSelected = true;
        //comp.IsSelected = !comp.IsSelected;
    }

    public override void Run()
    {
        if (Input.GetMouseButton(0))
            HandleMouseLKM();
    }

    private bool InputTimeCheck()
    {
        bool res = Time.realtimeSinceStartup - LastClickTime > ClickDelay;
        if (res)
            LastClickTime = Time.realtimeSinceStartup;
        return res;
    }

    private void HandleMouseLKM()
    {
        if (!InputTimeCheck())
            return;
        LastClickTime = Time.realtimeSinceStartup;
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        ECSFilter filter = new ECSFilter(Service);
        List<InputHandler> components = filter.GetComponents<InputHandler>();

        if (Physics.Raycast(inputRay, out hit))
        {
            foreach (var c in components)
                if (hit.transform.gameObject == c.gameObject)
                {
                    Selectable sel_comp = c.gameObject.GetComponent<Selectable>();
                    if (sel_comp != null)
                        Select(sel_comp);
                }

        }
    }
}
