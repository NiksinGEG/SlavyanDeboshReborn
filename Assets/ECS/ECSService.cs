using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Сервис, агрегирующий системы, и предоставляющий им доступ к компонентам.<br/>
/// Все добавляемые системы должны быть добавлены здесь.
/// </summary>
public class ECSService : MonoBehaviour
{
    /// <summary>
    /// Все компоненты всех объектов на карте. Точнее, ссылки на них.
    /// </summary>
    public List<IECSComponent> AllComponents;
    private List<IECSSystem> _systems; 

    void Awake()
    {
        
    }

    void Start()
    {
        AllComponents = new List<IECSComponent>();
        AllComponents.AddRange(Resources.FindObjectsOfTypeAll<IECSComponent>());

        _systems = new List<IECSSystem>();
        
        /*ДОБАВЛЯТЬ ЗДЕСЬ*/
        _systems.Add(new InputSystem(this));
        _systems.Add(new SelectionSystem(this));
        _systems.Add(new SpawnSystem(this));

        foreach (var s in _systems)
            s.Init();
    }

    void Update()
    {
        foreach (var s in _systems)
            s.Run();
    }
}
