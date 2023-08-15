using Mango.Service.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI.Services.Product
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
