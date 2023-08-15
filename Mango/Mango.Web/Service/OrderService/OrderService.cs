using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.BaseService;
using Mango.Web.Utility;

namespace Mango.Web.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        private readonly string APIName;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
            APIName = "OrderAPI";
        }

        public async Task<ResponseDTO?> CreateOrderAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(
                new RequestDTO
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = cartDTO,
                    Url = $"{StaticDetails.OrderAPIBase}/api/{APIName}/CreateOrder"
                });
        }

        public async Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDTO)
        {
            return await _baseService.SendAsync(
                new RequestDTO
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = stripeRequestDTO,
                    Url = $"{StaticDetails.OrderAPIBase}/api/{APIName}/CreateStripeSession"
                });
        }

        public async Task<ResponseDTO?> GetAllOrder(string? userId)
        {
            return await _baseService.SendAsync(
                new RequestDTO
                {
                    APIType = StaticDetails.APIType.GET,
                    Url = $"{StaticDetails.OrderAPIBase}/api/{APIName}/GetOrders/{userId}"
                });
        }

        public async Task<ResponseDTO?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(
                new RequestDTO
                {
                    APIType = StaticDetails.APIType.GET,
                    Url = $"{StaticDetails.OrderAPIBase}/api/{APIName}/GetOrder/{orderId}"
                });
        }

        public async Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(
                new RequestDTO
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = newStatus,
                    Url = $"{StaticDetails.OrderAPIBase}/api/{APIName}/UpdateOrderStatus/{orderId}"
                });
        }

        public async Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(
                new RequestDTO
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = orderHeaderId,
                    Url = $"{StaticDetails.OrderAPIBase}/api/{APIName}/ValidateStripeSession"
                });
        }
    }
}
