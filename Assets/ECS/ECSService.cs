using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Systems;
using UnityEngine;

/// <summary>
/// —ервис, предоставл€ющий доступ к системам.<br/>
/// ¬се системы должны быть добавлены здесь.
/// </summary>
public class ECSService : MonoBehaviour
{
    void InitSystems()
    {
        var Systems = ECSInstance.Instance().Systems;

        /*ƒќЅј¬Ћя“№ «ƒ≈—№*/
        Systems.Add(new InputSystem(this));
        Systems.Add(new SelectionSystem(this));
        Systems.Add(new SpawnSystem(this));
        Systems.Add(new MoveSystem(this));
        Systems.Add(new AttackSystem(this));
        Systems.Add(new NetSystem(this));
    }



    void Awake()
    {
        InitSystems();
    }

    void Start()
    {
        foreach (var s in ECSInstance.Instance().Systems)
            s.Init();
    }

    void Update()
    {
        foreach (var s in ECSInstance.Instance().Systems)
            s.Run();
    }

    public T GetSystem<T>() where T: IECSSystem
    {
        foreach(var s in ECSInstance.Instance().Systems)
            if (s.GetType() == typeof(T))
                return (T)s;
        return null;
    }
}
