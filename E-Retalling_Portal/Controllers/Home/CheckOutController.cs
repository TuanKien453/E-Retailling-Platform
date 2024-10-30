using Azure;
using E_Retalling_Portal.Controllers.Filter;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Models.GHNRequestModel;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Services;
using E_Retalling_Portal.Services.ExtendService;
using E_Retalling_Portal.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;

namespace E_Retalling_Portal.Controllers.Home
{
    public class CheckOutController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly GHNService _ghnService;
        public CheckOutController(IVnPayService vnPayService, GHNService ghnService)
        {
            _vnPayService = vnPayService;
            _ghnService = ghnService;
        }
        [HttpPost]
        public async Task<IActionResult> CalculateShippingFee(int productId, int quantity, int toDistrcitId, string toWardCode)
        {

            Product p;
            using (var context = new Context())
            {
                p = context.Products.GetProductById(productId).FirstOrDefault();
            }
            if (productId == null || p == null)
            {
                return BadRequest("Product not found or invalid");
            }
            int totalWeight = p.weight * quantity;
            if (totalWeight > 50000)
            {
                return BadRequest("Your order is greater than 50kg please split your order");
            }

            var feeRequest = new FeeRequest
            {
                serviceTypeId = 2,
                weight = totalWeight,
                fromDistrictId = p.shop.district,
                toDistrcitId = toDistrcitId,
                toWardCode = toWardCode,

            };
            try
            {
                var response = await _ghnService.CalulateFreeAsync(feeRequest);
                return Json(new { fee = response.Data.Total });
            }
            catch (Exception ex)
            {
                return BadRequest($"Shipping route not supported");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(String cartItems, int toProvinceId, int toDistrictId, string toWardCode, string address, int paymentMethod)
        {
            int? accId = HttpContext.Session.GetInt32(SessionKeys.AccountId.ToString());
            Console.WriteLine("CreatePaymentUrl called with parameters:");
            Console.WriteLine($"cartItems: {cartItems}");
            Console.WriteLine($"toDistrictId: {toDistrictId}");
            Console.WriteLine($"toWardCode: {toWardCode}");
            Console.WriteLine($"address: {address}");
            Console.WriteLine($"paymentMethod: {paymentMethod}");

            if (accId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int payment_type_id = 0;

            string paymentMethodString = "";
            if (paymentMethod == 1)
            {
                payment_type_id = 2;
                paymentMethodString = "cash";
            }
            else if (paymentMethod == 2)
            {
                payment_type_id = 1;
                paymentMethodString = "online";
            }
            else
            {
                return RedirectToAction("Error505", "Home");
            }
            //check user imformation
            User user;
            using (var context = new Context())
            {
                user = context.Accounts.GetAccountByAccountId(accId.Value).FirstOrDefault().user;
                if (user.phoneNumber.IsNullOrEmpty())
                {
                    TempData["mess"] = "complete your phone number to order";
                    return RedirectToAction("Index", "Cart");
                }
            }

            var piItems = new Dictionary<ProductItem, int>();
            var pItems = new Dictionary<Product, int>();
            var items = cartItems.Split(',');
            //read from cart item and check valid
            using (var context = new Context())
            {
                foreach (var item in items)
                {

                    if (item.Contains("PI"))
                    {
                        var parts = item.Split("PI:");
                        var productItemId = int.Parse(parts[0]);
                        var quantity = int.Parse(parts[1]);
                        var pi = context.ProductItems.GetProductItemByProductItemId(productItemId).FirstOrDefault();

                        if (pi == null || pi.product.weight * quantity > 50000)
                        {
                            return RedirectToAction("Error505", "Home");
                        }
                        //check quantity
                        if (pi.quantity < quantity)
                        {
                            TempData["mess"] = "quantity not enough to order";
                            return RedirectToAction("Index", "Cart");
                        }
                        piItems[pi] = quantity;
                    }
                    else if (item.Contains("P"))
                    {
                        var parts = item.Split("P:");
                        var productId = int.Parse(parts[0]);
                        var quantity = int.Parse(parts[1]);
                        var p = context.Products.GetProductById(productId).FirstOrDefault();
                        if (p == null || p.weight * quantity > 50000)
                        {
                            return RedirectToAction("Error505", "Home");
                        }
                        //check quantity
                        if (p.quantity < quantity)
                        {
                            TempData["mess"] = "quantity not enough to order";
                            return RedirectToAction("Index", "Cart");
                        }
                        pItems[p] = quantity;
                    }
                }
            }
            if (piItems.Count + pItems.Count == 0)
            {
                return RedirectToAction("Error505", "Home");
            }

            DateTime currentTime = DateTime.Now;
            string formattedCurrentTime = currentTime.ToString("yyyyMMddHHmmss");

            DateTime endTime = currentTime.AddMinutes(10);
            string formattedEndTime = endTime.ToString("yyyyMMddHHmmss");




            //check can create order in GHN
            using (var context = new Context())
            {
                foreach (var item in piItems)
                {
                    var price = context.ProductItems.GetProductItemDiscountPrice(item.Key);

                    var orderRequestItem = new OrderRequest
                    {
                        PaymentTypeId = payment_type_id,
                        ServiceTypeId = 2,
                        RequiredNote = "CHOXEMHANGKHONGTHU",
                        FromName = user.displayName,
                        FromPhone = user.phoneNumber,
                        FromAddress = item.Key.product.shop.address,
                        FromWardCode = item.Key.product.shop.ward,
                        FromDistrictId = item.Key.product.shop.district,

                        ToName = user.firstName + " " + user.lastName,
                        ToPhone = user.phoneNumber,
                        ToAddress = address,
                        ToWardCode = toWardCode,
                        ToDistrictId = toDistrictId,
                        COD = 0,

                        Weight = item.Key.product.weight * item.Value,
                        Length = 20,
                        Width = 20,
                        Height = 20,

                        Items = new List<OrderItemRequest>
                        {
                        new OrderItemRequest
                            {
                            Name = item.Key.product.name,
                            Weight = item.Key.product.weight,
                            Quantity =item.Value,
                            }
                        }
                    };
                    try
                    {
                        var orderResponse = await _ghnService.CreateShippingOrderPreviewAsync(orderRequestItem);
                    }
                    catch (Exception ex)
                    {
                        TempData["mess"] = "error while connect with shipping provider";
                        return RedirectToAction("Index", "Cart");
                    }
                }

                foreach (var item in pItems)
                {
                    var price = context.Products.GetProductDiscountPrice(item.Key);
                    var orderRequestItem = new OrderRequest
                    {
                        PaymentTypeId = payment_type_id,
                        ServiceTypeId = 2,
                        RequiredNote = "CHOXEMHANGKHONGTHU",
                        FromName = user.displayName,
                        FromPhone = user.phoneNumber,
                        FromAddress = item.Key.shop.address,
                        FromWardCode = item.Key.shop.ward,
                        FromDistrictId = item.Key.shop.district,

                        ToName = user.firstName + " " + user.lastName,
                        ToPhone = user.phoneNumber,
                        ToAddress = address,
                        ToWardCode = toWardCode,
                        ToDistrictId = toDistrictId,
                        COD = 0,

                        Weight = item.Key.weight * item.Value,
                        Length = 20,
                        Width = 20,
                        Height = 20,

                        Items = new List<OrderItemRequest>
                    {
                        new OrderItemRequest
                        {
                            Name = item.Key.name,
                            Weight = item.Key.weight,
                            Quantity =item.Value,
                        }
                    }
                    };
                    try
                    {
                        var orderResponse = await _ghnService.CreateShippingOrderPreviewAsync(orderRequestItem);

                    }
                    catch (Exception ex)
                    {
                        TempData["mess"] = "error while connect with shipping provider";
                        return RedirectToAction("Index", "Cart");
                    }
                }
            }

            Order order;
            int transactionFee = 0;
            using (var context = new Context())
            {
                transactionFee = Int32.Parse(context.Settings.FirstOrDefault(x => x.name.Equals("fee")).value.Replace("%", ""));
                order = new Order
                {
                    address = address,
                    province = toProvinceId,
                    district = toDistrictId,
                    ward = toWardCode,
                    createTime = formattedCurrentTime,
                    endTime = null,

                    note = "",
                    paymentMethod = paymentMethodString,
                    paymentStatus = null,

                    userId = user.id
                };
                context.Add(order);
                context.SaveChanges();
                //user pay Cash
                if (paymentMethod == 1)
                {
                    foreach (var item in piItems)
                    {
                        var price = context.ProductItems.GetProductItemDiscountPrice(item.Key);
                        var fromAcc = context.Accounts.GetAccountByAccountId(item.Key.product.shop.accountId).FirstOrDefault();
                        var orderRequestItem = new OrderRequest
                        {
                            PaymentTypeId = payment_type_id,
                            ServiceTypeId = 2,
                            RequiredNote = "CHOXEMHANGKHONGTHU",
                            FromName = fromAcc.shop.name,
                            FromPhone = fromAcc.user.phoneNumber,
                            FromAddress = item.Key.product.shop.address,
                            FromWardCode = item.Key.product.shop.ward,
                            FromDistrictId = item.Key.product.shop.district,

                            ToName = user.firstName + " " + user.lastName,
                            ToPhone = user.phoneNumber,
                            ToAddress = address,
                            ToWardCode = toWardCode,
                            ToDistrictId = toDistrictId,
                            COD = (int)price * item.Value,

                            Weight = item.Key.product.weight * item.Value,
                            Length = 20,
                            Width = 20,
                            Height = 20,

                            Items = new List<OrderItemRequest>
                    {
                        new OrderItemRequest
                        {
                            Name = item.Key.product.name,
                            Weight = item.Key.product.weight,
                            Quantity =item.Value,
                        }
                    }
                        };
                        var orderResponse = await _ghnService.CreateShippingOrderAsync(orderRequestItem);

                        OrderItem orderItem = new OrderItem
                        {
                            orderId = order.id,
                            price = price * item.Value,
                            quanity = item.Value,
                            productId = item.Key.product.id,
                            productItemId = item.Key.id,
                            shippingFee = Int32.Parse(orderResponse.Data.TotalFee),
                            transactionFee = transactionFee,
                            externalOrderCode = orderResponse.Data.OrderCode,
                            shippingStatus = "wait to take"
                        };
                        item.Key.quantity -= item.Value;
                        context.Add(orderItem);
                        context.SaveChanges();
                    }

                    foreach (var item in pItems)
                    {
                        var price = context.Products.GetProductDiscountPrice(item.Key);
                        var fromAcc = context.Accounts.GetAccountByAccountId(item.Key.shop.accountId).FirstOrDefault();
                        var orderRequestItem = new OrderRequest
                        {
                            PaymentTypeId = payment_type_id,
                            ServiceTypeId = 2,
                            RequiredNote = "CHOXEMHANGKHONGTHU",
                            FromName = fromAcc.shop.name,
                            FromPhone = fromAcc.user.phoneNumber,
                            FromAddress = item.Key.shop.address,
                            FromWardCode = item.Key.shop.ward,
                            FromDistrictId = item.Key.shop.district,

                            ToName = user.firstName + " " + user.lastName,
                            ToPhone = user.phoneNumber,
                            ToAddress = address,
                            ToWardCode = toWardCode,
                            ToDistrictId = toDistrictId,
                            COD = (int)price * item.Value,

                            Weight = item.Key.weight * item.Value,
                            Length = 20,
                            Width = 20,
                            Height = 20,

                            Items = new List<OrderItemRequest>
                    {
                        new OrderItemRequest
                        {
                            Name = item.Key.name,
                            Weight = item.Key.weight,
                            Quantity =item.Value,
                        }
                    }
                        };
                        var orderResponse = await _ghnService.CreateShippingOrderAsync(orderRequestItem);
                        OrderItem orderItem = new OrderItem
                        {
                            orderId = order.id,
                            price = price * item.Value,
                            quanity = item.Value,
                            productId = item.Key.id,
                            productItemId = null,
                            shippingFee = Int32.Parse(orderResponse.Data.TotalFee),
                            transactionFee = transactionFee,
                            externalOrderCode = orderResponse.Data.OrderCode,
                            shippingStatus = "wait to take"
                        };
                        item.Key.quantity -= item.Value;
                        context.Add(orderItem);
                        context.SaveChanges();
                    }

                }
                //online paying

                if (paymentMethod == 2)
                {
                    int totalShippingfee = 0;
                    double totalProductFee = 0;
                    order.paymentStatus = "pending";
                    order.endTime = formattedEndTime;
                    context.Update(order);
                    context.SaveChanges();
                    foreach (var item in piItems)
                    {
                        var price = context.ProductItems.GetProductItemDiscountPrice(item.Key);
                        var fromAcc = context.Accounts.GetAccountByAccountId(item.Key.product.shop.accountId).FirstOrDefault();
                        totalProductFee += price;
                        var orderRequestItem = new OrderRequest
                        {
                            PaymentTypeId = payment_type_id,
                            ServiceTypeId = 2,
                            RequiredNote = "CHOXEMHANGKHONGTHU",
                            FromName = fromAcc.shop.name,
                            FromPhone = fromAcc.user.phoneNumber,
                            FromAddress = item.Key.product.shop.address,
                            FromWardCode = item.Key.product.shop.ward,
                            FromDistrictId = item.Key.product.shop.district,

                            ToName = user.firstName + " " + user.lastName,
                            ToPhone = user.phoneNumber,
                            ToAddress = address,
                            ToWardCode = toWardCode,
                            ToDistrictId = toDistrictId,
                            COD = 0,

                            Weight = item.Key.product.weight * item.Value,
                            Length = 20,
                            Width = 20,
                            Height = 20,

                            Items = new List<OrderItemRequest>
                    {
                        new OrderItemRequest
                        {
                            Name = item.Key.product.name,
                            Weight = item.Key.product.weight,
                            Quantity =item.Value,
                        }
                    }
                        };
                        var orderResponse = await _ghnService.CreateShippingOrderPreviewAsync(orderRequestItem);
                        int shipFee = Int32.Parse(orderResponse.Data.TotalFee);
                        totalShippingfee += shipFee;
                        OrderItem orderItem = new OrderItem
                        {
                            orderId = order.id,
                            price = price * item.Value,
                            quanity = item.Value,
                            productId = item.Key.product.id,
                            productItemId = item.Key.id,
                            shippingFee = shipFee,
                            transactionFee = transactionFee,
                            externalOrderCode = orderResponse.Data.OrderCode,
                            shippingStatus = "wait to take"
                        };
                        item.Key.quantity -= item.Value;
                        context.Add(orderItem);
                        context.SaveChanges();
                    }

                    foreach (var item in pItems)
                    {
                        var price = context.Products.GetProductDiscountPrice(item.Key);
                        var fromAcc = context.Accounts.GetAccountByAccountId(item.Key.shop.accountId).FirstOrDefault();
                        totalProductFee += price;
                        var orderRequestItem = new OrderRequest
                        {
                            PaymentTypeId = payment_type_id,
                            ServiceTypeId = 2,
                            RequiredNote = "CHOXEMHANGKHONGTHU",
                            FromName = fromAcc.shop.name,
                            FromPhone = fromAcc.user.phoneNumber,
                            FromAddress = item.Key.shop.address,
                            FromWardCode = item.Key.shop.ward,
                            FromDistrictId = item.Key.shop.district,

                            ToName = user.firstName + " " + user.lastName,
                            ToPhone = user.phoneNumber,
                            ToAddress = address,
                            ToWardCode = toWardCode,
                            ToDistrictId = toDistrictId,
                            COD = 0,

                            Weight = item.Key.weight * item.Value,
                            Length = 20,
                            Width = 20,
                            Height = 20,

                            Items = new List<OrderItemRequest>
                            {
                                new OrderItemRequest
                                {
                                    Name = item.Key.name,
                                    Weight = item.Key.weight,
                                    Quantity =item.Value,
                                }
                            }
                        };
                        var orderResponse = await _ghnService.CreateShippingOrderPreviewAsync(orderRequestItem);
                        int shipFee = Int32.Parse(orderResponse.Data.TotalFee);
                        totalShippingfee += shipFee;
                        OrderItem orderItem = new OrderItem
                        {
                            orderId = order.id,
                            price = price * item.Value,
                            quanity = item.Value,
                            productId = item.Key.id,
                            productItemId = null,
                            shippingFee = shipFee,
                            transactionFee = transactionFee,
                            externalOrderCode = orderResponse.Data.OrderCode,
                            shippingStatus = "wait to take"
                        };
                        item.Key.quantity -= item.Value;
                        context.Add(orderItem);
                        context.SaveChanges();
                    }

                    int amount = totalShippingfee + (int)totalProductFee;
                    string stringAmount = amount + "00";
                    Console.WriteLine(totalProductFee);
                    Console.WriteLine(totalShippingfee);
                    Console.WriteLine(stringAmount);
                    var url = _vnPayService.CreatePaymentUrl(HttpContext, $"{order.id}", stringAmount, formattedCurrentTime, formattedEndTime);
                    return Redirect(url);
                }

                return Ok("order success");
            }




            //var url = _vnPayService.CreatePaymentUrl(HttpContext, "test","12346789");
        }

        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            //payment success
            if (response.Success == true && response.VnPayResponseCode.Equals("00"))
            {
                int orderId = Int32.Parse(response.OrderDescription);
                using (var context = new Context())
                {
                    Order o = context.Orders
                        .Include(o => o.user)
                        .Include(o => o.orderItems)
                        .FirstOrDefault(o => o.id == orderId);
                    List<OrderItem> oi = o.orderItems;

                    foreach (var item in oi)
                    {
                        string fromName = "";
                        string fromPhone = "";
                        string fromAddress = "";
                        string fromWardCode = "";
                        int fromDistrictId = 0;
                        int weight = 0;
                        string name = "";

                        var p = context.Products.GetProductById(item.productId).FirstOrDefault();

                        var fromAcc = context.Accounts.GetAccountByAccountId(p.shop.accountId).FirstOrDefault();
                        fromName = fromAcc.shop.name;
                        fromPhone = fromAcc.user.phoneNumber;
                        fromAddress = o.address;
                        fromWardCode = fromAcc.user.ward;
                        fromDistrictId = fromAcc.user.district;
                        weight = p.weight;
                        name = p.name;
                        var orderRequestItem = new OrderRequest
                        {
                            PaymentTypeId = 1,
                            ServiceTypeId = 2,
                            RequiredNote = "CHOXEMHANGKHONGTHU",
                            FromName = fromName,
                            FromPhone = fromPhone,
                            FromAddress = fromAddress,
                            FromWardCode = fromWardCode,
                            FromDistrictId = fromDistrictId,

                            ToName = o.user.firstName + " " + o.user.lastName,
                            ToPhone = o.user.phoneNumber,
                            ToAddress = o.address,
                            ToWardCode = o.ward,
                            ToDistrictId = o.district,
                            COD = 0,

                            Weight = weight * item.quanity,
                            Length = 20,
                            Width = 20,
                            Height = 20,

                            Items = new List<OrderItemRequest>
                            {
                                new OrderItemRequest
                                {
                                    Name = name,
                                    Weight = weight,
                                    Quantity =item.quanity,
                                }
                            }
                        };
                        try
                        {
                            var orderResponse = await _ghnService.CreateShippingOrderAsync(orderRequestItem);
                            item.externalOrderCode = orderResponse.Data.OrderCode;
                            context.Update(item);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }

                    }
                    o.paymentStatus = "done";
                    context.Update(o);
                    context.SaveChanges();
                }
                return Json(response);
            }
            return Ok("transaction fail");
        }
    }
}
