namespace SLMS.Models.Dtos.Book
{
    public class BookStorageDto
    {
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
        /// 总数
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 类别id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 书架号
        /// </summary>
        public int BookshelfNumber { get; set; }
    }
}
