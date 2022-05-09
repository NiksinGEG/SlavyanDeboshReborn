using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSystem : IECSSystem
{
    public PopupSystem(ECSService s) : base(s) { }

    public override void Init()
    {
        Service.GetSystem<InputSystem>().MouseDownLKM += OnMouseLKM;
        Service.GetSystem<InputSystem>().MouseDownRKM += OnMouseRKM;
    }

    public void OnMouseLKM(RaycastHit hit)
    {
        var comps = new ECSFilter().GetComponents<PopupComponent>();
        foreach (var c in comps)
            c.gameObject.SetActive(false);
    }

    public void OnMouseRKM(RaycastHit hit)
    {
        var comps = new ECSFilter().GetComponents<PopupComponent>();
        foreach (var c in comps)
            ShowPopup(c, hit);
    }

    private void ShowPopup(PopupComponent c, RaycastHit hit)
    {
        c.gameObject.SetActive(true);
        Vector3 new_pos = Camera.main.transform.position + Camera.main.transform.forward * c.Distance;
        c.transform.position = Camera.main.transform.position;
        c.transform.LookAt(new_pos);
        c.transform.position = new_pos;
        
        //var cTrans = c.gameObject.GetComponent<RectTransform>();
        //cTrans.rot
    }
}
