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
        //��� �������� ��� ���������� "SpawnComponent":
        ECSFilter<SpawnComponent> f = new ECSFilter<SpawnComponent>();
        //����� ����. ����� �������� � ����������� ������ ��� � SpawnComponent (������):
        foreach (var c in f)
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
