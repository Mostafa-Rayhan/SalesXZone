using System;
using System.Collections.Generic;
using System.Text;

namespace SalesXZone.Application.Models
{
    public class ItemCreateRequest
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal? WholesellPrice { get; set; }
        public decimal? MrpPrice { get; set; }
        public int Quantity { get; set; } = 0;
        public string Category { get; set; }
        public int? CategoryId { get; set; }
        public string Unit { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
