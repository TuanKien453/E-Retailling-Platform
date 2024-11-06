using E_Retalling_Portal.Services.ExtendService;
using E_Retalling_Portal.Services;
using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.GHNResponseModel;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using System.Transactions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace E_Retalling_Portal.Controllers.SellerShopManager
{
	public class OrderController : Controller
	{
		private readonly GHNService _ghnService;
		public OrderController(GHNService ghnService)
		{
			_ghnService = ghnService;
		}
		public async Task<IActionResult> OrderInfoAsync()
		{
			using (var context = new Context())
			{
                int? accountId = (int)HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                Account account = context.Accounts.GetAccountByAccountId(accountId.Value).FirstOrDefault();
				var shop = context.Shops.GetShopbyAccId(accountId.Value).FirstOrDefault();
                var orderItemList = context.OrderItems.GetOrderItemByUserId(shop.id).ToList();
				var orderInfoResponses = new List<OrderInfoResponse>();
				foreach (var orderItem in orderItemList)
				{
					if (orderItem.externalOrderCode != null)
					{
						var orderInfoResponse = await _ghnService.GetOrderInfoAsync(orderItem.externalOrderCode);
						orderInfoResponses.Add(orderInfoResponse);
					}
				}
				var joinedData = GetDataFromOrsedItemAndOrderInfoResponse(orderItemList, orderInfoResponses);
				foreach(var item in joinedData)
				{
                    Console.WriteLine(item.ShippingStatus);
				}
				ViewBag.OrderInfo = joinedData;
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
				var orderInfoResponses = new List<OrderInfoResponse>();
				foreach (var orderItem in orderItemList)
				{
					if (orderItem.externalOrderCode != null)
					{
						var orderInfoResponse = await _ghnService.GetOrderInfoAsync(orderItem.externalOrderCode);
						orderInfoResponses.Add(orderInfoResponse);
					}
				}
                var joinedData = GetDataFromOrsedItemAndOrderInfoResponse(orderItemList, orderInfoResponses);
				foreach (var item in joinedData)
				{
                    Console.WriteLine(item.Product.price);
				}
                ViewBag.OrderList = joinedData;
				return View("/Views/SellerShopManager/Order/ViewOrderList.cshtml");
			}
		}
        private IEnumerable<dynamic> GetDataFromOrsedItemAndOrderInfoResponse(List<OrderItem> orderItemList, List<OrderInfoResponse> orderInfoResponses)
        {
            var joinedData = orderInfoResponses
                            .Join(orderItemList,
                            orderInfo => orderInfo.Data.OrderCode,
                            orderItem => orderItem.externalOrderCode,
                            (orderInfo, orderItem) => new
                            {
                                OrderCode = orderInfo.Data.OrderCode,
                                OrderMessage = orderInfo.Message,
                                ItemId = orderItem.id,
                                ToName = orderInfo.Data.ToName,
                                ToPhone = orderInfo.Data.ToPhone,
                                ToAddress = orderInfo.Data.ToAddress,
                                Content = orderInfo.Data.Content,
                                RequiredNote = orderInfo.Data.RequiredNote,
                                ProductId = orderItem.productId,
                                Quantity = orderItem.quantity,
                                ShippingFee = orderItem.shippingFee,
                                TransactionFee = orderItem.transactionFee * orderItem.price / 100,
                                ShippingStatus = orderItem.shippingStatus,
                                Status = orderInfo.Data.Status,
                                Weight = orderInfo.Data.Weight,
                                Price = orderItem.price,
								Product = orderItem.product,
								ProductItem = orderItem.productItem,
								ProductItemId = orderItem.productItemId,
								OrderItem = orderItem
                            });
            return joinedData;
        }
    }
}
