using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : IECSSystem
{ 
    public InputSystem(ECSService service) : base(service) { }

    private float LastClickTime = 0f;
    public const float ClickDelay = 0.3f;

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
        ECSFilter filter = new ECSFilter();
        List<InputHandler> components = filter.GetComponents<InputHandler>();

        if (Physics.Raycast(inputRay, out hit))
        {
            foreach (var c in components)
                if (hit.transform.gameObject == c.gameObject)
                {
                    Selectable sel_comp = c.gameObject.GetComponent<Selectable>();
                    if (sel_comp != null)
                        Service.GetSystem<SelectionSystem>().Select(sel_comp);
                }

        }
    }
}
