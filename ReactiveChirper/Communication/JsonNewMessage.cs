using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Communication
{
    class JsonNewMessage : JsonMessage
    {
        public string Username;
        public string Message;


        public override string Type
        {
            get
            {
                return "NewMessage";

            }
        }

    }
}
