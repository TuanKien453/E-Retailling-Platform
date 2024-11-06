using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Policy;

namespace E_Retalling_Portal.Controllers.ShopManager
{
    public class ShopDashBoardController : Controller
    {
        public IActionResult Index()
        {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                //Get total product
                List<Product> products = context.Products.GetProductsByShop(shop.id).ToList();
                int totalProduct = products.Count;
                //Get stock product
                int stockProduct = context.Products.GetTotalStockProduct(shop.id);
                //Get Sale of Product
                int saleProducts = context.Products.GetNumberAllSalesProduct(products);
                //Get active day
                string createTime = shop.createdAt;
                DateTime creationDate = DateTime.Parse(createTime);
                DateTime currentDate = DateTime.Now;
                int activeDays = (currentDate - creationDate).Days;
                //Get All categories
                int category = context.Categories.GetAllNumberOfCategoriesByShop(shop.id);
                //Get All Order
                int order = context.Orders.GetAllNumberOfOrderByShop(shop.id);
                //Get all 
                int users = context.Users.GetAllNumberPeopeOrderByShop(shop.id);
                ViewBag.TotalProduct = totalProduct;
                ViewBag.StockProduct = stockProduct;
                ViewBag.SaleProducts = saleProducts;
                ViewBag.activeDays = activeDays;
                ViewBag.Category = category;
                ViewBag.Order = order;
                ViewBag.Users = users;

                return View("/Views/SellerShopManager/ShopDashBoard/Index.cshtml");
            }

        }

        [HttpGet]
        public IActionResult LoadChartData(string monthName)
        {
            if (string.IsNullOrEmpty(monthName))
                return Json(new { error = "Invalid month" });

            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Product> products = context.Products.GetProductsByShop(shop.id).ToList();
                int month;
                try
                {
                    month = DateTime.ParseExact(monthName.Trim(), "MMMM", null).Month;
                }
                catch
                {
                    return Json(new { error = "Invalid month name." });
                }
                int year = DateTime.Now.Year;
                int daysInMonth = DateTime.DaysInMonth(year, month);
                double[] data = new double[daysInMonth];
                double[] average = new double[daysInMonth];
                double[] other = new double[daysInMonth];
                double countSale = 0;
                double countAverage = 0;
                double countBreak = 0;
                double countFee = 0;
               
                
                for (int i = 0; i < daysInMonth; i++)
                {
                    countSale = 0;
                    countAverage = 0;
                    countBreak = 0;
                    countFee = 0;
                    int day = i + 1;
                    List<Order> orderInMonth = context.Orders.GetOrderByYearMonthDay(year, month, day).ToList();
                    foreach (var order in orderInMonth)
                    {                   
                        List<OrderItem> orderItems = context.OrderItems.GetOrderItemByOrderId(order.id).ToList();
                        foreach (var item in orderItems)
                        {
                            if (item.shippingStatus.ToString().Equals("delivered", StringComparison.OrdinalIgnoreCase))
                            {                               
                                if (products.Contains(context.Products.GetProductById(item.productId).FirstOrDefault()))
                                {
                                    double today = item.quantity * item.price;
                                    countFee += today * item.transactionFee / 100;
                                    countBreak += today * item.transactionFee/100 + (double)item.shippingFee / 1000;
                                   countSale += today;
                                    
                                }
                            }
                            
                        }
                        countAverage += countSale + countBreak;
                        data[i] = countSale;
                        other[i] = countFee;
                        average[i] = countAverage;
                    }
                   

                }

                    int[] days = new int[daysInMonth];
                for (int i = 1; i <= daysInMonth; i++)
                {
                    days[i-1] = i;
                }

                var labels = days;
                var sales = data;
                var others = other;
                var averages = average;
                return Json(new { labels, sales, others, averages });
            }

        }
    }
}
