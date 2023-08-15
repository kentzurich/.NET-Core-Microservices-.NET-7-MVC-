using Mango.Web.Models;
using Mango.Web.Service.OrderService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        public IActionResult OrderIndex()
        {
            return View();
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDTO? orderHeaderDTO = new();
            string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
                orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(response.Result.ToString());

            if (!User.IsInRole(StaticDetails.RoleAdmin) && userId != orderHeaderDTO.UserId)
                return NotFound();

            return View(orderHeaderDTO);
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_ReadyForPickup);
            if (response != null && response.IsSuccess)
                TempData["success"] = "Order status successfully updated.";
            else
                TempData["error"] = response.Message;

            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Completed);
            if (response != null && response.IsSuccess)
                TempData["success"] = "Order status successfully updated.";
            else
                TempData["error"] = response.Message;

            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Cancelled);
            if (response != null && response.IsSuccess)
                TempData["success"] = "Order status successfully updated.";
            else
                TempData["error"] = response.Message;

            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
         {
            IEnumerable<OrderHeaderDTO>? orderList;
            string userId = "";
            if(!User.IsInRole(StaticDetails.RoleAdmin))
                userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponseDTO? response = await _orderService.GetAllOrder(userId);
            if(response != null && response.IsSuccess)
            {
                orderList = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(response.Result.ToString());
                switch(status)
                {
                    case "approved":
                        orderList = orderList.Where(x => x.Status == StaticDetails.Status_Approved);
                        break;
                    case "readyforpickup":
                        orderList = orderList.Where(x => x.Status == StaticDetails.Status_ReadyForPickup);
                        break;
                    case "cancelled":
                        orderList = orderList.Where(x => x.Status == StaticDetails.Status_Cancelled);
                        break;
                    default: 
                        break;
                }
            }
            else
            {
                orderList = new List<OrderHeaderDTO>();
            }
            return Json(new { data = orderList });
        }
    }
}
