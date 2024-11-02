using E_Retalling_Portal.Services.ExtendService;
using E_Retalling_Portal.Services;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.GHNResponseModel;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;

namespace E_Retalling_Portal.Controllers.SellerShopManager
{
    public class OrderController : Controller
    {
        private readonly GHNService _ghnService;
        public OrderController( GHNService ghnService)
        {
            _ghnService = ghnService;
        }
        public async Task<IActionResult> OrderInfoAsync()
        {
            using(var context = new Context())
            {
                var orderItemList = context.OrderItems.ToList();
                var orderInfoResponses = new List<OrderInfoResponse>();
                foreach (var orderItem in orderItemList)
                {
                    if (orderItem.externalOrderCode != null)
                    {
                        var orderInfoResponse = await _ghnService.GetOrderInfoAsync(orderItem.externalOrderCode);
                        orderInfoResponses.Add(orderInfoResponse);
                    }
                }
                ViewBag.OrderInfo = orderInfoResponses;
            }
            return View("/Views/SellerShopManager/Order/OrderInfo.cshtml");
        }
        public async Task<IActionResult> ViewOrderList()
        {
            using (var context = new Context())
            {
                int? accountId = (int)HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                Account account = context.Accounts.GetAccountByAccountId(accountId.Value).FirstOrDefault();
                User user = context.Users.GetUserByUserIdInAccount(account.userId).FirstOrDefault();
                var orderItemList = context.OrderItems.GetOrderItemByUserId(user.id).ToList();
                ViewBag.OrderList = orderItemList;
                foreach (var item in orderItemList)
                {
                    if(item.productItemId == null)
                    {
                        Console.WriteLine(item.product.shop.name);
                        Console.WriteLine(item.product.coverImage.imageName);
                        Console.WriteLine(item.product.name);
                        Console.WriteLine(item.price);
                    }
                }
                return View("/Views/SellerShopManager/Order/ViewOrderList.cshtml");
            }
        }
    }
}
