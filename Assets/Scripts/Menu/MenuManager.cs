using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] public Menu[] Menus;

    public Menu ActiveMenu { get; private set; }

    public Menu GetMenu(string name)
    {
        foreach (var m in Menus)
            if (m.Name == name)
                return m;
        return null;
    }

    public void ShowMenu(string name)
    {
        foreach (var m in Menus)
        {
            if (m.Name == name)
                ActiveMenu = m;
            m.gameObject.SetActive(m.Name == name);
        }
    }

}
