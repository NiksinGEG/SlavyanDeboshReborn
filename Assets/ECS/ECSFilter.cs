using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSFilter
{
    public List<IECSComponent> Components;
    public ECSFilter(IECSComponent[] components) { Components = new List<IECSComponent>(components); }
    public ECSFilter(List<IECSComponent> components) { Components = components; }
    public ECSFilter(ECSService service) { Components = service.AllComponents; }
    
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

    public List<IECSComponent> GetComponents<T>()
    {
        List<IECSComponent> new_list = new List<IECSComponent>();
        foreach (var c in Components)
            if (c.GetType() == typeof(T))
                new_list.Add(c);
        return new_list;
    }
}