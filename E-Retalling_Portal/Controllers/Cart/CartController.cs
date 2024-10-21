using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using Microsoft.EntityFrameworkCore;
using E_Retalling_Portal.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using E_Retalling_Portal.Models.Query;
using X.PagedList.Mvc.Core;
using X.PagedList.Extensions;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace E_Retalling_Portal.Controllers.Cart
{
	public class CartController : Controller
	{
		public async Task<IActionResult> Index(int? page)
		{
			using (var context = new Context())
			{
				Dictionary<string, int> cartItems = GetCartItems();

				var productItems = await context.ProductItems.GetAllProductItem().ToListAsync();
				var products = await context.Products.GetProductsNoVariation().ToListAsync();

				var cartDetails = cartItems.Select(ci => new CartItemModel
				{
					quantity = ci.Value,
					product = ci.Key.EndsWith("P") ? products.FirstOrDefault(p => p.id == int.Parse(ci.Key.TrimEnd('P'))) : null,
					productItem = ci.Key.EndsWith("PI") ? productItems.FirstOrDefault(pi => pi.id == int.Parse(ci.Key.TrimEnd("PI".ToCharArray()))) : null
				}).ToList();

				// Paging
				var pageNumber = page ?? 1;
				var pageSize = 1;
				var pagedCartItem = cartDetails.ToPagedList(pageNumber, pageSize);
				if(pagedCartItem.Count() == 0)
				{
					return RedirectToAction("Index");
				}

				return View(pagedCartItem);
			}
		}

		public IActionResult AddToCart(int itemId, int quantity, bool isProduct)
		{
			var cartItems = GetCartItems();
			string cartKey = isProduct ? $"{itemId}P" : $"{itemId}PI";

			if (cartItems.ContainsKey(cartKey))
			{
				cartItems[cartKey] += quantity; 
			}
			else
			{
				cartItems[cartKey] = quantity; 
			}

			TempData["CartItems"] = JsonConvert.SerializeObject(cartItems);

			SetCartItems(cartItems); 

			return RedirectToAction("Index");
		}

		private Dictionary<string, int> GetCartItems()
		{
			var cartItems = new Dictionary<string, int>();

			if (TempData["CartItems"] != null)
			{
				var cartString = TempData["CartItems"].ToString();
				cartItems = JsonConvert.DeserializeObject<Dictionary<string, int>>(cartString);
			}
			else
			{
				var cookieValue = Request.Cookies["Cart"];
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

		[HttpPost]
		public IActionResult DeleteFromCart(int itemId, bool isProduct)
		{
			var cartItems = GetCartItems();
			string cartKey = isProduct ? $"{itemId}P" : $"{itemId}PI";

			if (cartItems.ContainsKey(cartKey))
			{
				cartItems.Remove(cartKey);
			}

			var cartString = string.Join(",", cartItems.Select(ci => $"{ci.Key}:{ci.Value}"));
			Response.Cookies.Append("Cart", cartString, new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30),
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict
			});
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult UpdateFromCart(int itemId, int quantity, bool isProduct)
		{
			var cartItems = GetCartItems();
			string cartKey = isProduct ? $"{itemId}P" : $"{itemId}PI";

			cartItems[cartKey] = quantity;

			var cartString = string.Join(",", cartItems.Select(ci => $"{ci.Key}:{ci.Value}"));
			Response.Cookies.Append("Cart", cartString, new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30),
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict
			});

			return RedirectToAction("Index");
		}

		private void SetCartItems(Dictionary<string, int> cartItems)
		{
			var cartString = string.Join(",", cartItems.Select(ci => $"{ci.Key}:{ci.Value}"));

			Response.Cookies.Append("Cart", cartString, new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30),
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict
			});
		}

	}
}
