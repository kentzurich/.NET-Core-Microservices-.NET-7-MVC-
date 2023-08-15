using Mango.Services.RewardAPI.Message;

namespace Mango.Service.RewardAPI.Services.Email
{
    public interface IRewardsService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}
