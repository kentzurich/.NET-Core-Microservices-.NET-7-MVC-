using Mango.Services.ShoppingCartAPI.Models.DTO;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Services.Coupon
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CouponDTO> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"api/CouponAPI/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);

            if (resp.IsSuccess)
                return JsonConvert.DeserializeObject<CouponDTO>(resp.Result.ToString());

            return new CouponDTO();
        }
    }
}
