using Fleck;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using Owin;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin.WebSocket.Extensions;

namespace WebServer
{
    public class Main : IBootstrapProvider
    {
        IDisposable host;
        Logger logger;

        // Required by the Orleans silo
        public string Name
        {
            get
            {
                return "ReactiveChirper";
            }
        }

        // Called by the Orleans silo when it shuts down
        public Task Close()
        {
            host.Dispose();
            return TaskDone.Done;
        }

        // Called by the Orleans silo when it starts
        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            logger = providerRuntime.GetLogger("ReactiveChirper");
            ChirperHub.ProviderRuntime = providerRuntime;
            ChirperHub.TaskScheduler = TaskScheduler.Current;

            var options = new StartOptions
            {
                ServerFactory = "Nowin",
                Port = 8081,
            };

            // setup router and controller for REST calls (mainly static file serving)
            var router = new Router();
            var controller = new Controller(router);

            // create a web server that will relay its requests to the controller and the WebSocketHandler
            var webserver = new WebServer(router);

            // Start the Owin server and configure our web server with it
            host = WebApp.Start(options, app => webserver.Configuration(app));
            
            return TaskDone.Done;
        }
    }
}
