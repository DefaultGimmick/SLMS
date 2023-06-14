using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Entities
{
    public class EntityCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public List<EntityBook> Books { get; set; } // Reference to User
    }
}
