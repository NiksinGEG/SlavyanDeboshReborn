using System;
using System.Text;
using System.Globalization;
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

        //s - строка в формате "имя:значение", возврат: ["имя", "значение"]
        private string[] Split(string s)
        {
            string[] res = new string[2];
            res[0] = "";
            res[1] = "";
            char c = s[0];
            res[0] += c;
            int i = 1;
            c = s[i];
            while (c != ':')
            {
                res[0] += c;
                c = s[++i];
            }
            ++i;
            while(i < s.Length)
            {
                c = s[i];
                res[1] += c;
                i++;
            }
            return res;
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
                s_arr += "(" + item.ToString() + "),";
            if (s_arr[s_arr.Length - 1] == ',')
                s_arr = s_arr.Remove(s_arr.Length - 1);
            s_arr += "]";

            _sb.AppendLine("{" + arrayName + ":" + s_arr + "}");

            keyValues.Add(arrayName, s_arr);
        }
        public void SerializeList<base_T>(string listName, List<base_T> list)
        {
            SerializeArray(listName, list.ToArray());
        }
        public override string ToString()
        {
            return _sb.ToString();
        }

        private string RemoveSpaces(string s)
        {
            string res = "";
            for(int i = 0; i < s.Length; i++)
                if (s[i] != ' ' && s[i] != '\n')
                    res += s[i];
            return res;
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
            s = RemoveSpaces(s);
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
                    string[] split = Split(new_s);

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
            var val = keyValues[valName].Replace(',', '.');
            return float.Parse(val, CultureInfo.InvariantCulture);
        }

        public double GetDouble(string valName)
        {
            return double.Parse(keyValues[valName], CultureInfo.InvariantCulture);
        }

        public Vector3 GetVector3(string valName)
        {
            string res = GetValue(valName);
            res = res.Substring(1, res.Length - 2);
            string[] split = res.Split(',');
            return new Vector3(float.Parse(split[0], CultureInfo.InvariantCulture), float.Parse(split[1], CultureInfo.InvariantCulture), float.Parse(split[2], CultureInfo.InvariantCulture));
        }

        public string[] GetArray(string valName)
        {
            string arr = GetValue(valName);
            List<string> split = new List<string>();
            int i = 1;
            char c = arr[i];
            while(c != ']')
            {
                if (c != '(')
                    throw new Exception("Wrong array serialization!");
                i++;
                int pars = 1;
                string new_s = "";
                while(pars > 0)
                {
                    if (c == '(')
                        pars++;
                    if (c == ')')
                        pars--;
                    if(pars > 0)
                        new_s += c;
                    c = arr[++i];
                }
                split.Add(new_s);
                if (c == ',')
                    c = arr[++i];
            }
            return split.ToArray();
        }
    }
}

