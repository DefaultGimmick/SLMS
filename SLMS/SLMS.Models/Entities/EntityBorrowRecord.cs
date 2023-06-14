using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Entities
{
    public class EntityBorrowRecord
    {
        public int Id { get; set; }
        public int BookId { get; set; } // 借阅的图书ID
        public EntityBook Book { get; set; } // 借阅的图书
        public int UserId { get; set; } // 借阅用户ID
        public EntityUser User { get; set; } // 借阅用户
        public DateTime BorrowDate { get; set; } // 借阅日期
        public DateTime DueDate { get; set; } // 归还日期
    }
}
