using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthAPI.Models.DTO
{
    public class LoginRequestDTO
    {
        [EmailAddress]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
