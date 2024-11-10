using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Cryptography;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Security.Policy;

namespace E_Retalling_Portal.Controllers.ShopManager
{
    public class ShopDashBoardController : Controller
    {
        [TypeFilter(typeof(ShopOwnerRoleFilter))]
        [TypeFilter(typeof(HaveShopFilter))]
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

        public IActionResult ViewDetailSaleProduct()
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            if (accId == null)
            {
                return View("Views/Shared/ErrorPage/Error500.cshtml");

            }

            using (var context = new Context())
            {
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Product> products = context.Products.GetProductsByShopNoNull(shop.id).ToList();
                List<Product> saleProducts = context.Products.GetAllSalesProduct(products);
                ViewBag.products = saleProducts;
            }

            return View("/Views/SellerShopManager/ShopDashBoard/ViewDetailSaleProduct.cshtml");
        }

        public IActionResult DetailProductProcess(int productId)
        {
            ViewBag.ProductId = productId;
            return View("/Views/SellerShopManager/ShopDashBoard/ChartDetailSaleProduct.cshtml");
        }
        public IActionResult LoadDetailSaleProductData(DateTime startTime, DateTime endTime, int productId)
        {
            using (var context = new Context())
            {
                Product product = context.Products.GetProductByIdNoNull(productId).FirstOrDefault();
                if (product == null)
                {
                    return View("Views/Shared/ErrorPage/Error500.cshtml");
                }
                List<Order> allOrders = context.Orders.ToList();

                // Filter orders in memory based on parsed `createTime`
                List<Order> ordersFromDate = allOrders
                    .Where(order =>
                    {
                        DateTime orderDate;
                        bool isParsed = DateTime.TryParseExact(
                            order.createTime,
                            "yyyyMMddHHmmss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out orderDate
                        );
                        return isParsed && orderDate >= startTime && orderDate <= endTime;
                    })
                    .ToList();
                Dictionary<int, int> productFromDay = new Dictionary<int, int>();
                if (product.isVariation)
                {

                    List<ProductItem> productItems = context.ProductItems.GetProductItemNoNull(productId).ToList();
                    foreach (var order in ordersFromDate)
                    {
                        List<OrderItem> orderItems = context.OrderItems.GetOrderItemByOrderId(order.id).ToList();
                        foreach (var item in orderItems)
                        {
                            int productItemId = item.productItemId ?? 0;
                            if (productItemId == 0)
                            {
                                break;
                            }
                            var productItem = context.ProductItems.GetProductItemByProductItemIdNoNull(productItemId).FirstOrDefault();
                            if (productItem != null && productItems.Contains(productItem))
                            {
                                if (productFromDay.ContainsKey(productItemId))
                                {
                                    productFromDay[productItemId] += item.quantity;
                                }
                                else
                                {
                                    productFromDay.Add(productItemId, item.quantity);
                                }

                            }
                        }

                    }
                    var productData = productFromDay.Select(item => new
                    {
                        ProductName = context.ProductItems.GetProductItemByProductItemIdNoNull(item.Key).FirstOrDefault()?.attribute,
                        Quantity = item.Value
                    }).ToList();

                    return Json(new
                    {
                        productNames = productData.Select(p => p.ProductName).ToArray(),
                        productQuantities = productData.Select(p => p.Quantity).ToArray()
                    });
                }
                else
                {
                    int count = 0;
                    foreach (var order in ordersFromDate)
                    {
                        List<OrderItem> orderItems = context.OrderItems.GetOrderItemByOrderId(order.id).ToList();
                        foreach (var item in orderItems)
                        {
                            if (item.shippingStatus.ToString().Equals("delivered", StringComparison.OrdinalIgnoreCase))
                            {
                                if (product.id == item.productId)
                                {
                                    count += item.quantity;
                                }
                            }
                        }
                    }
                    return Json(new
                    {
                        productNames = new string[] { product.name },
                        productQuantities = new int[] { count }
                    });
                }
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
                List<Product> products = context.Products.GetProductsByShopNoNull(shop.id).ToList();
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
                double countFee = 0;


                for (int i = 0; i < daysInMonth; i++)
                {
                    countSale = 0;
                    countFee = 0;
                    int day = i + 1;
                    List<Order> orderInMonth = context.Orders.GetOrderByYearMonthDay(year, month, day).ToList();
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
                                    double today = item.quantity * item.price;
                                    countFee += today * item.transactionFee / 100;
                                    countSale += today - countFee;

                                }
                            }

                        }
                        data[i] = countSale;
                    }


                }

                int[] days = new int[daysInMonth];
                for (int i = 1; i <= daysInMonth; i++)
                {
                    days[i - 1] = i;
                }

                var labels = days;
                var sales = data;
                return Json(new { labels, sales });
            }

        }

        public IActionResult LoadProductDataFromDay(DateTime startTime, DateTime endTime)
        {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();

                List<Product> products = context.Products.GetProductsByShopNoNull(shop.id).ToList();

                // Fetch orders without date filtering
                List<Order> allOrders = context.Orders.ToList();

                // Filter orders in memory based on parsed `createTime`
                List<Order> ordersFromDate = allOrders
                    .Where(order =>
                    {
                        DateTime orderDate;
                        bool isParsed = DateTime.TryParseExact(
                            order.createTime,
                            "yyyyMMddHHmmss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out orderDate
                        );
                        return isParsed && orderDate >= startTime && orderDate <= endTime;
                    })
                    .ToList();

                Dictionary<int, int> productFromDay = new Dictionary<int, int>();

                foreach (var order in ordersFromDate)
                {
                    List<OrderItem> orderItems = context.OrderItems.GetOrderItemByOrderId(order.id).ToList();
                    foreach (var item in orderItems)
                    {
                        if (item.shippingStatus.ToString().Equals("delivered", StringComparison.OrdinalIgnoreCase))
                        {
                            var product = context.Products.GetProductByIdNoNull(item.productId).FirstOrDefault();
                            if (product != null && products.Contains(product))
                            {
                                if (productFromDay.ContainsKey(item.productId))
                                {
                                    productFromDay[item.productId] += item.quantity;
                                }
                                else
                                {
                                    productFromDay[item.productId] = item.quantity;
                                }
                            }
                        }
                    }
                }

                var productData = productFromDay.Select(item => new
                {
                    ProductName = context.Products.GetProductByIdNoNull(item.Key).FirstOrDefault()?.name,
                    Quantity = item.Value
                }).ToList();

                return Json(new
                {
                    productNames = productData.Select(p => p.ProductName).ToArray(),
                    productQuantities = productData.Select(p => p.Quantity).ToArray()
                });
            }
        }

        public IActionResult LoadProductInCate()
        {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Category> category = context.Categories.GetAllOfCategoriesByShop(shop.id).ToList();
                int[] number = new int[category.Count];
                int i = 0;
                foreach (Category cate in category)
                {
                    number[i] = context.Products.GetNumberOfProductByCategory(shop.id, cate);
                    i++;
                }
                return Json(new { category = category.Select(c => c.name).ToArray(), number });
            }
        }

        public IActionResult ViewQuantityProduct()
        {
            return View("/Views/SellerShopManager/ShopDashBoard/ViewQuantityProduct.cshtml");
        }

        public IActionResult LoadProductQuantity()
        {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();
                List<Product> products = context.Products.GetProductsByShop(shop.id).ToList();
                string[] productName = new string[products.Count];
                int[] productQuantity = new int[products.Count];
                int i = 0;
                foreach (Product product in products)
                {
                    if (product.isVariation)
                    {
                        productName[i] = product.name;
                        List<ProductItem> pi = context.ProductItems.GetProductItem(product.id).ToList();
                        foreach (ProductItem piItem in pi)
                        {
                            productQuantity[i] += piItem.quantity;
                        }
                    }
                    else
                    {
                        productName[i] = product.name;
                        productQuantity[i] = product.quantity;
                    }
                    i++;
                }
                return Json(new { productName, productQuantity });
            }
        }

        public IActionResult TotalOrder()
        {
            return View("/Views/SellerShopManager/ShopDashBoard/TotalOrder.cshtml");
        }

        public IActionResult TotalOrderDataFromDay(DateTime startTime, DateTime endTime)
        {
            using (var context = new Context())
            {
                int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                var shop = context.Shops.GetShopbyAccId(accId.Value).FirstOrDefault();

                List<Product> products = context.Products.GetProductsByShopNoNull(shop.id).ToList();
                List<Order> allOrders = context.Orders.ToList();

                List<Order> ordersFromDate = allOrders
                    .Where(order =>
                    {
                        DateTime orderDate;
                        bool isParsed = DateTime.TryParseExact(
                            order.createTime,
                            "yyyyMMddHHmmss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out orderDate
                        );
                        return isParsed && orderDate >= startTime && orderDate <= endTime;
                    })
                    .ToList();

                var orderData = new Dictionary<string, Dictionary<string, int>>
                {
                    // This dictionary will have usernames as keys,
                    // with each username containing both delivered and undelivered counts.
                };

                foreach (var order in ordersFromDate)
                {
                    List<OrderItem> orderItems = context.OrderItems.GetOrderItemByOrderId(order.id).ToList();

                    foreach (var item in orderItems)
                    {
                        var product = context.Products.GetProductByIdNoNull(item.productId).FirstOrDefault();
                        if (product == null || !products.Contains(product)) continue;

                        User user = context.Users.GetUserById(order.userId).FirstOrDefault();
                        if (user == null) continue;

                        if (!orderData.ContainsKey(user.displayName))
                        {
                            orderData[user.displayName] = new Dictionary<string, int>
                    {
                        { "DeliveredOrders", 0 },
                        { "NotDeliveredOrders", 0 }
                    };
                        }

                        if (item.shippingStatus.Equals("delivered", StringComparison.OrdinalIgnoreCase))
                        {
                            orderData[user.displayName]["DeliveredOrders"]++;
                        }
                        else if (!string.IsNullOrEmpty(item.externalOrderCode))
                        {
                            orderData[user.displayName]["NotDeliveredOrders"]++;
                        }
                    }
                }

                return Json(orderData);
            }
        }
    }
}
