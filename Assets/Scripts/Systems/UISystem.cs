using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Assets.Scripts.Components.UI;
using UnityEngine.UI;

public class UISystem : IECSSystem
{
    public UISystem(ECSService s) : base(s) { }

    public override void Init()
    {
        base.Init();
    }

    public void SetText(string text, string name)
    {
        var f = new ECSFilter<TextComponent>();
        foreach (var c in f)
            if (c.Name == name)
                c.Text = text;
    }

    private void Handle(Hideable component)
    {
        if (component.IsHidden && component.gameObject.activeInHierarchy)
            component.gameObject.SetActive(false);
        else if (!component.IsHidden && !component.gameObject.activeInHierarchy)
            component.gameObject.SetActive(true);
    }

    private void Handle(ButtonComponent component)
    {
        if (!component.Clicked)
            return;
        var sys = Service.GetSystem(Type.GetType(component.SystemType));
        var method = sys.GetType().GetMethod(component.MethodName);
        method.Invoke(sys, new object[0]);
        component.Clicked = false;
    }

    private void Handle(TextComponent component)
    {
        component.gameObject.GetComponent<Text>().text = component.Text;
    }

    public override void Run()
    {
        var h_f = new ECSFilter<Hideable>();
        var b_f = new ECSFilter<ButtonComponent>();
        var t_f = new ECSFilter<TextComponent>();
        foreach (var c in h_f)
            Handle(c);
        foreach (var c in b_f)
            Handle(c);
        foreach (var c in t_f)
            Handle(c);
    }
}
