
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketServer
{
    // http://owin.org/extensions/owin-WebSocket-Extension-v0.4.0.htm
    

    /// <summary>
    /// This sample requires Windows 8, .NET 4.5, and Microsoft.Owin.Host.HttpListener.
    /// </summary>
    public class Startup
    {
        // Run at startup
        public void Configuration(IAppBuilder app)
        {
            app.UseWelcomePage();
        }

        // Run once per request
        

        
    }
}