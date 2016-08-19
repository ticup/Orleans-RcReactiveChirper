using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Communication
{
    class JsonFollow: JsonMessage
    {
        public string Username;
        public string ToFollow;


        public override string Type
        {
            get
            {
                return "Follow";

            }
        }

    }
}
