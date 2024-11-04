using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models.ChartModel;
using System.Collections.Generic;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;

namespace E_Retalling_Portal.Controllers.SellerShopManager
{
    public class ShopChartController : Controller
    {
        public IActionResult ViewShopChart()
        {           
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
                List<Product> products = context.Products.GetProductsByShop(shop.id).ToList();
               
                double[] data = new double[monthsString.Length];
                double[] average = new double[monthsString.Length];
                double[] other = new double[monthsString.Length];
                double countSale = 0;
                double countAverage = 0;
                double countBreak = 0;

                int year = DateTime.ParseExact(yearString.Trim(), "yyyy", null).Year;
                for (int i = 0; i < monthsString.Length; i++)
                {
                    countSale = 0;
                    countAverage = 0;
                    countBreak = 0;
                    int day = i + 1;
                    int month = DateTime.ParseExact(monthsString[i].Trim(), "MMMM", null).Month;
                    List<Order> orderInMonth = context.Orders.GetOrderByYearMonth(year, month).ToList();

                    foreach (var order in orderInMonth)
                    {

                        List<OrderItem> orderItems = context.OrderItems.GetOrderItemByOrderId(order.id).ToList();
                        foreach (var item in orderItems)
                        {
                            Console.WriteLine($"item = {item.shippingStatus}, {item.quanity}, {item.externalOrderCode} ");
                            if (item.shippingStatus.ToString().Equals("delivered", StringComparison.OrdinalIgnoreCase))
                            {
                                if (products.Contains(context.Products.GetProductById(item.productId).FirstOrDefault()))
                                {
                                    double today = item.quanity * item.price;
                                    countBreak += today * item.transactionFee + item.shippingFee;
                                    countSale += today;
                                    countAverage += countSale + countBreak;
                                }
                            }

                        }
                        data[i] = countSale;
                        other[i] = countBreak;
                        average[i] = countAverage;
                    }


                }


                var labels = monthsString;
                var sales = data;
                var others = other;
                var averages = average;
                return Json(new { labels, sales, others, averages });
            }

        }

    }



}
