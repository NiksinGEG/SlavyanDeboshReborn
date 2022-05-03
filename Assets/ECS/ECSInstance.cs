using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSInstance
{
    private static ECSInstance instance;
    public List<IECSComponent> Components;
    public List<IECSSystem> Systems;
    private ECSInstance()
    {
        Components = new List<IECSComponent>();
        Systems = new List<IECSSystem>();
    }
    public static ECSInstance Instance()
    {
        if(instance == null)
            instance = new ECSInstance();
        return instance;
    }
}
