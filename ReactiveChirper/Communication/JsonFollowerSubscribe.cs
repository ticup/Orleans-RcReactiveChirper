using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Communication
{
    class JsonFollowerSubscribe: JsonMessage
    {
        public string Username;

        public override string Type
        {
            get
            {
                return "FollowerSubscribe";
            }
        }
        
    }
}
