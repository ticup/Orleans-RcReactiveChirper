using System.Threading.Tasks;
using Orleans;
using System.Collections.Generic;

namespace GrainInterfaces
{
    /// <summary>
    /// Grain interface IUserGrain
    /// </summary>
	public interface IUserGrain : IGrainWithStringKey
    {
        Task<List<string>> GetFollowersList();
        Task<List<Message>> GetMessages(int amount);
        Task<Timeline> GetTimeline(int amount);

        Task Follow(string userName);
        Task Unfollow(string userName);
        Task<bool> PostText(string text);  
    }
}