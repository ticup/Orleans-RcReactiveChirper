using Microsoft.Owin;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using GrainInterfaces;

namespace ReactiveChirper
{

    public class DashboardController 
    {
        public TaskScheduler TaskScheduler { get; private set; }
        public IProviderRuntime ProviderRuntime { get; private set; }


        public DashboardController(Router router, TaskScheduler taskScheduler, IProviderRuntime providerRuntime)
        {

            this.TaskScheduler = taskScheduler;
            this.ProviderRuntime = providerRuntime;

            Action<string, Func<IOwinContext, IDictionary<string, string>, Task>> add = router.Add;
            var assembly = Assembly.GetExecutingAssembly();
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                Console.Out.WriteLine(resourceName);
            }


            add("/", Index);
            add("/index.min.js", IndexJs);
            add("/DashboardCounters", GetDashboardCounters);
            add("/RuntimeStats/:address", GetRuntimeStats);
            add("/HistoricalStats/:address", GetHistoricalStats);

            add("/chirper", ChirperPage);
            add("/chirper.min.js", ChirperJs);
            add("/timeline/:username", GetTimeline);
            add("/message/new", NewMessage);

            //this.Get["/SiloPerformanceMetrics"] = GetSiloPerformanceMetrics;
            //this.Get["/ClientPerformanceMetrics"] = GetClientPerformanceMetrics;
            //this.Get["/Counters"] = GetCounters;
            //add("/ForceActivationCollection/{timespan:int}/{address?}", PostForceActivationCollection);
        }



        Task Index(IOwinContext context, IDictionary<string,string> parameters)
        {
            return context.ReturnFile("Index.html", "text/html");
        }

        Task IndexJs(IOwinContext context, IDictionary<string, string> parameters)
        {
            return context.ReturnFile("index.min.js", "application/javascript");
        }


        async Task GetTimeline(IOwinContext context, IDictionary<string, string> parameters)
        {
            string username;
            parameters.TryGetValue("username", out username);
            var grain = this.ProviderRuntime.GrainFactory.GetGrain<IUserGrain>(username);
            Timeline result = (Timeline)await Dispatch(async () =>
           {
               return await grain.GetTimeline(100);
           });
           await context.ReturnJson(result);
        }

        async Task NewMessage(IOwinContext context, IDictionary<string, string> parameters)
        {
            var formData = await context.Request.ReadFormAsync() as IEnumerable<KeyValuePair<string, string[]>>;
            var username = formData.FirstOrDefault(x => x.Key == "username").Value[0];
            var text = formData.FirstOrDefault(x => x.Key == "text").Value[0];
            bool result = (bool) await Dispatch(async () =>
            {
                var grain = this.ProviderRuntime.GrainFactory.GetGrain<IUserGrain>(username);
                return await grain.PostText(text);
            });
            NewMessageResponse response = new NewMessageResponse { succeed = result };
            await context.ReturnJson(response);
        }


        /*
        //object PostForceActivationCollection(dynamic parameters)
        {
            var grain = Dashboard.ProviderRuntime.GrainFactory.GetGrain<IManagementGrain>(0);
            var timespan = TimeSpan.FromSeconds(parameters.timespan);

            if (parameters.address.HasValue)
            {
                var address = SiloAddress.FromParsableString((string)parameters.address);
                Dispatch(async () =>
                {
                    await grain.ForceActivationCollection(new SiloAddress[] { address }, timespan);
                    return "";
                });
            }
            else
            {
                Dispatch(async () =>
                {
                    await grain.ForceActivationCollection(timespan);
                    return "";
                });
            }

            return this.Response.AsJson(new { });
        }
        */

        Task ChirperPage(IOwinContext context, IDictionary<string, string> parameters)
        {
            return context.ReturnFile("www.Chirper.html", "text/html");
        }

        Task ChirperJs(IOwinContext context, IDictionary<string, string> parameters)
        {
            return context.ReturnFile("www.chirper.min.js", "application/javascript");
        }

        async Task GetDashboardCounters(IOwinContext context, IDictionary<string, string> parameters)
        {
            var grain = this.ProviderRuntime.GrainFactory.GetGrain<IDashboardGrain>(0);

            var result = await Dispatch(async () => {
                return await grain.GetCounters();
            });

            await context.ReturnJson(result);
        }


        async Task GetRuntimeStats(IOwinContext context, IDictionary<string, string> parameters)
        {
            var address = SiloAddress.FromParsableString(parameters["address"]);
            var grain = this.ProviderRuntime.GrainFactory.GetGrain<IManagementGrain>(0);
            
            var result = await Dispatch(async () =>
            {
                Dictionary<SiloAddress, SiloStatus> silos = await grain.GetHosts(true);
                
                SiloStatus siloStatus;
                if (silos.TryGetValue(address, out siloStatus))
                {
                    return (await grain.GetRuntimeStatistics(new SiloAddress[] { address })).FirstOrDefault();
                }
                return null;
            });


            await context.ReturnJson(result);
        }

        async Task GetHistoricalStats(IOwinContext context, IDictionary<string, string> parameters)
        {
            var grain = this.ProviderRuntime.GrainFactory.GetGrain<ISiloGrain>(parameters["address"]);

            var result = await Dispatch(async () =>
            {
                return await grain.GetRuntimeStatistics();
            });

            await context.ReturnJson(result);
        }


        Task<object> Dispatch(Func<Task<object>> func)
        {
            return Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, scheduler: this.TaskScheduler).Result;
        }

    }
}
