﻿using System;
using System.Net;
using System.Threading.Tasks;

using Orleans.Runtime.Host;
using System.Reflection;
using System.IO;
using WebServer;
using Orleans.Runtime.Configuration;
using System.Collections.Generic;

namespace TestHost
{
    internal class OrleansHostWrapper : IDisposable
    {
        public bool Debug
        {
            get { return siloHost != null && siloHost.Debug; }
            set { siloHost.Debug = value; }
        }

        private SiloHost siloHost;

        public OrleansHostWrapper()
        {
            siloHost = new SiloHost("primary", ClusterConfiguration.LocalhostPrimarySilo());
        }

        public bool Run()
        {
            bool ok = false;

            try
            {
                Dictionary<string, string> opts = new Dictionary<string, string>();
                opts.Add("Port", "8080");
                siloHost.InitializeOrleansSilo();
                siloHost.Config.Globals.RegisterBootstrapProvider<Main>("Main", opts);
                siloHost.Config.AddMemoryStorageProvider();

                ok = siloHost.StartOrleansSilo();
                if (!ok) throw new SystemException(string.Format("Failed to start Orleans silo '{0}' as a {1} node.", siloHost.Name, siloHost.Type));
            }
            catch (Exception exc)
            {
                siloHost.ReportStartupError(exc);
                var msg = string.Format("{0}:\n{1}\n{2}", exc.GetType().FullName, exc.Message, exc.StackTrace);
                Console.WriteLine(msg);
            }

            return ok;
        }

        public bool Stop()
        {
            bool ok = false;

            try
            {
                siloHost.StopOrleansSilo();
            }
            catch (Exception exc)
            {
                siloHost.ReportStartupError(exc);
                var msg = string.Format("{0}:\n{1}\n{2}", exc.GetType().FullName, exc.Message, exc.StackTrace);
                Console.WriteLine(msg);
            }

            return ok;
        }

   



        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            siloHost.Dispose();
            siloHost = null;
        }
    }
}
