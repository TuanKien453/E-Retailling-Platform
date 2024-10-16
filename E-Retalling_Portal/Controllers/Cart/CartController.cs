using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using Microsoft.EntityFrameworkCore;
using E_Retalling_Portal.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using E_Retalling_Portal.Models.Query;
using X.PagedList.Mvc.Core;
using X.PagedList.Extensions;

namespace E_Retalling_Portal.Controllers.Cart
{
	public class CartController : Controller
	{
		public async Task<IActionResult> Index(int? page)
		{
			using (var context = new Context())
			{
				// Get the items currently in the cart (handling both products and product items).
				Dictionary<string, int> cartItems = GetCartItems();

				// If there are no items in the cart, add some sample items for demonstration.
				if (!cartItems.Any())
				{
					cartItems = GetCartItems();
				}

				// Retrieve all products and product items from the database.
				var productItems = await context.ProductItems.GetAllProductItem().ToListAsync();
				var products =  await context.Products.GetProductsNoVariation().ToListAsync();

				// Map cart items to CartItemModel for the view.
				var cartDetails = cartItems.Select(ci => new CartItemModel
				{
					quantity = ci.Value,
					product = ci.Key.EndsWith("P") ? products.FirstOrDefault(p => p.id == int.Parse(ci.Key.TrimEnd('P'))) : null,
					productItem = ci.Key.EndsWith("PI") ? productItems.FirstOrDefault(pi => pi.id == int.Parse(ci.Key.TrimEnd("PI".ToCharArray()))) : null
				}).ToList();

				//paging
				var pageNumber = page ?? 1;
				var pageSize = 1;
				var pagedCartItem = cartDetails.ToPagedList(pageNumber, pageSize);

				return View(pagedCartItem);
			}
		}

		// Adds an item to the cart with a specified quantity, specifying if it's a product or product item.
		public IActionResult AddToCart(int itemId, int quantity, bool isProduct)
		{
			var cartItems = GetCartItems();
			string cartKey = isProduct ? $"{itemId}P" : $"{itemId}PI";

			// If the item already exists in the cart, update its quantity; otherwise, add it.
			if (cartItems.ContainsKey(cartKey))
			{
				cartItems[cartKey] += quantity;
			}
			else
			{
				cartItems[cartKey] = quantity;
			}

			// Store the updated cart items.
			SetCartItems(string.Join(",", cartItems.Select(ci => $"{ci.Key}:{ci.Value}")));

			TempData["CartItems"] = cartItems;
			return RedirectToAction("Index");
		}

		// Retrieves items from the cart (handling both TempData and cookies).
		private Dictionary<string, int> GetCartItems()
		{
			var cartItems = new Dictionary<string, int>();

			// Check if cart items are stored in TempData.
			if (TempData["CartItems"] != null)
			{
				cartItems = (Dictionary<string, int>)TempData["CartItems"];
			}
			else
			{
				// Check cookies for cart items.
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

		// Removes an item from the cart based on its product or product item ID.
		[HttpPost]
		public IActionResult DeleteFromCart(int itemId, bool isProduct)
		{
			var cartItems = GetCartItems();
			string cartKey = isProduct ? $"{itemId}P" : $"{itemId}PI";

			// If the item exists, remove it from the cart.
			if (cartItems.ContainsKey(cartKey))
			{
				cartItems.Remove(cartKey);
			}

			// Update the cookie with the remaining cart items.
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

		// Updates the quantity of a specific item in the cart.
		[HttpPost]
		public IActionResult UpdateFromCart(int itemId, int quantity, bool isProduct)
		{
			var cartItems = GetCartItems();
			string cartKey = isProduct ? $"{itemId}P" : $"{itemId}PI";

			// Update the quantity for the specified item.
			cartItems[cartKey] = quantity;

			// Update the cookie with the modified cart items.
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

		// Sets the cart items in the cookie by merging new items with existing ones.
		private void SetCartItems(string cartItems)
		{
			var existingCart = Request.Cookies["Cart"];
			var existingItems = new Dictionary<string, int>();

			// If existing cart items are found, parse them into a dictionary.
			if (!string.IsNullOrEmpty(existingCart))
			{
				existingItems = existingCart.Split(',')
									.Select(x => x.Split(':'))
									.ToDictionary(x => x[0], x => int.Parse(x[1]));
			}

			// Parse new cart items from the provided string.
			var newItems = cartItems.Split(',')
							.Select(x => x.Split(':'))
							.ToDictionary(x => x[0], x => int.Parse(x[1]));

			// Merge new items with existing items, summing quantities for duplicates.
			foreach (var item in newItems)
			{
				if (existingItems.ContainsKey(item.Key))
				{
					existingItems[item.Key] += item.Value;
				}
				else
				{
					existingItems[item.Key] = item.Value;
				}
			}

			// Convert the updated items back to a string format for the cookie.
			var updatedCartItems = string.Join(",", existingItems.Select(ci => $"{ci.Key}:{ci.Value}"));

			// Update the cookie with the merged cart items.
			Response.Cookies.Append("Cart", updatedCartItems, new CookieOptions
			{
				Expires = DateTime.Now.AddDays(30),
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict
			});
		}
	}
}
