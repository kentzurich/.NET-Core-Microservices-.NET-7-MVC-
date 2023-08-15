using Mango.Web.Models;
using Mango.Web.Service.ProductService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDTO>? list = new();

            ResponseDTO? response = await _productService.GetAllProductAsync();

            if (response != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<ProductDTO>>(response.Result.ToString());
            else
                TempData["error"] = response?.Message;

            return View(list);
        }

        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.CreateProductAsync(productDTO);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product created successfully.";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(productDTO);
        }

        public async Task<IActionResult> EditProduct(int productId)
        {
            ResponseDTO? response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                ProductDTO? ProductDTO = JsonConvert.DeserializeObject<ProductDTO>(response.Result.ToString());
                return View(ProductDTO);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.UpdateProductAsync(productDTO);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully.";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(productDTO);
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            ResponseDTO? response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                ProductDTO? ProductDTO = JsonConvert.DeserializeObject<ProductDTO>(response.Result.ToString());
                return View(ProductDTO);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(ProductDTO productDTO)
        {
            ResponseDTO? response = await _productService.DeleteProductAsync(productDTO.ProductId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully.";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View();
        }
    }
}
