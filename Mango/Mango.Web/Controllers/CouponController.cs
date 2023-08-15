using Mango.Web.Models;
using Mango.Web.Service.CouponService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mango.Web.Controllers
{
	public class CouponController : Controller
	{
		private readonly ICouponService _couponService;
		
		public CouponController(ICouponService couponService)
        {
			_couponService = couponService;
		}

        public async Task<IActionResult> CouponIndex()
		{
			List<CouponDTO>? list = new();

			ResponseDTO? response = await _couponService.GetAllCouponAsync();

            if (response != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<CouponDTO>>(response.Result.ToString());
            else
                TempData["error"] = response?.Message;

			return View(list);
		}

		public async Task<IActionResult> CreateCoupon()
		{
			return View();
		}

		[HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDTO couponDTO)
        {
			if(ModelState.IsValid)
			{
                ResponseDTO? response = await _couponService.CreateCouponAsync(couponDTO);

				if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon created successfully.";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(couponDTO);
        }

        public async Task<IActionResult> DeleteCoupon(int couponId)
        {
            ResponseDTO? response = await _couponService.GetCouponByIdAsync(couponId);

            if (response != null && response.IsSuccess)
            {
                CouponDTO? couponDTO = JsonConvert.DeserializeObject<CouponDTO>(response.Result.ToString());
                return View(couponDTO);
            }
            else
            {
                TempData["error"] = response?.Message;
            }  

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(CouponDTO couponDTO)
        {
            ResponseDTO? response = await _couponService.DeleteCouponAsync(couponDTO.CouponId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon deleted successfully.";
                return RedirectToAction(nameof(CouponIndex));
            }  
            else
            {
                TempData["error"] = response?.Message;
            }

            return View();
        }
    }
}
