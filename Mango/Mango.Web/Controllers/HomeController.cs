using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.ProductService;
using Mango.Web.Service.ShoppingCartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public HomeController(IProductService productService, IShoppingCartService shoppingCartService)
		{
			_productService = productService;
            _shoppingCartService = shoppingCartService;
        }

		public async Task<IActionResult> Index()
		{
			List<ProductDTO>? list = new();

			ResponseDTO? response = await _productService.GetAllProductAsync();

			if (response != null && response.IsSuccess)
				list = JsonConvert.DeserializeObject<List<ProductDTO>>(response.Result.ToString());
			else
				TempData["error"] = response?.Message;

			return View(list);
		}

		[Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDTO? productDTO = new();

            ResponseDTO? response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
                productDTO = JsonConvert.DeserializeObject<ProductDTO>(response.Result.ToString());
            else
                TempData["error"] = response?.Message;

            return View(productDTO);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDTO productDTO)
        {
            CartDTO cartDTO = new()
            {
                CartHeader = new()
                {
                    UserId = User.Claims.Where(x => x.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDTO cartDetailsDTO = new()
            {
                Count = productDTO.Count,
                ProductId = productDTO.ProductId,
            };

            List<CartDetailsDTO> cartDetailsDTOs = new() { cartDetailsDTO };
            cartDTO.CartDetails = cartDetailsDTOs;

            ResponseDTO? response = await _shoppingCartService.UpsertCartAsync(cartDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been added to the shopping cart.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(productDTO);
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}