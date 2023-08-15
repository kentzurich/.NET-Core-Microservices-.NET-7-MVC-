using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.DTO;

namespace Mango.Service.EmailAPI.Services.Email
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO cartDTO);
        Task RegisterUserEmailAndLog(string email);
        Task LogPlacedOrder(RewardsMessage rewardsMessage);
    }
}
