using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.OrderService;
using Mango.Web.Service.ShoppingCartService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;

        public CartController(IShoppingCartService shoppingCartService, 
                              IOrderService orderService)
        {
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartBasedOnLoginbUsers());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartBasedOnLoginbUsers());
        }

        [Authorize]
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            CartDTO cart =  await LoadCartBasedOnLoginbUsers();
            cart.CartHeader.Name = cartDTO.CartHeader.Name;
            cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            cart.CartHeader.Email = cartDTO.CartHeader.Email;

            var response = await _orderService.CreateOrderAsync(cart);
            if(response != null && response.IsSuccess)
            {
                OrderHeaderDTO? orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(response.Result.ToString());

                //get stripe session and redirect to stripe to place order
                var domain = $"{Request.Scheme}://{Request.Host.Value}/";
                StripeRequestDTO stripeRequestDTO = new()
                {
                    ApprovedUrl = $"{domain}cart/Confirmation?orderId={orderHeaderDTO.OrderHeaderId}",
                    CancelUrl = $"{domain}cart/checkout",
                    OrderHeader = orderHeaderDTO
                };

                var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDTO);
                StripeRequestDTO? stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDTO>(stripeResponse.Result.ToString());
                Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);
                return new StatusCodeResult(303);
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return View(cart);
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDTO? response = await _orderService.ValidateStripeSession(orderId);

            if (response != null && response.IsSuccess)
            {
                OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(response.Result.ToString());
                if(orderHeaderDTO.Status == StaticDetails.Status_Approved)
                {
                    return View(orderId);
                }
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(orderId);
        }

        [Authorize]
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _shoppingCartService.RemoveFromCartAsync(cartDetailsId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCouponCode(CartDTO cartDTO)
        {
            ResponseDTO? response = await _shoppingCartService.ApplyCouponAsync(cartDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmailShoppingCart(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartBasedOnLoginbUsers();
            cart.CartHeader.Email = User
                .Claims
                .Where(x => x.Type == JwtRegisteredClaimNames.Email)?
                .FirstOrDefault()?
                .Value;

            ResponseDTO? response = await _shoppingCartService.EmailCart(cart);

            if (response != null && response.IsSuccess)
                TempData["success"] = "Email will be processed and sent shortly.";
            else
                TempData["error"] = response?.Message;

            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCouponCode(CartDTO cartDTO)
        {
            cartDTO.CartHeader.CouponCode = string.Empty;
            ResponseDTO? response = await _shoppingCartService.ApplyCouponAsync(cartDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
                return View();
            }
        }

        private async Task<CartDTO> LoadCartBasedOnLoginbUsers()
        {
            var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _shoppingCartService.GetCartByUserIdAsync(userId);

            if(response != null && response.IsSuccess)
            {
                CartDTO? cartDTO = JsonConvert.DeserializeObject<CartDTO>(response.Result.ToString());
                return cartDTO;
            }

            return new();
        }
    }
}
