using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace E_Retalling_Portal.Services
{
    public class TransactionCleanupService : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("cleaning expired transactions");
                using (var context = new Context())
                {
                    var expiredTransactions = context.Orders
                        .Include(o => o.orderItems)
                            .ThenInclude(oi => oi.product)
                        .Include(o => o.orderItems)
                            .ThenInclude(oi => oi.productItem)
                        .Where(o => o.paymentStatus == "pending")
                            .AsEnumerable() 
                        .Where(o => !Util.TimeUtil.IsCurrentTimeBetween(o.createTime, o.endTime))
                            .ToList();

                    foreach (var transaction in expiredTransactions)
                    {
                        Console.WriteLine($"order {transaction.id} removed");
                        foreach (var orderItem in transaction.orderItems)
                        {
                            Console.WriteLine($"Include OrderItem {orderItem.id} removed");
                            // Update quantity of ProductItem (if exists)
                            if (orderItem.productItem != null)
                            {
                                orderItem.productItem.quantity += orderItem.quantity;
                                context.ProductItems.Update(orderItem.productItem);
                            }

                            // Update quantity of Product (if exists)
                            else if (orderItem.product != null)
                            {
                                orderItem.product.quantity += orderItem.quantity;
                                context.Products.Update(orderItem.product);
                            }
                        }
                    }

                    context.Orders.RemoveRange(expiredTransactions);
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        //var entry = ex.Entries.Single();
                        //var databaseEntry = await entry.GetDatabaseValuesAsync();

                        //entry.OriginalValues.SetValues(databaseEntry);
                    }
                }


                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }

    }
}

