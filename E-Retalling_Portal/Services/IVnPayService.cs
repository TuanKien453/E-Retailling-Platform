using E_Retalling_Portal.Models;

namespace E_Retalling_Portal.Services.ExtendService
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, String orderInfo);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
