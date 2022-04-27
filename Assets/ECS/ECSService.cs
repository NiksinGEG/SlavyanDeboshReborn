using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Systems;
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
        AllComponents.AddRange(FindObjectsOfType<IECSComponent>());

        _systems = new List<IECSSystem>();
        
        /*��������� �����*/
        _systems.Add(new InputSystem(this));
        _systems.Add(new SelectionSystem(this));
        _systems.Add(new SpawnSystem(this));
        _systems.Add(new MoveSystem(this));
        _systems.Add(new AttackSystem(this));

        foreach (var s in _systems)
            s.Init();
    }

    void Update()
    {
        foreach (var s in _systems)
        {
            s.Run();
        }
            
    }
}
