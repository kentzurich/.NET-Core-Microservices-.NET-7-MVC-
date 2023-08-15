using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.OrderAPI.Services.Product
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
