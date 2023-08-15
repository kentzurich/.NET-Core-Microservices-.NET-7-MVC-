using Mango.MessageBus;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private protected ResponseDTO _response;

        public AuthAPIController(IAuthService authService, 
                                 IMessageBus messageBus,
                                 IConfiguration configuration)
        {
            _authService = authService;
            _messageBus = messageBus;
            _configuration = configuration;
            _response = new();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            var errorMessage = await _authService.Register(registrationRequestDTO);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }

            await _messageBus.PublishMessage(registrationRequestDTO.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));

            return Ok(_response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await _authService.Login(loginRequestDTO);
            if(loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or Password in incorrect.";
                return BadRequest(_response);
            }

            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            var assignedRoleSuccess = await _authService.AssignRole(registrationRequestDTO.Email, registrationRequestDTO.Role.ToUpper());
            if (!assignedRoleSuccess)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered.";
                return BadRequest(_response);
            }

            return Ok(_response);
        }
    }
}
