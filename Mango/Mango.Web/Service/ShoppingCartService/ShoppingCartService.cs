using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.BaseService;
using Mango.Web.Utility;

namespace Mango.Web.Service.ShoppingCartService
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IBaseService _baseService;
        private readonly string APIName;

        public ShoppingCartService(IBaseService baseService)
        {
            _baseService = baseService;
            APIName = "CartAPI";
        }

        public async Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = cartDTO,
                    Url = $"{StaticDetails.ShoppingCartAPIBase}/api/{APIName}/ApplyCoupon"
                });
        }

        public async Task<ResponseDTO?> EmailCart(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = cartDTO,
                    Url = $"{StaticDetails.ShoppingCartAPIBase}/api/{APIName}/EmailCartRequest"
                });
        }

        public async Task<ResponseDTO?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(
                new RequestDTO()
                {
                    APIType = StaticDetails.APIType.GET,
                    Url = $"{StaticDetails.ShoppingCartAPIBase}/api/{APIName}/GetCart/{userId}"
                });
        }

        public async Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(
               new RequestDTO()
               {
                   APIType = StaticDetails.APIType.POST,
                   Data = cartDetailsId,
                   Url = $"{StaticDetails.ShoppingCartAPIBase}/api/{APIName}/RemoveCart"
               });
        }

        public async Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(
               new RequestDTO()
               {
                   APIType = StaticDetails.APIType.POST,
                   Data = cartDTO,
                   Url = $"{StaticDetails.ShoppingCartAPIBase}/api/{APIName}/CartUpsert"
               });
        }
    }
}
