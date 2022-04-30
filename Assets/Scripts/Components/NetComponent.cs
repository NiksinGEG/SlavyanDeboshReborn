using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetComponent : IECSComponent
{
    public int Id;
    public bool NeedSend;
    public IECSComponent ComponentToSend;
}
