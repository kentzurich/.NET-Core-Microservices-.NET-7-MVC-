using Mango.Web.Models;

namespace Mango.Web.Service.CouponService
{
	public interface ICouponService
	{
		Task<ResponseDTO?> GetCouponAsync(string couponCode); 
		Task<ResponseDTO?> GetAllCouponAsync();
		Task<ResponseDTO?> GetCouponByIdAsync(int couponId); 
		Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDTO); 
		Task<ResponseDTO?> UpdateCouponAsync(CouponDTO couponDTO); 
		Task<ResponseDTO?> DeleteCouponAsync(int couponId); 
	}
}
