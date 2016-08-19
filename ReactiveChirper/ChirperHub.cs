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
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;

namespace WebServer
{
    public class ChirperHub : Hub
    {
        public static IProviderRuntime ProviderRuntime;
        public static TaskScheduler TaskScheduler;

        //HashSet<string> TimelineSubscriptions = new HashSet<string>();
        //HashSet<string> FollowerSubscriptions = new HashSet<string>();

        //public async Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        //{
        //    var jsonString = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
        //    //Use something like Json.Net to read the json
        //    var json = JsonConvert.DeserializeObject<JsonMessage>(jsonString);
        //    switch (json.Type)
        //    {
        //        case "TimelineSubscribe":
        //            TimelineSubscribe(((JsonTimelineSubscribe)json).Username);
        //            break;

        //        case "FollowerSubscribe":
        //            FollowerSubscribe(((JsonFollowerSubscribe)json).Username);
        //            break;

        //        case "Follow":
        //            Follow(((JsonFollow)json).Username, ((JsonFollow)json).ToFollow);
        //            break;

        //        case "NewMessage":
        //            NewMessage(((JsonNewMessage)json).Username, ((JsonNewMessage)json).Message);
        //            break;

        //        default:
        //            Send(new Communication.JsonException { Text = "unknown request" });
        //            break;
        //    }


        //}


        /* Timeline API */
        public void TimelineSubscribe(string username)
        {
            //TimelineSubscriptions.Add(username);
            var grain = ProviderRuntime.GrainFactory.GetGrain<IUserGrain>(username);
            Dispatch(async () =>
            {
                var Rc = ProviderRuntime.GrainFactory.StartReactiveComputation(() =>
                    grain.GetTimeline(100));

                var It = Rc.GetResultEnumerator();

                while (true)
                {
                    var result = await It.NextResultAsync();
                    Clients.Caller.TimelineResult(new { Type = "TimelineResult", timeline = result });
                }
            });
        }

        //void TimelineUnsubscribe(string username)
        //{
        //    TimelineSubscriptions.Remove(username);
        //}


        /* 
         * Follower API
         */
        public void FollowerSubscribe(string username)
        {
            //FollowerSubscriptions.Add(username);
            var grain = ProviderRuntime.GrainFactory.GetGrain<IUserGrain>(username);
            Dispatch(async () =>
            {
                var Rc = ProviderRuntime.GrainFactory.StartReactiveComputation(() =>
                    grain.GetFollowersList());


                var It = Rc.GetResultEnumerator();

                while (true)
                {
                    var result = await It.NextResultAsync();
                    Clients.Caller.FollowerResult(new { type = "FollowerResult", followers = result });
                }
            });
        }

        //void FollowerUnsubscribe(string username)
        //{
        //    FollowerSubscriptions.Remove(username);
        //}

        public async void Follow(string username, string toFollow)
        {
            await Dispatch(async () =>
            {
                var grain = ProviderRuntime.GrainFactory.GetGrain<IUserGrain>(username);
                await grain.Follow(toFollow);
                //Clients.Caller.(new { succeed = true });
                //return await grain.GetTimeline(100);
            });
        }

        public async void NewMessage(string username, string text)
        {
            await Dispatch(async () =>
            {
                var grain = ProviderRuntime.GrainFactory.GetGrain<IUserGrain>(username);
                var result = await grain.PostText(text);
                //Send(new { succeed = result });
            });
        }


        Task Dispatch(Func<Task> func)
        {
            return Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, scheduler: TaskScheduler).Result;
        }


        /*
         * Socket Lifetime
         */

        public void OnOpen()
        {
            Console.WriteLine("Socket was opened!");
        }

        public void OnClose()
        {
            Console.WriteLine("Socket was closed!");
        }

        public bool Authenticate(IOwinRequest request)
        {
            return true;
        }
    }

}