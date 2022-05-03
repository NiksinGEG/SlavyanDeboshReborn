using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assets.Scripts.Logger
{
    class Loger
    {
        private static Loger instance;

        private Loger()
        {
            
        }

        public static void DeleteLog(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { }
        }

        public static void Log(string path,string message)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path, true, Encoding.ASCII);
                sw.WriteLine(message);
                sw.Close();
            }
            catch { }
        }

        public static Loger getInstance()
        {
            if(instance == null)
                instance = new Loger();
            return instance;
        }
    }
}
