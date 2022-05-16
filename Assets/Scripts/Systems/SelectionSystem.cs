using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Scripts.Statics;

public class SelectionSystem : IECSSystem
{
    public SelectionSystem(ECSService s) : base(s) { }

    public void OnMouseLKM(RaycastHit hit)
    {
        Selectable c = hit.transform.gameObject.GetComponent<Selectable>();
        if (c == null)
            return;
        Select(c);
    }

    public void OnMouseRKM()
    {
        ECSFilter<Selectable> f = new ECSFilter<Selectable>(c => c.IsSelected);
        if(f.Count > 0)
        {
            Movable mov_c = f.First().gameObject.GetComponent<Movable>();
            if (mov_c == null)
                return;
            Service.GetSystem<MoveSystem>().SetWay(mov_c, GlobalVariables.lastHit);
        }
    }

    public override void Init()
    {
        Service.GetSystem<InputSystem>().MouseDownLKM += OnMouseLKM;
    }

    public void Select(Selectable comp)
    {
        if (comp.IsSelected)
        {
            comp.IsSelected = false;
            return;
        }
        ECSFilter<Selectable> filter = new ECSFilter<Selectable>(s => s.IsSelected);
        foreach (var c in filter)
            c.IsSelected = false;
        comp.IsSelected = true;
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

    private int GetComponentTeam()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            var chunk = hit.transform.gameObject.GetComponentInParent<HexGridChunk>();
            if (chunk != null)
            {
                var grid = hit.transform.gameObject.GetComponentInParent<HexGridChunk>().gameObject.GetComponentInParent<HexGrid>();
                var unit = Map.GetByPosition(chunk.gameObject.transform.position);
            }
        }
        return 0;
    }

    public override void Run()
    {
        ECSFilter<Selectable> f = new ECSFilter<Selectable>();
        foreach(var c in f)
        {
            if (c.IsSelected)
                WhileSelected(c);
            else
                WhileDeselected(c);
        }
    }
}
