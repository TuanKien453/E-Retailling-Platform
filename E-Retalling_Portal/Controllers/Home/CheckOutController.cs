using Azure;
using E_Retalling_Portal.Services.ExtendService;
using E_Retalling_Portal.Util;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.Home
{
    public class CheckOutController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public CheckOutController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }
        public IActionResult CreatePaymentUrl()
        {
            Dictionary<String,int> cartItem = CookiesUtils.GetCartItems(Request);
            var piItems = new Dictionary<string, int>();
            var pItems = new Dictionary<string, int>();
            foreach (var item in cartItem)
            {
                Console.WriteLine(item.Key);
                if (item.Key.EndsWith("C"))
                {
                    if (item.Key.Contains("PI"))
                    {
                        piItems[item.Key] = item.Value;
                    }
                    else if (item.Key.Contains("P"))
                    {
                        pItems[item.Key] = item.Value;
                    }

                }
            }
            if (piItems.Count == 0 || pItems.Count == 0) {
                return RedirectToAction("Index", "Cart");
            }
            foreach (var item in piItems) { 
            
            }
            foreach (var item in pItems) {
            
            }
            Console.WriteLine("PI Items: " + string.Join(", ", piItems));
            Console.WriteLine("P Items: " + string.Join(", ", pItems));
            return Ok("");
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
