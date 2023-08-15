using System.ComponentModel.DataAnnotations;

namespace Mango.Services.RewardAPI.Models
{
    public class Rewards
    {
        [Key]
        public int RewardId { get; set; }
        public string UserId { get; set; }
        public DateTime RewardsDate { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }
    }
}
