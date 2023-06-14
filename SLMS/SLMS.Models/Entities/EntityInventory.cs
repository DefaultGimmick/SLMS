using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Entities
{
    public class EntityInventory
    {
        public int Id { get; set; }
        public int RemainingQuantity { get; set; }
        public int TotalQuantity { get; set; }
        public int BookId { get; set; } // Reference to Inventory
        public EntityBook Book { get; set; } // Reference to Inventory

    }
}
