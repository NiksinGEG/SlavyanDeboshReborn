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
        var comps = new ECSFilter().GetComponents<PopupComponent>();
        foreach (var c in comps)
            c.gameObject.SetActive(false);
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

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition); //получаем направление на курсор
        Vector3 new_pos = Camera.main.transform.position + inputRay.direction * c.Distance; //вычисляем новую позицию менюшки
        c.transform.position = Camera.main.transform.position; //ставим менюшку на камеру
        c.transform.LookAt(new_pos); //смотрим на новую позицию
        c.transform.position = new_pos; //и когда ставим меню на новую позицию, она будет смотреть прямо на нас!
    }
}
