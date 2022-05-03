using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// По сути просто юнитевский компонент. Создан для более удобного их "вычленения" <br/>
/// и отделения от обычных юнитевских компонентов.
/// </summary>
public class IECSComponent : MonoBehaviour
{
    private void Awake()
    {
        ECSInstance.Instance().Components.Add(this);
    }

    public void AddComponent<T>(T component) where T: IECSComponent
    {
        gameObject.AddComponent<T>();
        ECSInstance.Instance().Components.Add(component);
    }

    public void RemoveComponent<T>(T component) where T : IECSComponent
    {
        ECSInstance.Instance().Components.Remove(component);
        Destroy(component);
    }

    public virtual string Serialize()
    {
        return ToString();
    }

    public virtual void Set(string input) { }
}
