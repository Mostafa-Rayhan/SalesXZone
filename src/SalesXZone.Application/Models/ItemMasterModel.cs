using System;
using System.Collections.Generic;
using System.Text;

namespace SalesXZone.Application.Models
{
    public class ItemMasterModel
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal WholesellPrice { get; set; }
        public decimal MrpPrice { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } // from join
        public string Unit { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
