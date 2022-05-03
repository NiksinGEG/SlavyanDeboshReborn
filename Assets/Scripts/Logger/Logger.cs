using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Logger
{
    class Logger
    {
        private static Logger instance;

        private Logger()
        {

        }

        public static Logger getInstance()
        {
            if(instance == null)
                instance = new Logger();
            return instance;
        }
    }
}
