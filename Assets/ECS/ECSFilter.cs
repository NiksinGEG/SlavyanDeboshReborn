using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSFilter
{
    public List<IECSComponent> Components;
    public ECSFilter(IECSComponent[] components) { Components = new List<IECSComponent>(components); }
    public ECSFilter(List<IECSComponent> components) { Components = components; }
    public ECSFilter() { Components = ECSInstance.Instance().Components; }
    
    public ECSFilter OfType<T>()
    {
        List<IECSComponent> new_list = new List<IECSComponent>();
        foreach(var c in Components)
            if(c.GetType() == typeof(T))
                new_list.Add(c);
        return new ECSFilter(new_list);
    }
    public ECSFilter WithoutType<T>()
    {
        List<IECSComponent> new_list = new List<IECSComponent>();
        foreach (var c in Components)
            if (c.GetType() != typeof(T))
                new_list.Add(c);
        return new ECSFilter(new_list);
    }

    public List<T> GetComponents<T>() where T: IECSComponent
    {
        List<T> new_list = new List<T>();
        foreach (var c in Components)
            if (c.GetType() == typeof(T))
                new_list.Add((T)c);
        return new_list;
    }
    public List<T> GetComponents<T>(Func<T, bool> predicate) where T: IECSComponent
    {
        List<T> new_list = new List<T>();
        foreach (var c in Components)
            if (c.GetType() == typeof(T))
                if(predicate.Invoke((T)c))
                    new_list.Add((T)c);
        return new_list;
    }
}
