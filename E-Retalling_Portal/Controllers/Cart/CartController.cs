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
using X.PagedList;
using E_Retalling_Portal.Util;
using Microsoft.CodeAnalysis.Operations;

namespace E_Retalling_Portal.Controllers.Cart
{
	public class CartController : Controller
	{
		public async Task<IActionResult> Index(int? page)
		{
			using (var context = new Context())
			{
				Dictionary<string, int> cartItems = CookiesUtils.GetCartItems(Request);

				var productItems = await context.ProductItems.GetAllProductItem().ToListAsync();
				var products = await context.Products.GetProductsNoVariation().ToListAsync();


				var removedProductIds = new HashSet<int>();
				var removedProductItemIds = new HashSet<int>();

				//Delete deletedItem in cookie and save deletedItem to hashset
				foreach (var item in cartItems)
				{
					var key = item.Key;

					if (key.EndsWith("P"))
					{
						int productId = int.Parse(key.TrimEnd('P'));
						if (!products.Any(p => p.id == productId))
						{
							removedProductIds.Add(productId);
							DeleteFromCart(productId, true);
						}
					}
					else if (key.EndsWith("PI"))
					{
						int productItemId = int.Parse(key.TrimEnd("PI".ToCharArray()));
						if (!productItems.Any(pi => pi.id == productItemId))
						{
							removedProductItemIds.Add(productItemId);
							DeleteFromCart(productItemId, false);
						}
					}
				}

				//IF cookiedata null or check deleledItem is exist in cookie or not  
				var cookiedata = Request.Cookies["Cart"];
				if (cookiedata == null)
				{
					cartItems = new Dictionary<string, int>();
				}
				else
				{
					bool allItemsRemoved = true;

					foreach (var item in cartItems)
					{
						var key = item.Key;
						if (key.EndsWith("P"))
						{
							int productId = int.Parse(key.TrimEnd('P'));
							if (!removedProductIds.Contains(productId))
							{
								allItemsRemoved = false;
								break;
							}
						}
						else if (key.EndsWith("PI"))
						{
							int productItemId = int.Parse(key.TrimEnd("PI".ToCharArray()));
							if (!removedProductItemIds.Contains(productItemId))
							{
								allItemsRemoved = false;
								break;
							}
						}
					}

					if (allItemsRemoved)
					{
						cartItems = new Dictionary<string, int>();
					}
				}

				var cartDetails = cartItems.Select(ci => new CartItemModel
				{
					quantity = ci.Value,
					product = ci.Key.EndsWith("P") ? products.FirstOrDefault(p => p.id == int.Parse(ci.Key.TrimEnd('P'))) : null,
					productItem = ci.Key.EndsWith("PI") ? productItems.FirstOrDefault(pi => pi.id == int.Parse(ci.Key.TrimEnd("PI".ToCharArray()))) : null,
					discountedPrice = ci.Key.EndsWith("P") ? (float)context.Products.GetProductDiscountPrice(products.FirstOrDefault(p => p.id == int.Parse(ci.Key.TrimEnd('P')))) :
					ci.Key.EndsWith("PI") ? (float)context.ProductItems.GetProductItemDiscountPrice(productItems.FirstOrDefault(pi => pi.id == int.Parse(ci.Key.TrimEnd("PI".ToCharArray())))) : 0
				}).ToList();

				if (cartDetails == null || !cartDetails.Any())
				{
					ViewBag.IsCartEmpty = true;
					return View();
				}

                User user = null;
				if (HttpContext.Session.Keys.Contains(SessionKeys.AccountId.ToString())) {
                    var accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
                    var acc = context.Accounts.GetAccountByAccountId(accId.Value).FirstOrDefault();
					user = acc.user;
				}
				ViewBag.IsCartEmpty = false;
				ViewBag.User = user;
				return View(cartDetails);
			}
		}


		public IActionResult AddToCart(int itemId, int quantity, bool isProduct)
		{
			using (var context = new Context())
			{
				if (isProduct)
				{
					if(context.Products.GetProductById(itemId).FirstOrDefault() == null)
					{
						return RedirectToAction("Error505", "Home");			    
					}
				}	
				else
				{
					if(context.ProductItems.GetProductItemByProductItemId(itemId).FirstOrDefault() == null)
					{
                        return RedirectToAction("Error505", "Home");
                    }
				}
			}
			var cartItems = CookiesUtils.GetCartItems(Request);
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

			CookiesUtils.SetCartItems(cartItems, Response);

			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult DeleteFromCart(int itemId, bool isProduct)
		{
			var cartItems = CookiesUtils.GetCartItems(Request);
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
			var cartItems = CookiesUtils.GetCartItems(Request);
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
	}
}
