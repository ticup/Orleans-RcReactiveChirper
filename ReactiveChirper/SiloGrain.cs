﻿using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Runtime;

namespace ReactiveChirper
{
    public class SiloGrain : Grain, ISiloGrain
    {
        Queue<SiloRuntimeStatistics> stats;
        IDisposable timer;

        public override async Task OnActivateAsync()
        {
            stats = new Queue<SiloRuntimeStatistics>();

            foreach (var x in Enumerable.Range(1, Dashboard.HistoryLength))
            {
                stats.Enqueue(null);
            }

            timer = this.RegisterTimer(this.Callback, true, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

            await Callback(false);

            await base.OnActivateAsync();
        }

        async Task Callback(object canDeactivate)
        {
            var address = SiloAddress.FromParsableString(this.GetPrimaryKeyString());
            var grain = this.GrainFactory.GetGrain<IManagementGrain>(0);
            try
            {
                var results = (await grain.GetRuntimeStatistics(new SiloAddress[] { address })).FirstOrDefault();

                stats.Enqueue(results);
                while (this.stats.Count > Dashboard.HistoryLength)
                {
                    this.stats.Dequeue();
                }
            }
            catch (Exception)
            {
                // we can't get the silo stats, it's probably dead, so kill the grain
                if (!(bool) canDeactivate) return;
                if (null != timer) timer.Dispose();
                timer = null;
                this.DeactivateOnIdle();
            }
        }

        public Task<SiloRuntimeStatistics[]> GetRuntimeStatistics()
        {
            return Task.FromResult(this.stats.ToArray());
        }
    }
}
