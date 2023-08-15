using Mango.Web.Models;

namespace Mango.Web.Service.ProductService
{
    public interface IProductService
    {
        Task<ResponseDTO?> GetAllProductAsync();
        Task<ResponseDTO?> GetProductByIdAsync(int productId);
        Task<ResponseDTO?> CreateProductAsync(ProductDTO productDTO);
        Task<ResponseDTO?> UpdateProductAsync(ProductDTO productDTO);
        Task<ResponseDTO?> DeleteProductAsync(int productId);
    }
}
