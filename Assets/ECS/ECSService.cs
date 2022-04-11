using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������, ������������ �������, � ��������������� �� ������ � �����������.<br/>
/// ��� ����������� ������� ������ ���� ��������� �����.
/// </summary>
public class ECSService : MonoBehaviour
{
    /// <summary>
    /// ��� ���������� ���� �������� �� �����. ������, ������ �� ���.
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
        
        /*��������� �����*/
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
