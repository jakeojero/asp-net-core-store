using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casestudy.ViewModels;
using Newtonsoft.Json;

namespace Casestudy.Models
{
    public class OrderModel
    {
        private AppDbContext _db;

        public OrderModel(AppDbContext context)
        {
            _db = context;
        }

        public List<OrderViewModel> GetOrderDetails(int oid, string uid)
        {
            List<OrderViewModel> allDetails = new List<OrderViewModel>();
            // LINQ way of doing INNER JOINS
            var results = from t in _db.Set<Order>()
                          join oi in _db.Set<OrderLineItem>() on t.Id equals oi.OrderId
                          join p in _db.Set<Product>() on oi.ProductId equals p.Id
                          where (t.UserId == uid && t.Id == oid)
                          select new OrderViewModel
                          {
                              OrderId = t.Id,
                              UserId = uid,
                              ProductId = p.Id,
                              OrderAmount = t.OrderAmount,
                              Tax = t.OrderAmount - t.OrderAmount / 1.13M,
                              SubTotal = t.OrderAmount / 1.13M,
                              SellingPrice = oi.SellingPrice,
                              QtySold = oi.QtySold,
                              QtyBackOrdered = oi.QtyBackOrdered,
                              QtyOrdered = oi.QtyOrdered,
                              ProductName = p.ProductName,
                              OrderDate = t.OrderDate.ToString("yyyy/MM/dd - hh:mm tt")
                          };
            allDetails = results.ToList<OrderViewModel>();
            return allDetails;
        }
        public int AddOrder(Dictionary<string, object> items, string user, ref string addedMessage)
        {
            int orderId = -1;
            using (_db)
            {
                // Need a transaction as multiple entities are involved
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        Order order = new Order();
                        order.UserId = user;
                        order.OrderDate = System.DateTime.Now;
                        order.OrderAmount = 0;

                        // Loop to get Order Total
                        foreach (var key in items.Keys)
                        {
                            ProductViewModel product = JsonConvert.DeserializeObject<ProductViewModel>(Convert.ToString(items[key]));
                            if (product.Qty > 0)
                            {
                                decimal tax = 1.13M;
                                decimal total = (product.MSRP * product.Qty) * tax;
                                order.OrderAmount += total;
                            }
                        }
                        _db.Orders.Add(order);
                        _db.SaveChanges();

                        // Add each product to OrderItemsTable
                        foreach (var key in items.Keys)
                        {
                            ProductViewModel product = JsonConvert.DeserializeObject<ProductViewModel>(Convert.ToString(items[key]));
                            OrderLineItem oItem = new OrderLineItem
                            {
                                Order = order,
                                OrderId = order.Id,
                                ProductId = product.Id,
                                SellingPrice = product.MSRP
                            };


                            oItem.Product = _db.Products.FirstOrDefault(p => p.Id == product.Id);

                            // If theres enough stock
                            if (product.Qty <= product.QtyOnHand)
                            {
                                oItem.Product.QtyOnHand -= product.Qty;
                                _db.Products.Update(oItem.Product);

                                oItem.QtySold = product.Qty;
                                oItem.QtyOrdered = product.Qty;
                                oItem.QtyBackOrdered = 0;
                            }
                            // Not enough stock
                            else if (product.Qty > product.QtyOnHand)
                            {
                                oItem.Product.QtyOnHand = 0;
                                oItem.Product.QtyOnBackOrder += (product.Qty - product.QtyOnHand);
                                _db.Products.Update(oItem.Product);

                                oItem.QtySold = product.QtyOnHand;
                                oItem.QtyOrdered = product.Qty;
                                oItem.QtyBackOrdered = product.Qty - product.QtyOnHand;
                                addedMessage = "Some products were backordered.";
                            }

                            // Add OrderItem
                            _db.OrderItems.Add(oItem);
                            _db.SaveChanges();
                        }

                        _trans.Commit();
                        orderId = order.Id;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _trans.Rollback();
                    }
                }
            }
            return orderId;
        }
    }
}
