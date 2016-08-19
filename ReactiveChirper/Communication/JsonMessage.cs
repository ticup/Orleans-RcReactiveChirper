using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Communication
{
    public abstract class JsonMessage
    {
        public abstract string Type { get; }
        public object Data;

    }
}
