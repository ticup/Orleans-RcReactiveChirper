using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IMessageChunkGrain : Orleans.IGrainWithGuidKey
    {
        Task<List<Message>> getMessages();
        Task<bool> AddMessage(Message message);
    }
}
