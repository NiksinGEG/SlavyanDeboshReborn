using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : IECSSystem
{
    public SpawnSystem(ECSService s) : base(s) { }

    public override void Init()
    {
        
    }
    
    public override void Run()
    {
        //Как получить все компоненты "SpawnComponent":
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<SpawnComponent>();
        //Конец мема. Чтобы работать с компонентом именно как с SpawnComponent (пример):
        foreach (var c in components)
        {
            SpawnComponent aboba = (SpawnComponent)c;
            if(aboba.spawn)
            {
                MonoBehaviour.Instantiate(aboba.gameObject);
                aboba.obj.transform.localPosition = aboba.pos;
                aboba.spawn = false;
            }

        }
    }
}
