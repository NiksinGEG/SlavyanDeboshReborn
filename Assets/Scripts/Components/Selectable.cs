using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Selectable : IECSComponent
{
    private bool _selected;
    public bool IsSelected
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            if (value)
                OnSelect.Invoke();
            else
                OnDeselect.Invoke();
        }
    }
    /// <summary>
    /// ����������� ����� IsSelected ������������� true
    /// </summary>
    public UnityEvent OnSelect;
    /// <summary>
    /// �����������, ����� IsSelected ������������� false
    /// </summary>
    public UnityEvent OnDeselect;

    /// <summary>
    /// ����������� ������ ���� ���� IsSelected == true
    /// </summary>
    public UnityEvent WhileSelected;
    /// <summary>
    /// ����������� ������ ���� ���� IsSelected == false
    /// </summary>
    public UnityEvent WhileDeselected;
}
