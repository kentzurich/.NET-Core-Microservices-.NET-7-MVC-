using AutoMapper;
using Mango.MessageBus;
using Mango.Service.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Services.Coupon;
using Mango.Services.ShoppingCartAPI.Services.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _config;
        private ResponseDTO _response;

        public CartAPIController(AppDbContext db, 
                                 IMapper mapper, 
                                 IProductService productService,
                                 ICouponService couponService,
                                 IMessageBus messageBus,
                                 IConfiguration config)
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _config = config;
            _response = new ResponseDTO();
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDTO> CartUpsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == cartDTO.CartHeader.UserId);
                if(cartHeaderFromDb == null)
                {
                    //create create header and details

                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    await _db.CartHeaders.AddAsync(cartHeader);
                    await _db.SaveChangesAsync();

                    await UpsertCartDetailsAsync(cartDTO, cartHeader);
                }
                else
                {
                    //if header is not null
                    //check if details has same product

                    var cartDetailsFromDb = await _db.CartDetails
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => 
                        x.ProductId == cartDTO.CartDetails.First().ProductId &&
                        x.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if(cartDetailsFromDb == null)
                    {
                        //create cart details
                        await UpsertCartDetailsAsync(cartDTO, cartHeaderFromDb);
                    }
                    else
                    {
                        //update cart details
                        await UpsertCartDetailsAsync(cartDTO, cartDetails: cartDetailsFromDb, isCreate: false);
                    }
                }
                _response.Result = cartDTO;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }

            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDTO> ApplyCoupon(CartDTO cartDTO)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(x => x.UserId == cartDTO.CartHeader.UserId);
                cartFromDb.CouponCode = cartDTO.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }

            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart([FromBody]int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails
                    .FirstAsync(x => x.CartDetailsId == cartDetailsId);

                int totalCartCount = _db.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);

                if(totalCartCount == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                        .FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                _response.Result = false;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }

            return _response;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartDTO cartDTO = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(await _db.CartHeaders.FirstAsync(x => x.UserId == userId))
                };
                cartDTO.CartDetails = _mapper
                    .Map<IEnumerable<CartDetailsDTO>>(_db
                    .CartDetails
                    .Where(x => x.CartHeaderId ==  cartDTO.CartHeader.CartHeaderId));

                IEnumerable<ProductDTO> productDTO = await _productService.GetProducts();

                foreach (var cart in cartDTO.CartDetails)
                {
                    cart.Product = productDTO.FirstOrDefault(x => x.ProductId == cart.ProductId);
                    cartDTO.CartHeader.CartTotal += (cart.Count * cart.Product.Price);
                }

                //apply coupon if any
                if(!string.IsNullOrEmpty(cartDTO.CartHeader.CouponCode))
                {
                    CouponDTO couponDTO = await _couponService.GetCoupon(cartDTO.CartHeader.CouponCode);
                    if(couponDTO != null &&  cartDTO.CartHeader.CartTotal > couponDTO.MinAmount)
                    {
                        cartDTO.CartHeader.CartTotal -= couponDTO.DiscountAmount;
                        cartDTO.CartHeader.Discount = couponDTO.DiscountAmount;
                    }
                }

                _response.Result = cartDTO;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }

            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDTO> EmailCartRequest([FromBody] CartDTO cartDTO)
        {
            try
            {
                await _messageBus.PublishMessage(cartDTO, _config.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }

            return _response;
        }

        private async Task UpsertCartDetailsAsync(CartDTO cartDTO, 
                                                  CartHeader? cartHeader = null, 
                                                  CartDetails? cartDetails = null,
                                                  bool isCreate = true)
        {
            if(isCreate)
            {
                cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                await _db.CartDetails.AddAsync(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                await _db.SaveChangesAsync();
            }
            else
            {
                cartDTO.CartDetails.First().Count += cartDetails.Count;
                cartDTO.CartDetails.First().CartHeaderId = cartDetails.CartHeaderId;
                cartDTO.CartDetails.First().CartDetailsId = cartDetails.CartDetailsId;
                _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                await _db.SaveChangesAsync();
            }
        }
    }
}
