using Mango.Service.EmailAPI.Data;
using Mango.Service.EmailAPI.Models;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Service.EmailAPI.Services.Email
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _options;

        public EmailService(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task EmailCartAndLog(CartDTO cartDTO)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("</br>Cart Email Requested ");
            message.AppendLine("</br>Total: " + cartDTO.CartHeader.CartTotal);
            message.Append("</br>");
            message.Append("<ul>");

            foreach(var item in cartDTO.CartDetails)
            {
                message.Append("<li>");
                message.Append($"{item.Product.Name} x {item.Count}");
                message.Append("</li>"); 
            }

            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDTO.CartHeader.Email);
        }

        public async Task LogPlacedOrder(RewardsMessage rewardsMessage)
        {
            string message = "New Order Placed. </br> Order Id: " + rewardsMessage.OrderId;
            await LogAndEmail(message, "admin@user.com");
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "User registration successfull. </br> Email : " + email;
            await LogAndEmail(message, "admin@user.com");
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message,
                };

                await using var _db = new AppDbContext(_options);
                await _db.EmailLoggers.AddAsync(emailLogger);
                await _db.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }
    }
}
