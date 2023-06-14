using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Dtos.Book
{
    public class BookBorrowDTO
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 图书名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }


        /// <summary>
        /// ISBN
        /// </summary>
        public string ISBN { get; set; }

        /// <summary>
        /// 出版社
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        public string UserNumber { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string  UserName{ get; set; }
    }
}
