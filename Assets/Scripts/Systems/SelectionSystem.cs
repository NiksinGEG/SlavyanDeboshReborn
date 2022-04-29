using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Scripts;
using Assets.Scripts.Components;

public class SelectionSystem : IECSSystem
{
    private float LastClickTime = 0f;
    public const float ClickDelay = 0.3f;

    public SelectionSystem(ECSService s) : base(s) { }

    public void Select(Selectable comp)
    {
        if (comp.IsSelected)
        {
            comp.IsSelected = false;
            return;
        }
        ECSFilter filter = new ECSFilter();
        List<Selectable> selectable_comps = filter.GetComponents<Selectable>(s => s.IsSelected);
        foreach (var c in selectable_comps)
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

    /// <summary>
    /// �������� ���� ����� ��� �������� ����� ���� (��������� ��������)<br/>
    /// ����� ����� �������� LastClickTime!
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
                var unit = grid.GetByPosition(chunk.gameObject.transform.position);
            }
        }
        return 0;
    }

    private void HandleSelectedRKM(Selectable component)
    {
        if (!InputTimeCheck())
            return;
        Movable mov_c = component.gameObject.GetComponent<Movable>();
        if (mov_c != null)
        {
            Service.GetSystem<MoveSystem>().SetWay(mov_c);
        }

        Attack attack_c = component.gameObject.GetComponent<Attack>();
        if (attack_c != null)
        {
            GetComponentTeam();
            Debug.Log("GetComponentTeam");
            Debug.Log("Attack != null");
        }
    }

    public override void Run()
    {
        ECSFilter f = new ECSFilter();
        List<Selectable> components = f.GetComponents<Selectable>();
        foreach(var c in components)
        {
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
