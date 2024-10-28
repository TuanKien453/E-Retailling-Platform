using Azure;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.GHNRequestModel;
using E_Retalling_Portal.Services;
using E_Retalling_Portal.Services.ExtendService;
using E_Retalling_Portal.Util;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace E_Retalling_Portal.Controllers.Home
{
    public class CheckOutController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly GHNService _ghnService;
        public CheckOutController(IVnPayService vnPayService,GHNService ghnService)
        {
            _vnPayService = vnPayService;
            _ghnService = ghnService;
        }
        public async Task<IActionResult> CreatePaymentUrl(String cartItems)
        {
            var piItems = new Dictionary<int, int>();
            var pItems = new Dictionary<int, int>();
			var items = cartItems.Split(';');
            using (var context = new Context()) {
				foreach (var item in items)
				{
                    
					if (item.Contains("PI"))
					{
						var parts = item.Split("PI:");
						var productItemId = int.Parse(parts[0]);
						var quantity = int.Parse(parts[1]);
						if (context.ProductItems.FirstOrDefault(pi=>pi.id == productItemId) != null)
                        {
							piItems[productItemId] = quantity;
						}

					}
					else if (item.Contains("P"))
					{
						var parts = item.Split("P:");
						var productId = int.Parse(parts[0]);
						var quantity = int.Parse(parts[1]);

						if (context.Products.FirstOrDefault(p => p.id == productId) != null)
						{
							pItems[productId] = quantity;
						}
						
					}
				}
			}
			var orderRequest = new OrderRequest
			{
				PaymentTypeId = 2,
				ServiceTypeId = 0,
				ServiceId = 53330,
				RequiredNote = "KHONGCHOXEMHANG", 
				FromName = "TinTest124",
				FromPhone = "0987654321",
				FromAddress = "72 Thành Thái, Phường 14, Quận 10, Hồ Chí Minh, Vietnam",
				FromWardName = "Phường 14",
				FromDistrictName = "Quận 10",
				FromProvinceName = "HCM",

				// Required fields for the recipient
				ToName = "TinTest124", 
				ToPhone = "0987654321",  
				ToAddress = "72 Thành Thái, Phường 14, Quận 10, Hồ Chí Minh, Vietnam",
				ToWardName = "Xã Vĩnh Viễn A",
				ToDistrictName = "Long Mỹ",
				ToProvinceName = "Hậu Giang",

				// Order details
				Weight = 200,  
				Length = 1,
				Width = 19,
				Height = 10,

				Items = new List<OrderItemRequest>
					{
						new OrderItemRequest
						{
							Name = "Áo Polo",
							Weight = 1200,
							Quantity =1
						}
					}
			};

			var a = await _ghnService.CreateShippingOrderAsync(orderRequest);
			return Ok(a);
			if (piItems.Count + pItems.Count == 0) {
                ViewBag.mess = "Choose your product to checkout";
                return RedirectToAction("Index","Cart");
            }

            foreach (var item in piItems) {
                
            }

			return Ok(new { pItems, piItems });
            //var url = _vnPayService.CreatePaymentUrl(HttpContext, "test","12346789");
            //return Redirect(url);
        }

        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            //payment success
            if (response.Success == true && response.VnPayResponseCode.Equals("00"))
            {
                return Json(response);
            }
            return Json(response);
        }
    }
}
