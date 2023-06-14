using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Entities
{
    public class EntityBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public decimal Price { get; set; }
        public int BookshelfNumber { get; set; }
        public int CategoryId { get; set; }
        public EntityCategory Category { get; set; }
        public int UserId { get; set; }
        public EntityUser User { get; set; }
        public EntityInventory Inventory { get; set; }
        public List<EntityBorrowRecord> BorrowRecords { get; set; }
    }
}
