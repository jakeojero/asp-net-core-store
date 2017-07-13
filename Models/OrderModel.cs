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

                        foreach (var key in items.Keys)
                        {
                            ProductViewModel product =
                                JsonConvert.DeserializeObject<ProductViewModel>(Convert.ToString(items[key]));
                            if (product.Qty > 0)
                            {
                                order.OrderAmount += (product.MSRP * product.Qty);
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


                            Product retProduct = _db.Products
                                .FirstOrDefault(p => p.ProductName == product.ProductName);

                            if (product.Qty < product.QtyOnHand)
                            {
                                retProduct.QtyOnHand -= product.Qty;
                                _db.Products.Update(retProduct);

                                oItem.QtySold = product.Qty;
                                oItem.QtyOrdered = product.Qty;
                                oItem.QtyBackOrdered = 0;
                            }
                            else if (product.Qty > product.QtyOnHand)
                            {
                                retProduct.QtyOnHand = 0;
                                retProduct.QtyOnBackOrder += (product.Qty - product.QtyOnHand);
                                _db.Products.Update(retProduct);

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
