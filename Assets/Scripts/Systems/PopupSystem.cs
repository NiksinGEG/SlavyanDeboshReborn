using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSystem : IECSSystem
{
    public PopupSystem(ECSService s) : base(s) { }

    public override void Init()
    {
        Service.GetSystem<InputSystem>().MouseUpLKM += OnMouseLKM;
        Service.GetSystem<InputSystem>().MouseDownRKM += OnMouseRKM;

        ECSFilter<PopupComponent> f = new ECSFilter<PopupComponent>();
        foreach (var c in f)
            c.gameObject.SetActive(false);
    }

    public void OnMouseLKM(RaycastHit hit)
    {
        ECSFilter<PopupComponent> f = new ECSFilter<PopupComponent>();
        foreach (var c in f)
            c.gameObject.SetActive(false);
    }

    public void OnMouseRKM(RaycastHit hit)
    {
        ECSFilter<PopupComponent> f = new ECSFilter<PopupComponent>();
        foreach (var c in f)
        {
            ShowPopup(c);
            c.hit = hit;
            GlobalVariables.lastHit = hit;
        }
    }

    private void ShowPopup(PopupComponent c)
    {
        c.gameObject.SetActive(true);

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition); //получаем направление на курсор
        Vector3 new_pos = Camera.main.transform.position + inputRay.direction * c.Distance; //вычисляем новую позицию менюшки
        c.transform.position = Camera.main.transform.position; //ставим менюшку на камеру
        c.transform.LookAt(new_pos); //смотрим на новую позицию
        c.transform.position = new_pos; //и когда ставим меню на новую позицию, она будет смотреть прямо на нас!
    }
}
