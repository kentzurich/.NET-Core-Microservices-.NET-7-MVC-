using Mango.Service.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync("api/ProductAPI");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);

            if(resp.IsSuccess)
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(resp.Result.ToString());

            return new List<ProductDTO>();
        }
    }
}
