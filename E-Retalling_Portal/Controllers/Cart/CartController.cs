using Microsoft.AspNetCore.Mvc;
using E_Retalling_Portal.Models;
using Microsoft.EntityFrameworkCore;
using E_Retalling_Portal.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using E_Retalling_Portal.Models.Query;

namespace E_Retalling_Portal.Controllers.Cart
{
    public class CartController : Controller
    {

        public IActionResult Index()
        {
            using (var context = new Context())
            {

                // Get the items currently in the cart.
                Dictionary<int, int> cartItems = GetCartItems();

                //If there are no items in the cart, add some sample items for demonstration.
                if (!cartItems.Any())
                {
                    AddToCart(1, 2);
                    AddToCart(2, 3);
                    AddToCart(1, 3);
                    cartItems = GetCartItems();
                }

                // Retrieve all product items from the database.
                var productItems = context.ProductItems.GetAllProductItem().ToList();

                // Map cart items to CartItemModel for the view.
                var cartDetails = cartItems.Select(ci => new CartItemModel
                {
                    quantity = ci.Value,
                    productItem = context.ProductItems.GetProductItemByProductItemId(ci.Key).FirstOrDefault()
                }).ToList();

                return View(cartDetails);
            }
        }

        // Adds an item to the cart with a specified quantity.
        public IActionResult AddToCart(int productItemId, int quantity)
        {
            var cartItems = GetCartItems();

            // If the item already exists in the cart, update its quantity; otherwise, add it.
            if (cartItems.ContainsKey(productItemId))
            {
                cartItems[productItemId] += quantity;
            }
            else
            {
                cartItems[productItemId] = quantity;
            }

            // Store the updated cart items.
            SetCartItems(string.Join(",", cartItems.Select(ci => $"{ci.Key}:{ci.Value}")));

            // Use TempData to temporarily store cart items.
            TempData["CartItems"] = cartItems;
            return RedirectToAction("Index");
        }

        // Retrieves items from the cart, checking both TempData and cookies.
        private Dictionary<int, int> GetCartItems()
        {
            var cartItems = new Dictionary<int, int>();

            // Check if cart items are stored in TempData.
            if (TempData["CartItems"] != null)
            {
                cartItems = (Dictionary<int, int>)TempData["CartItems"];
            }
            else
            {
                // Check cookies for cart items associated with the account ID.
                var cookieValue = Request.Cookies["Cart"];
                if (!string.IsNullOrEmpty(cookieValue))
                {
                    var items = cookieValue.Split(',');
                    foreach (var item in items)
                    {
                        var parts = item.Split(':');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int productItemId) && int.TryParse(parts[1], out int quantity))
                        {
                            cartItems[productItemId] = quantity;
                        }
                    }
                }
            }

            return cartItems;
        }

        // Removes an item from the cart based on its product item ID.
        [HttpPost]
        public IActionResult DeleteFromCart(int productItemId)
        {
            var cartItems = GetCartItems();

            // If the item exists, remove it from the cart.
            if (cartItems.ContainsKey(productItemId))
            {
                cartItems.Remove(productItemId);
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
        public IActionResult UpdateFromCart(int productItemId, int quantity)
        {
            var cartItems = GetCartItems();

            // Update the quantity for the specified product item ID.
            cartItems[productItemId] = quantity;

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

            var existingItems = new Dictionary<int, int>();

            // If existing cart items are found, parse them into a dictionary.
            if (!string.IsNullOrEmpty(existingCart))
            {
                existingItems = existingCart.Split(',')
                                    .Select(x => x.Split(':'))
                                    .ToDictionary(x => int.Parse(x[0]), x => int.Parse(x[1]));
            }

            // Parse new cart items from the provided string.
            var newItems = cartItems.Split(',')
                            .Select(x => x.Split(':'))
                            .ToDictionary(x => int.Parse(x[0]), x => int.Parse(x[1]));

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