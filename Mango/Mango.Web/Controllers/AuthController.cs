using Mango.Web.Models;
using Mango.Web.Service.AuthService;
using Mango.Web.Service.TokenProvider;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);
        }

        [HttpGet]
        public IActionResult Register()
        {
            PopulateRoleList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            ResponseDTO? loginUser = await _authService.LoginAsync(loginRequestDTO);

            if (loginUser != null && loginUser.IsSuccess)
            {
                LoginResponseDTO? loginResponseDTO = JsonConvert.DeserializeObject<LoginResponseDTO?>(loginUser.Result.ToString());
                await SignInUser(loginResponseDTO);
                _tokenProvider.SetToken(loginResponseDTO.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = loginUser.Message;
                return View(loginRequestDTO);
            }
            //else
            //{
            //    ModelState.AddModelError(string.Empty, loginUser.Message);
            //    return View(loginRequestDTO);
            //}
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ResponseDTO? registerUser = await _authService.RegisterAsync(registrationRequestDTO);
            ResponseDTO? assignRole;

            if(registerUser != null && registerUser.IsSuccess)
            {
                if (string.IsNullOrEmpty(registrationRequestDTO.Role))
                {
                    registrationRequestDTO.Role = StaticDetails.RoleCustomer;
                }
                else
                {
                    assignRole = await _authService.AssignRoleAsync(registrationRequestDTO);
                    if (assignRole != null && assignRole.IsSuccess)
                    {
                        TempData["success"] = "Registration successful.";
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        TempData["error"] = assignRole.Message;
                    }  
                }
            }
            else
            {
                TempData["error"] = registerUser.Message;
            }

            PopulateRoleList();
            return View(registrationRequestDTO);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDTO loginResponseDTO)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResponseDTO.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(
                new Claim(
                    JwtRegisteredClaimNames.Email, 
                    jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value
                    ));
            identity.AddClaim(
                new Claim(
                    JwtRegisteredClaimNames.Sub, 
                    jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value
                    ));
            identity.AddClaim(
                new Claim(
                    JwtRegisteredClaimNames.Name, 
                    jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value
                    ));

            identity.AddClaim(
                new Claim(
                    ClaimTypes.Name,
                    jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value
                    ));
            identity.AddClaim(
               new Claim(
                   ClaimTypes.Role,
                   jwt.Claims.FirstOrDefault(x => x.Type == "role").Value
                   ));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private void PopulateRoleList()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{ Value = StaticDetails.RoleAdmin, Text = StaticDetails.RoleAdmin },
                new SelectListItem{ Value = StaticDetails.RoleCustomer, Text = StaticDetails.RoleCustomer },
            };
            ViewBag.RoleList = roleList;
        }
    }
}
