using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Selectable : IECSComponent
{
    public bool IsSelected { get; set; }
}
