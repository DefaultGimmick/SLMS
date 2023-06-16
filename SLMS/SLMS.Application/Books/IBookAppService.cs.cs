using SLMS.Models.Dtos.Book;
using SLMS.Models.Entities;
using SLMS.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SLMS.Application.Books
{
    public interface IBookAppService
    {
        /// <summary>
        /// 获取图书列表
        /// </summary>
        /// <returns></returns>
        public Task<List<BookDto>> GetAllBookAsync();

        /// <summary>
        /// 新书入库
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public Task<bool> AddBookAsync(EntityBook book);
        /// <summary>
        /// 获取图书详细
        /// </summary>
        /// <returns></returns>
        public Task<BookDetailsDto> GetBookDetailsAsync(int id);

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<List<BookDto>> QueryBookAsync(RequestSelect input);

        /// <summary>
        /// 根据条件查询该图书总数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<int> QueryBookCountByInputAsync(RequestSelect input);

        /// <summary>
        /// 根据isbn查询图书
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public Task<EntityBook> FindBookByisbnAsync(string isbn);

        /// <summary>
        /// 添加借阅记录
        /// </summary>
        /// <param name="borrowRecord"></param>
        /// <returns></returns>
        public Task<bool> AddBorrowRecordAsync(EntityBorrowRecord borrowRecord);

        /// <summary>
        /// 查询图书总数
        /// </summary>
        /// <returns></returns>
        public Task<int> FindBookTotalCount();
    }
}
