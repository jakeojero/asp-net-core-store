using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Casestudy.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public int QtyBackOrdered { get; set; }
        public int QtyOrdered { get; set; }
        public int QtySold { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal SubTotal { get; set; }
        public string OrderDate { get; set; }
        public string UserId { get; set; }
        public string ProductName { get; set; }
    }
}
