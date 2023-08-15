using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Service.OrderService
{
    public interface IOrderService
    {
        Task<ResponseDTO?> CreateOrderAsync(CartDTO cartDTO);
        Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDTO);
        Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDTO?> GetAllOrder(string? userId);
        Task<ResponseDTO?> GetOrder(int orderId);
        Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus);
    }
}
