using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.ManagerStatisticModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace E_Retalling_portal.Models.Query
{
    public static class ManagerStatisticQuery
    {
        public static List<CategoryStats> GetCategoryStatsByShopId(int shopId)
        {
            using (var context = new Context())
            {
                var salesData = context.OrderItems
                    .Include(oi => oi.product)
                            .ThenInclude(p => p.category)
                    .Include(oi => oi.order)
                    .Include(oi => oi.product.shop)
                    .Where(oi => oi.product.shopId == shopId && oi.shippingStatus == "delivered")
                    .ToList();

                var groupedData = salesData
                    .GroupBy(oi => new
                    {
                        shopId = oi.product.shopId,
                        shopName = oi.product.shop.name,
                        CategoryId = oi.product.categoryId,
                        CategoryName = oi.product.category.name,
                        saleYear = int.Parse(oi.order.createTime.Substring(0, 4)),
                        saleMonth = int.Parse(oi.order.createTime.Substring(4, 2)),
                        saleDay = int.Parse(oi.order.createTime.Substring(6, 2))
                    })
                    .Select(g => new CategoryStats
                    {
                        shopId = g.Key.shopId,
                        shopName = g.Key.shopName,
                        categoryId = g.Key.CategoryId,
                        categoryName = g.Key.CategoryName,
                        saleYear = g.Key.saleYear,
                        saleMonth = g.Key.saleMonth,
                        saleDay = g.Key.saleDay,
                        totalQuantity = g.Sum(oi => oi.quanity)
                    })
                    .OrderBy(s => s.shopId)
                    .ThenBy(s => s.saleYear)
                    .ThenBy(s => s.saleMonth)
                    .ThenBy(s => s.saleDay)
                    .ThenBy(s => s.categoryId)
                    .ToList();

                return groupedData;
            }
        }

        public static List<CustomerStats> GetCustomerStatsByShopId(int shopId)
        {
            using (var context = new Context())
            {
                var orderItems = context.OrderItems
                    .Include(oi => oi.order)
                    .Include(oi => oi.product.shop)
                    .Where(oi => oi.product.shopId == shopId && oi.shippingStatus == "delivered")
                    .ToList();

                var customerStats = orderItems
                    .GroupBy(oi => new
                    {
                        shopId = oi.product.shopId,
                        shopName = oi.product.shop.name,
                        saleYear = Convert.ToInt32(oi.order.createTime.Substring(0, 4)),
                        saleMonth = Convert.ToInt32(oi.order.createTime.Substring(4, 2)),
                        saleDay = int.Parse(oi.order.createTime.Substring(6, 2))
                    })
                    .Select(g => new CustomerStats
                    {
                        shopId = g.Key.shopId,
                        shopName = g.Key.shopName,
                        saleYear = g.Key.saleYear,
                        saleMonth = g.Key.saleMonth,
                        saleDay = g.Key.saleDay,
                        totalCustomers = g.Select(oi => oi.order.userId).Distinct().Count()
                    })
                    .OrderBy(s => s.shopId)
                    .ThenBy(s => s.saleYear)
                    .ThenBy(s => s.saleMonth)
                    .ThenBy(s => s.saleDay)
                    .ToList();

                return customerStats;
            }
        }
        public static List<RevenueStats> GetRevenueStatsByShopId(int shopId)
        {
            using (var context = new Context())
            {
                var orderItems = context.OrderItems
                    .Include(oi => oi.order)
                    .Include(oi => oi.product.shop)
                    .Include(oi => oi.product.productItems)
                    .Where(oi => oi.product.shopId == shopId && oi.shippingStatus == "delivered")
                    .ToList();

                var RevenueStats = orderItems
                    .GroupBy(oi => new
                    {
                        shopId = oi.product.shopId,
                        shopName = oi.product.shop.name,
                        saleYear = int.Parse(oi.order.createTime.Substring(0, 4)),
                        saleMonth = int.Parse(oi.order.createTime.Substring(4, 2)),
                        saleDay = int.Parse(oi.order.createTime.Substring(6, 2))
                    })
                    .Select(g => new RevenueStats
                    {
                        shopId = g.Key.shopId,
                        shopName = g.Key.shopName,
                        saleYear = g.Key.saleYear,
                        saleMonth = g.Key.saleMonth,
                        saleDay = g.Key.saleDay,
                        totalRevenue = (decimal)g.Sum(oi =>
                            oi.productItemId.HasValue ?
                                oi.quanity * oi.product.productItems.FirstOrDefault(pi => pi.id == oi.productItemId)?.price :
                                oi.quanity * oi.product.price),
                        totalTransactionFee = (decimal)g.Sum(oi => oi.transactionFee)
                    })
                    .OrderBy(s => s.shopId)
                    .ThenBy(s => s.saleYear)
                    .ThenBy(s => s.saleMonth)
                    .ThenBy(s => s.saleDay)
                    .ToList();

                return RevenueStats;
            }
        }

        public static List<OrderStats> GetOrderStatsByShopId(int shopId)
        {
            using (var context = new Context())
            {
                var orderItems = context.OrderItems
                    .Include(oi => oi.order)
                    .Include(oi => oi.product.shop)
                    .Where(oi => oi.product.shopId == shopId && oi.shippingStatus == "delivered")
                    .ToList();

                var OrderStats = orderItems
                    .GroupBy(oi => new
                    {
                        shopId = oi.product.shopId,
                        shopName = oi.product.shop.name,
                        saleYear = int.Parse(oi.order.createTime.Substring(0, 4)),
                        saleMonth = int.Parse(oi.order.createTime.Substring(4, 2)),
                        saleDay = int.Parse(oi.order.createTime.Substring(6, 2))
                    })
                    .Select(g => new OrderStats
                    {
                        shopId = g.Key.shopId,
                        shopName = g.Key.shopName,
                        saleYear = g.Key.saleYear,
                        saleMonth = g.Key.saleMonth,
                        saleDay = g.Key.saleDay,
                        totalOrders = g.Select(oi => oi.id).Distinct().Count()
                    })
                    .OrderBy(s => s.shopId)
                    .ThenBy(s => s.saleYear)
                    .ThenBy(s => s.saleMonth)
                    .ThenBy(s => s.saleDay)
                    .ToList();

                return OrderStats;
            }
        }

        public static List<Top10SellingProduct> GetTop10BestSellingProductsByShopId(int shopId)
        {
            using (var context = new Context())
            {
                var orderItems = context.OrderItems
                    .Include(oi => oi.product)
                    .Include(oi => oi.order)
                    .Include(oi => oi.product.shop)
                    .Include(oi => oi.product.productItems)
                    .Where(oi => oi.product.shopId == shopId && oi.shippingStatus == "delivered")
                    .ToList();

                var bestSellingProducts = orderItems
                    .GroupBy(oi => new
                    {
                        ProductId = oi.productId,
                        ProductName = oi.product.name,
                        ProductItemId = oi.productItemId,
                        ProductItem = oi.productItemId.HasValue
                    ? oi.product.productItems.FirstOrDefault(pi => pi.id == oi.productItemId)
                    : null,
                        ShopId = oi.product.shopId,
                        ShopName = oi.product.shop.name,
                        SaleYear = int.Parse(oi.order.createTime.Substring(0, 4)),
                        SaleMonth = int.Parse(oi.order.createTime.Substring(4, 2)),
                        SaleDay = int.Parse(oi.order.createTime.Substring(6, 2))
                    })
                    .Select(g => new Top10SellingProduct
                    {
                        productId = g.Key.ProductId,
                        productItemId = g.Key.ProductItemId,
                        productName = g.Key.ProductName,
                        productItemName = g.Key.ProductItem != null
                    ? $"{g.Key.ProductItem.product.name} - {g.Key.ProductItem.attribute}"
                    : g.Key.ProductName,
                        shopId = g.Key.ShopId,
                        shopName = g.Key.ShopName,
                        saleYear = g.Key.SaleYear,
                        saleMonth = g.Key.SaleMonth,
                        saleDay = g.Key.SaleDay,
                        quantitySold = g.Sum(oi => oi.quanity)
                    })
                    .OrderByDescending(p => p.quantitySold)
                    .Take(10)
                    .ToList();

                return bestSellingProducts;
            }
        }


    }
}
