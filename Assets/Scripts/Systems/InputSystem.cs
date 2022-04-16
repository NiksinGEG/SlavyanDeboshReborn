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
        comp.IsSelected = !comp.IsSelected;
    }

    public override void Run()
    {
        try
        {
            if (Input.GetMouseButton(0)
            && Time.realtimeSinceStartup - LastClickTime > ClickDelay)
            {
                LastClickTime = Time.realtimeSinceStartup;
                Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                ECSFilter filter = new ECSFilter(Service);
                List<IECSComponent> components = filter.GetComponents<InputHandler>();

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
        catch
        {
            Debug.Log("GOVNO");
        }
        
    }
}
