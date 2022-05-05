using System;
using System.Threading.Tasks;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Systems.Net;

public class NetSystem : IECSSystem
{
    private List<string> recievedStrings;
    public NetSystem(ECSService s) : base(s) { recievedStrings = new List<string>(); }

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

    private void Send(Type type, IECSComponent compToSend, NetComponent netComp)
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
        byte[] query = Encoding.UTF8.GetBytes(s.ToString());
        GlobalVariables.NetStream.Write(query, 0, query.Length);
    }

    private async void RecieveAsync()
    {
        await Task.Run(() =>
        {
            while(true)
            {
                if (GlobalVariables.NetStream == null)
                    continue;
                string s = "";
                byte[] buf = new byte[1];
                GlobalVariables.NetStream.Read(buf, 0, 1);
                char c = Encoding.UTF8.GetString(buf)[0];
                if (c != '{')
                    return;
                s += c;
                int pars = 1;
                while (pars > 0)
                {
                    GlobalVariables.NetStream.Read(buf, 0, 1);
                    c = Encoding.UTF8.GetString(buf)[0];
                    if (c == '{')
                        pars++;
                    if (c == '}')
                        pars--;
                    s += c;
                }
                Debug.Log("Net system recieved:\n" + s);
                recievedStrings.Add(s);
            }
        });
    }

    public override void Init()
    {
        RecieveAsync();
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

        if (recievedStrings.Count == 0)
            return;
        foreach(var resp in recievedStrings)
        {
            Serializer s = new Serializer(resp);
            string comp = s.GetValue("Component");
            int id = s.GetInt("Id");
            var netComp = f.GetComponents<NetComponent>(c => c.Id == id)[0];
            netComp.gameObject.GetComponent<Movable>().Set(comp);
        }
        recievedStrings.Clear();
    }
}
