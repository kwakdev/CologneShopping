using CaseStudyAPI.DAL.DomainClasses;
using CaseStudyAPI.Helpers;

namespace CaseStudyAPI.DAL.DAO
{
    public class OrderDAO
    {
        private readonly AppDbContext _db;
        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<int> AddOrder(int customerId, OrderSelectionHelper[] selections)
        {
            int orderId = -1;
            // we need a transaction as multiple entities involved
            using (var _trans = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    // Create and add the order
                    Order order = new()
                    {
                        CustomerId = customerId,
                        OrderDate = DateTime.Now,
                        OrderAmount = 0
                    };

                    // Calculate the total price and then add the order row to the table
                    foreach (OrderSelectionHelper selection in selections)
                    {
                        order.OrderAmount += selection.Item!.MSRP * selection.Qty;
                    }
                    await _db.Orders!.AddAsync(order);
                    await _db.SaveChangesAsync();

                    // Add each line item to the orderitems table
                    foreach (OrderSelectionHelper selection in selections)
                    {
                        CologneItem product = await _db.CologneItems.FindAsync(selection.Item!.Id);
                        if (product == null)
                        {
                            throw new Exception("Product not found");
                        }

                        // Check stock availability
                        if (selection.Qty <= product.QtyOnHand)
                        {
                            product.QtyOnHand -= selection.Qty;
                        }
                        else
                        {
                            product.QtyOnBackOrder += selection.Qty - product.QtyOnHand;
                            product.QtyOnHand = 0;
                        }

                        // Create and add the line item
                        OrderLineitem oItem = new()
                        {
                            OrderId = order.Id,
                            ProductId = selection.Item.Id,
                            QtyOrdered = selection.Qty,
                            QtySold = Math.Min(selection.Qty, product.QtyOnHand + product.QtyOnBackOrder),
                            QtyBackOrdered = selection.Qty > product.QtyOnHand ? selection.Qty - product.QtyOnHand : 0,
                            SellingPrice = selection.Item.MSRP
                        };

                        await _db.OrderLines!.AddAsync(oItem);
                        await _db.SaveChangesAsync();
                    }

                    await _trans.CommitAsync();
                    orderId = order.Id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await _trans.RollbackAsync();
                }
            }
            return orderId;
        }
    }
}

