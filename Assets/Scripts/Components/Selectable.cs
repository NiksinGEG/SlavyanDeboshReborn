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
    /// Выполняется когда IsSelected присваивается true
    /// </summary>
    public UnityEvent OnSelect;
    /// <summary>
    /// Выполняется, когда IsSelected присваивается false
    /// </summary>
    public UnityEvent OnDeselect;

    /// <summary>
    /// Выполняется каждый кадр пока IsSelected == true
    /// </summary>
    public UnityEvent WhileSelected;
    /// <summary>
    /// Выполняется каждый кадр пока IsSelected == false
    /// </summary>
    public UnityEvent WhileDeselected;
}
