using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Systems.Net
{
    public class Serializer
    {
        StringBuilder _sb;

        public Serializer() { _sb = new StringBuilder(); }

        public void SerializeField<field_T>(string fieldName, field_T fieldVal)
        {
            _sb.AppendLine("{" + fieldName + ":" + fieldVal.ToString() + "}");
        }
        public void SerializeArray<base_T>(string arrayName, base_T[] arr)
        {
            _sb.Append("{" + arrayName + ":[");
            foreach(var elem in arr)
                _sb.Append(elem.ToString() + ",");
            if (_sb[_sb.Length - 1] == ',')
                _sb.Remove(_sb.Length - 1, 1);
            _sb.Append("]}");
            _sb.AppendLine();
        }
        public void SerializeList<base_T>(string listName, List<base_T> list)
        {
            _sb.Append("{" + listName + ":[");
            foreach (var elem in list)
                _sb.Append(elem.ToString() + ",");
            if (_sb[_sb.Length - 1] == ',')
                _sb.Remove(_sb.Length - 1, 1);
            _sb.Append("]}");
            _sb.AppendLine();
        }
        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}

