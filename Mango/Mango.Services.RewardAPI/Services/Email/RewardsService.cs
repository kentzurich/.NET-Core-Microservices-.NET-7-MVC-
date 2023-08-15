using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.RewardAPI.Services.Email
{
    public class RewardsService : IRewardsService
    {
        private DbContextOptions<AppDbContext> _options;

        public RewardsService(DbContextOptions<AppDbContext> options) => _options = options;
        
        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    UserId = rewardsMessage.UserId,
                    RewardsDate = DateTime.Now
                };

                await using var _db = new AppDbContext(_options);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();
            }
            catch {  }
        }
    }
}
