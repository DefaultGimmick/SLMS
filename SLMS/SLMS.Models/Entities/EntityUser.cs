using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Entities
{
    public class EntityUser
    {
        public int Id { get; set; }
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public List<EntityBook> Books { get; set; }
        public List<EntityBorrowRecord> BorrowRecords { get; set; }
    }
}
