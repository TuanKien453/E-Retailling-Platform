using Azure;
using Azure.Core;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;

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

		public static Dictionary<string, int> GetCartItems(HttpRequest request)
		{
			var cartItems = new Dictionary<string, int>();

			// Lấy cartItems từ TempData nếu có
			if (request.HttpContext.Items["CartItems"] != null)
			{
				var cartString = request.HttpContext.Items["CartItems"].ToString();
				cartItems = JsonConvert.DeserializeObject<Dictionary<string, int>>(cartString);
			}
			else
			{
				// Lấy cartItems từ cookie
				var cookieValue = request.Cookies["Cart"];
				if (!string.IsNullOrEmpty(cookieValue))
				{
					var items = cookieValue.Split(',');
					foreach (var item in items)
					{
						var parts = item.Split(':');
						if (parts.Length == 2 && int.TryParse(parts[0].TrimEnd('P', 'I'), out int itemId) && int.TryParse(parts[1], out int quantity))
						{
							cartItems[parts[0]] = quantity;
						}
					}
				}
			}

			return cartItems;
		}

		public static string SetCartItems(Dictionary<string, int> cartItems, HttpResponse response)
		{
			var cartString = string.Join(",", cartItems.Select(ci => $"{ci.Key}:{ci.Value}"));

			response.Cookies.Append("Cart", cartString, new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30),
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict
			});
			return cartString;
		}
	}
}
