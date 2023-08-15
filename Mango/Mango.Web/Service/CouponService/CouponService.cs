using Mango.Web.Models;
using Mango.Web.Service.BaseService;
using Mango.Web.Utility;

namespace Mango.Web.Service.CouponService
{
	public class CouponService : ICouponService
	{
		private readonly IBaseService _baseService;
		private readonly string APIName;

		public CouponService(IBaseService baseService)
        {
			_baseService = baseService;
			APIName = "CouponAPI";
		}

        public async Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDTO)
		{
			return await _baseService.SendAsync(
				new RequestDTO()
				{
					APIType = StaticDetails.APIType.POST,
					Data = couponDTO,
					Url = $"{StaticDetails.CouponAPIBase}/api/{APIName}"
				});
		}

		public async Task<ResponseDTO?> DeleteCouponAsync(int couponId)
		{
			return await _baseService.SendAsync(
				new RequestDTO()
				{
					APIType = StaticDetails.APIType.DELETE,
					Url = $"{StaticDetails.CouponAPIBase}/api/{APIName}/{couponId}"
				});
		}

		public async Task<ResponseDTO?> GetAllCouponAsync()
		{
			return await _baseService.SendAsync(
				new RequestDTO()
				{
					APIType = StaticDetails.APIType.GET,
					Url = $"{StaticDetails.CouponAPIBase}/api/{APIName}"
				});
		}

		public async Task<ResponseDTO?> GetCouponAsync(string couponCode)
		{
			return await _baseService.SendAsync(
				new RequestDTO()
				{
					APIType = StaticDetails.APIType.GET,
					Url = $"{StaticDetails.CouponAPIBase}/api/{APIName}/GetByCode/{couponCode}"
				});
		}

		public async Task<ResponseDTO?> GetCouponByIdAsync(int couponId)
		{
			return await _baseService.SendAsync(
				new RequestDTO()
				{
					APIType = StaticDetails.APIType.GET,
					Url = $"{StaticDetails.CouponAPIBase}/api/{APIName}/{couponId}"
				});
		}

		public async Task<ResponseDTO?> UpdateCouponAsync(CouponDTO couponDTO)
		{
			return await _baseService.SendAsync(
				new RequestDTO()
				{
					APIType = StaticDetails.APIType.PUT,
					Data = couponDTO,
					Url = $"{StaticDetails.CouponAPIBase}/api/{APIName}"
				});
		}
	}
}
