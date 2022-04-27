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
        List<SpawnComponent> components = f.GetComponents<SpawnComponent>();
        //Конец мема. Чтобы работать с компонентом именно как с SpawnComponent (пример):
        foreach (var c in components)
        {
            if(c.spawn)
            {
                MonoBehaviour.Instantiate(c.gameObject);
                c.obj.transform.localPosition = c.pos;
                c.spawn = false;
            }

        }
    }
}
