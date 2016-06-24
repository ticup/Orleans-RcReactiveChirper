using System.Threading.Tasks;
using Orleans;
using GrainInterfaces;
using System.Collections.Generic;
using System;
using Orleans.Providers;
using System.Linq;

namespace Grains
{

    public class UserGrainState
    {
        
        public HashSet<string> Subscriptions { get; set; }
        public List<IMessageChunkGrain> MessageChunks { get; set; }
       
    }



    [StorageProvider(ProviderName = "MemoryStore")]
    public class UserGrain : Grain<UserGrainState>, IUserGrain
    {
        IMessageChunkGrain CurrentChunk;
        public string Name { get; set; }



        public Task Follow(string userName)
        {
            //IUserGrain userGrain = GrainClient.GrainFactory.GetGrain<IUserGrain>(userName);
            State.Subscriptions.Add(userName);
            return WriteStateAsync();
        }

        public Task Unfollow(string userName)
        {
            State.Subscriptions.Remove(userName);
            return WriteStateAsync();
        }

        public Task<List<string>> GetFollowersList()
        {
            return Task.FromResult(State.Subscriptions.ToList());
        }

        public async Task<List<Message>> GetMessages(int amount)
        {
            // There is a possibility of caching x number of chunks in here too.
            int NoChunks = (int)Math.Ceiling((double) (amount / MessageChunkGrain.MessageChunkSize));
            int Cur = 0;
            List<Task<List<Message>>> tasks = new List<Task<List<Message>>>();
            List<Message> FlatMsgs = new List<Message>();
            foreach (IMessageChunkGrain c in State.MessageChunks)
            {
                if (Cur > NoChunks) break;
                tasks.Add(c.getMessages());
            }
            List<Message>[] Msgs = await Task.WhenAll(tasks);
            foreach(List<Message> l in Msgs)
            {
                FlatMsgs.AddRange(l);
            }
            return FlatMsgs;
        }

        public async Task<bool> PostText(string text)
        {
            //  add the message to the current chunk
            Message msg = new Message(text, Name);
            bool succeed = await CurrentChunk.AddMessage(msg);
            if (succeed) return true;

            // chunk was full, add new one and try again (only once, not recursive).
            await AddChunk();
            return await CurrentChunk.AddMessage(msg);
        }

        public override Task OnActivateAsync()
        {
            // First time activating this grain instance
            if (State.MessageChunks == null)
            {
                State.MessageChunks = new List<IMessageChunkGrain>();
                AddChunk();
            }
            if (State.Subscriptions == null)
            {
                State.Subscriptions = new HashSet<string>();
            }

            Name = this.GetPrimaryKeyString();
            CurrentChunk = State.MessageChunks[State.MessageChunks.Count - 1];

            return TaskDone.Done;
        }

        private Task AddChunk()
        {
            Guid id = Guid.NewGuid();
            IMessageChunkGrain chunk = GrainFactory.GetGrain<IMessageChunkGrain>(id);
            State.MessageChunks.Add(chunk);
            CurrentChunk = chunk;
            return WriteStateAsync();
        }



        // i) gets limit number of messages from every subscription (huge overestimation) and yourself,
        // ii) sorts them to date and
        // ii) returns limit number of messages
        public async Task<Timeline> GetTimeline(int limit)
        {
            List<Message> Msgs = new List<Message>();
            List<Task<List<Message>>> tasks = new List<Task<List<Message>>>();
            foreach (string UserName in State.Subscriptions)
            {
                IUserGrain user = GrainFactory.GetGrain<IUserGrain>(UserName);
                tasks.Add(user.GetMessages(limit));
            }
            List<Message>[] MsgsByUser = await Task.WhenAll(tasks);
            foreach (List<Message> Umsgs in MsgsByUser)
            {
                Msgs.AddRange(Umsgs);
            }
            Msgs.AddRange(await GetMessages(limit));
            return new Timeline(Msgs.OrderBy((m) => m.Timestamp).Take(limit).ToList());
        }
    }
}
