using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controller
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CouponAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly IMapper _mapper;
		private ResponseDTO _response;

		public CouponAPIController(AppDbContext db, IMapper mapper)
        {
			_db = db;
			_mapper = mapper;
			_response = new ResponseDTO();
		}

		[HttpGet]
		public ResponseDTO Get()
		{
			try
			{
				IEnumerable<Coupon> objList = _db.Coupons.ToList();
				_response.Result = _mapper.Map<IEnumerable<CouponDTO>>(objList);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}

		[HttpGet]
		[Route("{couponId:int}")]
		public ResponseDTO Get(int couponId)
		{
			try
			{
				Coupon obj = _db.Coupons.First(x => x.CouponId == couponId);
				_response.Result = _mapper.Map<CouponDTO>(obj);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}

		[HttpGet]
		[Route("GetByCode/{couponCode}")]
		public ResponseDTO GetByCode(string couponCode)
		{
			try
			{
				Coupon obj = _db.Coupons.First(x => x.CouponCode.ToLower() == couponCode.ToLower());
				_response.Result = _mapper.Map<CouponDTO>(obj);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}

		[HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDTO Post([FromBody] CouponDTO model)
		{
			try
			{
                Coupon obj = _mapper.Map<Coupon>(model);

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(model.DiscountAmount * 100),
                    Name = model.CouponCode,
                    Currency = "usd",
                    Id = model.CouponCode
                };
                var service = new Stripe.CouponService();
                service.Create(options);

				_db.Coupons.Add(obj);
				_db.SaveChanges();

                _response.Result = _mapper.Map<CouponDTO>(obj);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}

		[HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDTO Put([FromBody] CouponDTO model)
		{
			try
			{
				Coupon obj = _mapper.Map<Coupon>(model);
				_db.Coupons.Update(obj);
				_db.SaveChanges();

				_response.Result = _mapper.Map<CouponDTO>(obj);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}

		[HttpDelete]
        [Route("{couponId:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDTO Delete(int couponId)
		{
			try
			{
				Coupon obj = _db.Coupons.First(x => x.CouponId == couponId);

                var service = new Stripe.CouponService();
                service.Delete(obj.CouponCode);

                _db.Coupons.Remove(obj);
				_db.SaveChanges();
            }
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}
	}
}
