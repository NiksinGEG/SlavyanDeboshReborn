using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;

public class SelectionSystem : IECSSystem
{
    private float LastClickTime = 0f;
    public const float ClickDelay = 0.3f;

    public SelectionSystem(ECSService s) : base(s) { }

    private void SetCoords(Movable comp)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            if(hit.transform.gameObject.GetComponentInParent<HexGridChunk>() != null) //if clicked at cell of a map
                comp.position = HexCoords.FromHitToCoords(hit);
        }
    }

    private void WhileSelected(Selectable component)
    {
        try
        {
            Material mat = component.gameObject.GetComponent<Renderer>().material;
            float o_width = mat.GetFloat("_Outline");
            if (o_width < 0.3f)
                o_width += 0.05f;
            mat.SetFloat("_Outline", o_width);
        }
        catch {
            Debug.Log($"Error via object {component.gameObject.name}");
        }
        
    }

    private void WhileDeselected(Selectable component)
    {
        try
        {
            Material mat = component.gameObject.GetComponent<Renderer>().material;
            float o_width = mat.GetFloat("_Outline");
            if (o_width > 0f)
                o_width -= 0.05f;
            mat.SetFloat("_Outline", o_width);
        }
        catch { 
            Debug.Log($"Error via object {component.gameObject.name}"); 
        }
    }

    /// <summary>
    /// Вызывать этот метод для проверки ввода мыши (учитывает задержку)<br/>
    /// Также может изменять LastClickTime!
    /// </summary>
    /// <returns></returns>
    private bool InputTimeCheck()
    {
        bool res = Time.realtimeSinceStartup - LastClickTime > ClickDelay;
        if(res)
            LastClickTime = Time.realtimeSinceStartup;
        return res;
    }

    private void HandleSelectedLKM(Selectable component)
    {
        if (!InputTimeCheck())
            return;
    }

    private void HandleSelectedRKM(Selectable component)
    {
        if (!InputTimeCheck())
            return;
        Movable mov_c = component.gameObject.GetComponent<Movable>();
        if (mov_c != null)
        {
            SetCoords(mov_c);
            mov_c.isSelected = !mov_c.isSelected;
        }
    }

    public override void Run()
    {
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Selectable>();
        foreach(var _c in components)
        {
            Selectable c = (Selectable)_c;
            if (c.IsSelected)
            {
                WhileSelected(c);
                if (Input.GetMouseButton(0))
                    HandleSelectedLKM(c);
                if (Input.GetMouseButton(1))
                    HandleSelectedRKM(c);
            }
            else
                WhileDeselected(c);
        }
    }
}
