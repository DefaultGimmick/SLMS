using SLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Dtos.Book
{
    public class BookDetailsDto
    {
        /// <summary>
        /// 主键：ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 图书名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// ISBN
        /// </summary>
        public string ISBN { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 出版社
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 剩余总数
        /// </summary>
        public int RemainingQuantity { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

    }
}
