﻿using Microsoft.Owin.Hosting;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveChirper
{
    public class Dashboard : IBootstrapProvider
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
                return "Dashboard";
            }
        }


        public Task Close()
        {
            host.Dispose();
            return TaskDone.Done;
        }


        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            this.logger = providerRuntime.GetLogger("Dashboard");

            var router = new Router();
            new DashboardController(router, TaskScheduler.Current,  providerRuntime);

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

            var dashboardGrain = providerRuntime.GrainFactory.GetGrain<IDashboardGrain>(0);
            return dashboardGrain.Init();
        }
    }
}
