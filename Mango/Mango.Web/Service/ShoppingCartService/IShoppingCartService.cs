using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Service.ShoppingCartService
{
    public interface IShoppingCartService
    {
        Task<ResponseDTO?> GetCartByUserIdAsync(string userId);
        Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO);
        Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO);
        Task<ResponseDTO?> EmailCart(CartDTO cartDTO);

    }
}
