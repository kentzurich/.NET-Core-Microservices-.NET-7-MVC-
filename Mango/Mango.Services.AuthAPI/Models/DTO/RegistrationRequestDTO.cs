﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthAPI.Models.DTO
{
    public class RegistrationRequestDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
