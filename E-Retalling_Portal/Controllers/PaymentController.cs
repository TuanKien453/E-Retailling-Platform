using E_Retalling_Portal.Models;
using E_Retalling_Portal.Services;
using E_Retalling_Portal.Services.ExtendService;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace E_Retalling_Portal.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }
        public IActionResult CreatePaymentUrl( )
        {
            var url = _vnPayService.CreatePaymentUrl(HttpContext, "test");

            return Redirect(url);
        }

        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            //payment success
            if (response.Success == true && response.VnPayResponseCode.Equals("00")) {
                return Json(response);
            }
            return Json(response);
        }
    }
}
