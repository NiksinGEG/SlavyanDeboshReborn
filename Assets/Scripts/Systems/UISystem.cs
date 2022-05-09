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
        var f = new ECSFilter();
        var cs = f.GetComponents<TextComponent>();
        foreach (var c in cs)
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
        var sys = Service.GetSystem(component.SystemType);
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
        var f = new ECSFilter();
        var hComps = f.GetComponents<Hideable>();
        foreach (var c in hComps)
            Handle(c);
        var btnComps = f.GetComponents<ButtonComponent>();
        foreach (var c in btnComps)
            Handle(c);
        var txtComps = f.GetComponents<TextComponent>();
        foreach (var c in txtComps)
            Handle(c);
    }
}
