using Azure;
using Azure.Core;

namespace E_Retalling_Portal.Util
{
    public static class CookiesUtils
    {
        private const string RecentlyViewedProductsCookie = "RecentlyViewedProducts";
        private const int MaxRecentProducts = 5;
        public static List<int> GetRecentlyViewedProductsFromCookie(HttpRequest request,HttpResponse response)
        {
            var cookieValue = request.Cookies[RecentlyViewedProductsCookie];
            if (string.IsNullOrEmpty(cookieValue))
            {

                return new List<int>();
            }
            return cookieValue.Split(',').Select(int.Parse).ToList();
        }

        public static void SaveProductToCookie(int productId, HttpRequest request, HttpResponse response)
        {
            var recentlyViewedProducts = CookiesUtils.GetRecentlyViewedProductsFromCookie(request,response);

            if (!recentlyViewedProducts.Contains(productId))
            {
                recentlyViewedProducts.Insert(0, productId);

                if (recentlyViewedProducts.Count > MaxRecentProducts)
                {
                    recentlyViewedProducts.RemoveAt(recentlyViewedProducts.Count - 1);
                }

                var cookieValue = string.Join(",", recentlyViewedProducts);
                response.Cookies.Append(RecentlyViewedProductsCookie, cookieValue, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }
        }
    }
}
