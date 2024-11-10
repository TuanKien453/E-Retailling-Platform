using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models.ChartModel;
using System.Collections.Generic;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using Microsoft.IdentityModel.Tokens;
using E_Retalling_Portal.Controllers.Filter;

namespace E_Retalling_Portal.Controllers.SellerShopManager
{
    public class ShopChartController : Controller
    {
        [TypeFilter(typeof(ShopOwnerRoleFilter))]
        [TypeFilter(typeof(HaveShopFilter))]
        public IActionResult ViewShopChart()
        {
            var context = new Context();
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
            DateTime now = DateTime.Now;
            int currentYear = now.Year;
            DateTime createAt = DateTime.Parse(shop.createdAt);
            int createYear = createAt.Year;
            List<int> year = new List<int>();
            if (currentYear - createYear >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    year.Add(currentYear - i);
                }
            }
            else
            {
                for (int i = 0; i < currentYear - createYear; i++)
                {
                    year.Add(currentYear - i);
                }
                if (year.IsNullOrEmpty())
                {
                    year.Add(createYear);
                }
            }
            ViewBag.years = year;
            return View("/Views/SellerShopManager/ShopChart/ViewShopChart.cshtml");
        }

        [HttpGet]
        public IActionResult LoadChartData(string yearString)
        {
            string[] monthsString = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            if (string.IsNullOrEmpty(yearString))
                return Json(new { error = "Invalid year" });

            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Product> products = context.Products.GetProductsByShopNoNull(shop.id).ToList();
               
                double[] data = new double[monthsString.Length];
                double[] average = new double[monthsString.Length];
                double[] other = new double[monthsString.Length];
                double countSale = 0;
                double countFee = 0;

                int year = DateTime.ParseExact(yearString.Trim(), "yyyy", null).Year;
                for (int i = 0; i < monthsString.Length; i++)
                {
                    countSale = 0;
                    countFee = 0;
                    int day = i + 1;
                    int month = DateTime.ParseExact(monthsString[i].Trim(), "MMMM", null).Month;
                    List<Order> orderInMonth = context.Orders.GetOrderByYearMonth(year, month).ToList();

                    foreach (var order in orderInMonth)
                    {
                        List<OrderItem> orderItems = context.OrderItems.GetOrderItemByOrderId(order.id).ToList();
                        foreach (var item in orderItems)
                        {
                            countFee = 0;
                            if (item.shippingStatus.ToString().Equals("delivered", StringComparison.OrdinalIgnoreCase))
                            {
                                if (products.Contains(context.Products.GetProductByIdNoNull(item.productId).FirstOrDefault()))
                                {
                                    double today = item.price;
                                    countFee += today * item.transactionFee / 100;
                                    countSale += today - countFee;
                                    Console.WriteLine($"don = {today}, fee = {countFee}, sale = {countSale}");
                                }
                            }

                        }
                        data[i] = countSale;
                    }

                }
                var labels = monthsString;
                var sales = data;
                return Json(new { labels, sales});
            }

        }

    }



}
