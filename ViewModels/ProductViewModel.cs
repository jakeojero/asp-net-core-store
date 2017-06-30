using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casestudy.Models;

namespace Casestudy.ViewModels
{
    public class ProductViewModel
    {
        public string BrandName { get; set; }
        public int BrandId { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string ProductName { get; set; }
        public string GraphicName { get; set; }
        public int Qty { get; set; }
        public int QtyOnBackOrder { get; set; }
        public int QtyOnHand { get; set; }
        public decimal MSRP { get; set; }
        public decimal CostPrice { get; set; }
        public string JsonData { get; set; }
    }
}
