using Mango.Web.Models;
using Mango.Web.Service.BaseService;
using Mango.Web.Utility;

namespace Mango.Web.Service.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        private readonly string APIName;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
            APIName = "AuthAPI";
        }

        public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = registrationRequestDTO,
                    Url = $"{StaticDetails.AuthAPIBase}/api/{APIName}/AssignRole"
                });
        }

        public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = loginRequestDTO,
                    Url = $"{StaticDetails.AuthAPIBase}/api/{APIName}/Login"
                }, withBearer: false);
        }

        public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = registrationRequestDTO,
                    Url = $"{StaticDetails.AuthAPIBase}/api/{APIName}/Register"
                }, withBearer: false);
        }
    }
}
