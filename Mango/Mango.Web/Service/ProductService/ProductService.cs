using Mango.Web.Models;
using Mango.Web.Service.BaseService;
using Mango.Web.Utility;

namespace Mango.Web.Service.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        private readonly string APIName;

        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
            APIName = "ProductAPI";
        }

        public async Task<ResponseDTO?> CreateProductAsync(ProductDTO ProductDTO)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = ProductDTO,
                    Url = $"{StaticDetails.ProductAPIBase}/api/{APIName}",
                    ContentType = StaticDetails.ContentType.MultipartFormData
                });
        }

        public async Task<ResponseDTO?> DeleteProductAsync(int ProductId)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.DELETE,
                    Url = $"{StaticDetails.ProductAPIBase}/api/{APIName}/{ProductId}"
                });
        }

        public async Task<ResponseDTO?> GetAllProductAsync()
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.GET,
                    Url = $"{StaticDetails.ProductAPIBase}/api/{APIName}"
                });
        }

        public async Task<ResponseDTO?> GetProductAsync(string ProductCode)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.GET,
                    Url = $"{StaticDetails.ProductAPIBase}/api/{APIName}/{ProductCode}"
                });
        }

        public async Task<ResponseDTO?> GetProductByIdAsync(int ProductId)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.GET,
                    Url = $"{StaticDetails.ProductAPIBase}/api/{APIName}/{ProductId}"
                });
        }

        public async Task<ResponseDTO?> UpdateProductAsync(ProductDTO ProductDTO)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.PUT,
                    Data = ProductDTO,
                    Url = $"{StaticDetails.ProductAPIBase}/api/{APIName}",
                    ContentType = StaticDetails.ContentType.MultipartFormData
                });
        }
    }
}
