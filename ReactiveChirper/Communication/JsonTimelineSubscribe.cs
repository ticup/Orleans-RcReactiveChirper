using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Communication
{
    class JsonTimelineSubscribe: JsonMessage
    {
        public override string Type
        {
            get
            {
                return "TimelineSubscribe";

            }
        }

        public string Username;
    }
}
