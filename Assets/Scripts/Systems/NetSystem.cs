using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSystem : IECSSystem
{
    public NetSystem(ECSService s) : base(s) { }

    public string Serialize(System.Type type, object obj)
    {
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine("{Type:" + type.ToString() + "}");
        
        var fields = type.GetFields();
        var props = type.GetProperties();

        sb.AppendLine("{Fields:[");
        foreach (var field in fields)
            sb.Append("{" + field.Name + ":" + field.GetValue(obj).ToString() + "}");
        sb.Append("]}");

        sb.AppendLine("{Props:[");
        foreach (var prop in props)
            sb.Append("{" + prop.Name + ":" + prop.GetValue(obj).ToString() + "}");
        sb.Append("]}");

        return sb.ToString();
    }

    public void Send(System.Type type, IECSComponent compToSend, NetComponent netComp)
    {

    }

    public override void Run()
    {
        ECSFilter f = new ECSFilter();
        var netComps = f.GetComponents<NetComponent>();
        foreach(var c in netComps)
        {
            if(c.NeedSend)
            {
                Debug.Log(c.ComponentToSend.Serialize());
                c.NeedSend = false;
                //c.ComponentToSend.
            }
        }
    }
}
