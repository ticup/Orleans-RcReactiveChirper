﻿using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveChirper
{
    public interface IDashboardGrain : IGrain, IGrainWithIntegerKey
    {
        Task Init();

        Task<DashboardCounters> GetCounters();
    }
}
