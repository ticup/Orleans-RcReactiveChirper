using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Communication
{
    class JsonException: JsonMessage
    {
        public override string Type {
            get
            {
                return "Exception";

            }
        }

        public string Text;
    }
}
