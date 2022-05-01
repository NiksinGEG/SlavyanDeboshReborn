using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Systems.Net;

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

    private void Send(System.Type type, IECSComponent compToSend, NetComponent netComp)
    {
        Serializer s = new Serializer();
        s.SerializeField("Component", compToSend.Serialize());
        s.SerializeField("Id", netComp.Id);
        s.SerializeField("Type", type);
        string component = s.ToString();
        s = new Serializer();
        s.SerializeField("NetFrame", component);
        Debug.Log("Sending:\n");
        Debug.Log(s.ToString());
    }

    public override void Run()
    {
        ECSFilter f = new ECSFilter();
        var netComps = f.GetComponents<NetComponent>();
        foreach(var c in netComps)
        {
            if(c.NeedSend)
            {
                Send(c.ComponentToSend.GetType(), c.ComponentToSend, c);
                c.NeedSend = false;
            }
        }
    }
}
