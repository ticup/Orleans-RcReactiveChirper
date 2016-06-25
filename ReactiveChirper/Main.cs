using Microsoft.Owin.Hosting;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer
{
    public class Main : IBootstrapProvider
    {
        IDisposable host;
        Logger logger;

        public static int HistoryLength
        {
            get
            {
                return 25;
            }
        }

        public string Name
        {
            get
            {
                return "ReactiveChirper";
            }
        }


        public Task Close()
        {
            host.Dispose();
            return TaskDone.Done;
        }


        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            this.logger = providerRuntime.GetLogger("ReactiveChirper");

            var router = new Router();
            new ReactiveChirpController(router, TaskScheduler.Current,  providerRuntime);

            var options = new StartOptions
            {
                ServerFactory = "Nowin",
                Port = config.Properties.ContainsKey("Port") ? int.Parse(config.Properties["Port"]) : 8080,
            };

            var username = config.Properties.ContainsKey("Username") ? config.Properties["Username"] : null;
            var password = config.Properties.ContainsKey("Password") ? config.Properties["Password"] : null;
            try
            {
                host = WebApp.Start(options, app => new WebServer(router, username, password).Configuration(app));
            }
            catch (Exception ex)
            {
                this.logger.Error(10001, ex.ToString());
            }

            this.logger.Verbose($"Chirper listening on {options.Port}");


            return TaskDone.Done;
        }
    }
}
