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

namespace WebServer
{

    public class ReactiveChirpController 
    {
        public TaskScheduler TaskScheduler { get; private set; }
        public IProviderRuntime ProviderRuntime { get; private set; }


        public ReactiveChirpController(Router router, TaskScheduler taskScheduler, IProviderRuntime providerRuntime)
        {

            this.TaskScheduler = taskScheduler;
            this.ProviderRuntime = providerRuntime;

            Action<string, Func<IOwinContext, IDictionary<string, string>, Task>> add = router.Add;

            // Print out all the files that belong to the assembly
            // -> These are alle the files that can be returned by the .ReturnFromFile() method.
            // Add them by right-clicking on the solution -> add new/existing item.
            // Then click on the new item and go to properties (bottom right) and change "Build Action" to "Embedded Resource"!
            var assembly = Assembly.GetExecutingAssembly();
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                Console.Out.WriteLine(resourceName);
            }


            // Setup routing
            add("/", ChirperPage);
            add("/chirper.min.js", ChirperJs);

            add("/timeline/:username", GetTimeline);
            add("/followers/:username", GetFollowers);
            add("/message/new", NewMessage);
            add("/follow", Follow);
        }


        // GET /timeline/:username
        // => {posts: Message[]}
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

        // GET /followers/:username
        // => {followers: string[]}
        async Task GetFollowers(IOwinContext context, IDictionary<string, string> parameters)
        {
            string username;
            parameters.TryGetValue("username", out username);
            var grain = this.ProviderRuntime.GrainFactory.GetGrain<IUserGrain>(username);
            List<string> followers = (List<string>)await Dispatch(async () =>
            {
                return await grain.GetFollowersList();
            });
            await context.ReturnJson(new { followers = followers });
        }

        // PUT /message/new {username, text}
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
            await context.ReturnJson(new { succeed = result });
        }

        // PUT /follow {username, toFollow}
        async Task Follow(IOwinContext context, IDictionary<string, string> parameters)
        {
            var formData = await context.Request.ReadFormAsync() as IEnumerable<KeyValuePair<string, string[]>>;
            var username = formData.FirstOrDefault(x => x.Key == "username").Value[0];
            var toFollow = formData.FirstOrDefault(x => x.Key == "toFollow").Value[0];
            await Dispatch(async () =>
            {
                var grain = this.ProviderRuntime.GrainFactory.GetGrain<IUserGrain>(username);
                await grain.Follow(toFollow);
                return true;
                //return await grain.GetTimeline(100);
            });
            await context.ReturnJson(new { succeed = true });
        }

        

        /* Static pages */
        Task ChirperPage(IOwinContext context, IDictionary<string, string> parameters)
        {
            return context.ReturnFile("www.Chirper.html", "text/html");
        }

        Task ChirperJs(IOwinContext context, IDictionary<string, string> parameters)
        {
            return context.ReturnFile("www.chirper.min.js", "application/javascript");
        }

        Task<object> Dispatch(Func<Task<object>> func)
        {
            return Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, scheduler: this.TaskScheduler).Result;
        }

    }
}
