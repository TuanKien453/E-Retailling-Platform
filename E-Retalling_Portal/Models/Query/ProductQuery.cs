using E_Retalling_Portal.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace E_Retalling_Portal.Models.Query
{
    public static class ProductQuery
    {
        public static IQueryable<Product> GetProductsByShop(this DbSet<Product> dbProduct, int shopId)
        {
            return dbProduct.Include("coverImage").Include("images").Include("category").Where(p => p.shopId == shopId && p.deleteAt == null);
        }
        public static IQueryable<Product> GetProductById(this DbSet<Product> dbProduct, int productId)
        {
            return dbProduct.Include("coverImage").Include("images").Include("category").Include("shop").Where(p => p.id == productId && p.deleteAt == null);
        }
        public static IQueryable<Product> GetProduct(this DbSet<Product> dbProduct)
        {
            return dbProduct.Include("coverImage").Include("category").Include("images").Include("productItems").Include("shop").Where(p => p.deleteAt == null || (p.productItems.Where(pi => pi.deleteAt == null).ToList().Count > 0 && p.isVariation == true));
        }
        public static IQueryable<Product> GetSimilarProductByProductCategory(this DbSet<Product> dbProduct, int parentCategoryId, int productId)
        {
            return dbProduct
                .Include(p => p.coverImage)
                .Include(p => p.images)
                .Include(p => p.productItems)
                .Include(p => p.category)
                .Where(p =>
                    p.deleteAt == null &&
                    p.id != productId &&
                    p.category.parentCategoryId == parentCategoryId &&
                    (p.productItems == null || p.productItems.Count >= 0)
                );
        }





        public static void DeleteProductById(this DbSet<Product> dbProduct, int productId, Context context)
        {
            var product = dbProduct.GetProductById(productId).FirstOrDefault();
            if (product != null)
            {
                context.ProductDiscounts.DeleteProductDiscount(productId, null);
                context.SaveChanges();
                product.deleteAt = DateTime.Now.ToString();
                context.Entry(product).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static IQueryable<Product> GetProductsNoVariation(this DbSet<Product> dbProduct)
        {
            return dbProduct.Include(p => p.coverImage).Include(p => p.shop).Where(p => p.deleteAt == null && p.isVariation == false);
        }

        public static bool IsShop(this DbSet<Product> dbProduct, int shopId, int productId)
        {

            var p = dbProduct.Where(p => p.id == productId).FirstOrDefault();
            if (p == null)
            {
                return false;
            }
            return p.shopId == shopId;
        }

        public static bool IsNotMatch(this DbSet<Product> dbProduct, int? productId, int? productItemId)
        {
            if (productItemId == null || productId == null)
            {
                return false;
            }
            else
            {
                var p = dbProduct.Where(p => p.id == productId).FirstOrDefault();
                if (p.isVariation)
                {
                    foreach (var pi in p.productItems)
                    {
                        if (pi.id == (int)productItemId)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else return false;
            }
        }


        public static double GetProductDiscountPrice (this DbSet<Product> dbProduct, Product product)
        {
            using (var context = new Context())
            {
                ProductDiscount productDiscount = context.ProductDiscounts.GetProductDiscountByProductIdAndProductItemId(product.id, null).FirstOrDefault();
                if (productDiscount == null)
                {
                    return product.price;
                } else
                {
                    Discount discount = context.Discounts.GetDiscountByDiscountId(productDiscount.discountId).FirstOrDefault();
                    var today = DateTime.Today;
                    if (today >= DateTime.Parse(discount.startDate.ToString()) && today <= DateTime.Parse(discount.endDate.ToString()))
                    {
                        return Math.Round(product.price - Math.Round(product.price * Math.Round(((double)discount.value / 100), 2), 2));
                    }
                    else return product.price;
                } 
                
            }
        }

        public static int GetTotalStockProduct(this DbSet<Product> dbProduct, int shopId)
        {
            using (var context = new Context())
            {
                int stockProduct = 0;
                List<Product> products = context.Products.GetProductsByShop(shopId).ToList();
                foreach (var product in products)
                {
                    
                    if (product.isVariation)
                    {
                        List<ProductItem> pi = context.ProductItems.GetProductItem(product.id).ToList();
                        foreach (ProductItem piItem in pi)
                        {
                            stockProduct += piItem.quantity;
                        }
                    } else
                    {
                        stockProduct += product.quantity;
                    }
                }

                return stockProduct;
            }
        }

        public static int GetNumberAllSalesProduct(this DbSet<Product> dbProduct, List<Product> products)
        {
            int salesProduct = 0;
            using (var context = new Context())
            {
                    List<OrderItem> orderItems = context.OrderItems.GetAllOrderItemHasSales().ToList();
                    if (!orderItems.IsNullOrEmpty())
                    {
                    foreach (var product in products)
                    {
                        foreach (var item in orderItems)
                        {
                            if (product.id == item.productId)
                            {
                                salesProduct += item.quantity;
                            }
                        }
                    }
                    }
                    return salesProduct;
            }
        }
        public static int GetNumberOfProductByCategory(this DbSet<Product> dbProduct, int shopId, Category category)
        {
            using (var context = new Context())
            {
                int count = 0;
                List<Product> products = context.Products.GetProductsByShop(shopId).ToList();
                foreach (var product in products)
                {
                    if(product.categoryId == category.id)
                    {
                        count++;
                    }
                }
                return count;

            }
        }
        public static List<Product> GetAllSalesProduct(this DbSet<Product> dbProduct, List<Product> products)
        {
            using (var context = new Context())
            {
                List<Product>saleProduct = new List<Product>();
                List<OrderItem> orderItems = context.OrderItems.GetAllOrderItemHasSales().ToList();
                if (!orderItems.IsNullOrEmpty())
                {
                    foreach (var product in products)
                    {
                        foreach (var item in orderItems)
                        {
                            if (product.id == item.productId)
                            {
                                if (!saleProduct.Contains(context.Products.GetProductByIdNoNull(item.productId).FirstOrDefault()))
                                {
                                    saleProduct.Add(context.Products.GetProductByIdNoNull(item.productId).FirstOrDefault());
                                }
                                
                            }
                        }
                    }
                }
                return saleProduct;
            }
        }
        public static IQueryable<Product> GetProductsByShopNoNull(this DbSet<Product> dbProduct, int shopId)
        {
            return dbProduct.Include("coverImage").Include("images").Include("category").Where(p => p.shopId == shopId);
        }
        public static IQueryable<Product> GetProductByIdNoNull(this DbSet<Product> dbProduct, int productId)
        {
            return dbProduct.Include("coverImage").Include("images").Include("category").Include("shop").Where(p => p.id == productId);
        }
    }
}
