using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers.Feedback
{
    [TypeFilter(typeof(CustomerFilter))]
    public class FeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View("Views/Home/Index.cshtml");
        }

        public IActionResult FeedbackProcess(int orderItemId,  string comment, int star)
        {
            if(comment.Length > 200)
            {
                return RedirectToAction("Error500", "Home");
            }
            using(var context = new Context())
            {
                var orderItem = context.OrderItems.GetOrderItemByOrderItemId(orderItemId).FirstOrDefault();
                if (orderItem != null)
                {
                    orderItem.comment = comment;
                    orderItem.rating = star;
                    context.Update(orderItem);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("ViewOrderList","Order");
        }
    }
}
