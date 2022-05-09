using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupComponent : IECSComponent
{
    public float Distance;
    public RaycastHit hit; //hit который привёл к открытию попапа
}
