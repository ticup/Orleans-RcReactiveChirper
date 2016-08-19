using Microsoft.Owin;

using Owin;
using Owin.WebSocket.Extensions;
using System;
using System.Text;
using System.Threading.Tasks;
using GrainInterfaces;
using Newtonsoft.Json;
using Orleans.Providers;
using WebServer.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Orleans;
using Owin.WebSocket;
using Microsoft.Owin;

namespace WebServer
{
    using WebSocketAccept = Action<IDictionary<string, object>, // options
        Func<IDictionary<string, object>, Task>>; // callback
    using WebSocketCloseAsync =
        Func<int /* closeStatus */,
            string /* closeDescription */,
            CancellationToken /* cancel */,
            Task>;
    using WebSocketReceiveAsync =
        Func<ArraySegment<byte> /* data */,
            CancellationToken /* cancel */,
            Task<Tuple<int /* messageType */,
                bool /* endOfMessage */,
                int /* count */>>>;
    using WebSocketSendAsync =
        Func<ArraySegment<byte> /* data */,
            int /* messageType */,
            bool /* endOfMessage */,
            CancellationToken /* cancel */,
            Task>;
    using WebSocketReceiveResult = Tuple<int, // type
        bool, // end of message?
        int>; // count
    using Microsoft.Owin.Cors;

    public class WebServer
    {
        Router Router;

        public WebServer(Router router)
        {
            Router = router;
        }
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
            app.Use(HandleRequest);
        }

        Task HandleRequest(IOwinContext context, Func<Task> func)
        {
            var result = this.Router.Match(context.Request.Path.Value);
            if (null != result)
            {
                return result(context);
            }

            context.Response.StatusCode = 404;
            return Task.FromResult(0);
        }
    }

}
