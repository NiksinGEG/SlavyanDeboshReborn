using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Net
{
    public class Serializer
    {
        StringBuilder _sb;
        Dictionary<string, string> keyValues;

        public Serializer() 
        { 
            _sb = new StringBuilder(); 
            keyValues = new Dictionary<string, string>();
        }
        public Serializer(string input) 
        { 
            _sb = new StringBuilder(input);
            keyValues = Parse(input);
        }

        public void SerializeField<field_T>(string fieldName, field_T fieldVal)
        {
            _sb.AppendLine("{" + fieldName + ":" + fieldVal.ToString() + "}");


            keyValues.Add(fieldName, fieldVal.ToString());
        }
        public void SerializeArray<base_T>(string arrayName, base_T[] arr)
        {
            string s_arr = "[";
            foreach (var item in arr)
                s_arr += item.ToString() + ",";
            if (s_arr[s_arr.Length - 1] == ',')
                s_arr.Remove(s_arr.Length - 1);
            s_arr += "]";

            _sb.AppendLine("{" + arrayName + ":" + s_arr + "}");

            keyValues.Add(arrayName, s_arr);
        }
        public void SerializeList<base_T>(string listName, List<base_T> list)
        {
            /*_sb.Append("{" + listName + ":[");
            foreach (var elem in list)
                _sb.Append(elem.ToString() + ",");
            if (_sb[_sb.Length - 1] == ',')
                _sb.Remove(_sb.Length - 1, 1);
            _sb.Append("]}");
            _sb.AppendLine();*/

            SerializeArray(listName, list.ToArray());
        }
        public override string ToString()
        {
            return _sb.ToString();
        }

        private List<string> DivideByObjects(string s)
        {
            List<string> vals = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                int par = 0;
                if (s[i] == '{')
                {
                    par++;
                    string new_s = "{";
                    for (int j = i + 1; j < s.Length && par > 0; j++)
                    {
                        if (s[j] == '{')
                            par++;
                        if (s[j] == '}')
                            par--;
                        new_s += s[j];
                    }
                    if (par > 0)
                        throw new Exception("Closing par expected!");
                    vals.Add(new_s);
                }
            }
            return vals;
        }

        private Dictionary<string, string> Parse(string s)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            List<string> objects = DivideByObjects(s);
            foreach(string obj in objects)
            {
                string new_s = obj;
                try
                {
                    //1. "{val:23}" => "val:23"
                    new_s = new_s.Substring(1, new_s.Length - 2);

                    //2. "val:23" => ["val", "23"]
                    string[] split = new_s.Split(':');

                    res.Add(split[0], split[1]);
                }
                catch
                {
                    throw new Exception("Value definition expected!");
                }
            }
            return res;
        }

        public string GetValue(string valName)
        {
            return keyValues[valName];
        }

        public int GetInt(string valName)
        {
            return int.Parse(keyValues[valName]);
        }

        public float GetFloat(string valName)
        {
            
            return float.Parse(keyValues[valName]);//(float)Convert.ToDouble(keyValues[valName]);
        }

        public double GetDouble(string valName)
        {
            return double.Parse(keyValues[valName]);
        }

        public Vector3 GetVector3(string valName)
        {
            string res = GetValue(valName);
            res = res.Substring(1, res.Length - 2);
            string[] split = res.Split(',');
            return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
        }

        public string[] GetArray(string valName)
        {
            string arr = GetValue(valName);
            arr = arr.Substring(1, arr.Length - 2); //"[1,2,3]" => "1,2,3"
            string[] split = arr.Split(',');
            return split;
        }
    }
}

