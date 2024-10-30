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
        public IActionResult ViewShopChart(int year = 2023) // Default year if none is specified
        {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Product> products = context.Products.GetProductsByShop(shop.id).ToList();
                int[] months = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                double[] saleOnMonth = new double[months.Length];
                string[] monthsString = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

                for (int i = 0; i < months.Length; i++)
                {
                    List<Order> orderInMonth = context.Orders.GetOrderByYearMonth(year, months[i]).ToList();
                    foreach (Order order in orderInMonth)
                    {
                        List<OrderItem> orderItemsInMonth = context.OrderItems.GetOrderItemByOrderId(order.id).ToList();
                        saleOnMonth[i] += context.OrderItems.GetOrderItemPriceOnMonth(orderItemsInMonth);
                    }
                }

                // Create a dictionary with month names and sales data
                Dictionary<string, double> SaleOnYear = new Dictionary<string, double>();
                for (int i = 0; i < months.Length; i++)
                {
                    SaleOnYear[monthsString[i]] = saleOnMonth[i];
                }

                // Pass data to ViewData
                ViewData["ChartData"] = SaleOnYear;
                ViewData["SelectedYear"] = year;
            }

            return View("/Views/SellerShopManager/ShopChart/ViewShopChart.cshtml");
        }
    }
}
