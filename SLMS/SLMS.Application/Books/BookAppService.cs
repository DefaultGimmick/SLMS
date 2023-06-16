using Microsoft.EntityFrameworkCore;
using SLMS.Models.Dtos.Book;
using SLMS.Models.Entities;
using SLMS.Models.SLMS.EntityFrameworkCore;
using SLMS.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLMS.Application.Books
{
    public class BookAppService:IBookAppService
    {
        private readonly SLMSDBContext _dataContext;

        public BookAppService(SLMSDBContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// 获取图书列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<BookDto>> GetAllBookAsync()
        {
            var query = _dataContext.Books.Select(c => new BookDto
            {
                Id = c.Id,
                ISBN = c.ISBN,
                Author = c.Author,
                Title = c.Title,
                RemainingQuantity = c.Inventory.RemainingQuantity,
                TotalQuantity = c.Inventory.TotalQuantity,
                CategoryName = c.Category.CategoryName,
            }); 
            return await query.ToListAsync();
        }

        /// <summary>
        /// 新书入库
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public async Task<bool> AddBookAsync(EntityBook book)
        {
            var query = _dataContext.Books.Include(c => c.Inventory).FirstOrDefault(c => c.ISBN == book.ISBN);
            if(query != null)
            {
                query.Inventory.RemainingQuantity += book.Inventory.TotalQuantity;
                query.Inventory.TotalQuantity += book.Inventory.TotalQuantity;
                _dataContext.SaveChanges();
                return true;
            }
            _dataContext.Books.Add(book);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 获取图书详细
        /// </summary>
        /// <returns></returns>
        public async Task<BookDetailsDto> GetBookDetailsAsync(int id)
        {
            var query = _dataContext.Books.Select(c => new BookDetailsDto
            {
                Id = c.Id,
                ISBN = c.ISBN,
                Author = c.Author,
                Title = c.Title,
                Price = c.Price,
                Publisher = c.Publisher,
                RemainingQuantity = c.Inventory.RemainingQuantity,
                TotalQuantity = c.Inventory.TotalQuantity,
                CategoryName = c.Category.CategoryName
            }).FirstOrDefaultAsync(c => c.Id == id);

            return await query;
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<BookDto>> QueryBookAsync(RequestSelect input)
        {
            var query = _dataContext.Books.Select(c => new BookDto
            {
                Id = c.Id,
                ISBN = c.ISBN,
                Author = c.Author,
                Title = c.Title,
                RemainingQuantity = c.Inventory.RemainingQuantity,
                TotalQuantity = c.Inventory.TotalQuantity,
                CategoryId = c.CategoryId,
                CategoryName = c.Category.CategoryName
            });

            if (!string.IsNullOrEmpty(input.AuthorInput) && input.CategoryInput != 0)
            {
                query = query.Where(c => c.Author == input.AuthorInput);
            }

            if (!string.IsNullOrEmpty(input.TitleInput) && input.CategoryInput != 0)
            {
                query = query.Where(c => c.Title.Contains(input.TitleInput));
            }

            if (input.CategoryInput != 0 && string.IsNullOrEmpty(input.TitleInput) && string.IsNullOrEmpty(input.AuthorInput))
            {
                query = query.Where(c => c.CategoryId == input.CategoryInput);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// 根据条件查询该图书总数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> QueryBookCountByInputAsync(RequestSelect input)
        {
            int query = await _dataContext.Books.CountAsync();

            if (!string.IsNullOrEmpty(input.AuthorInput) && input.CategoryInput != 0)
            {
                query = await _dataContext.Books.CountAsync(c => c.Author == input.AuthorInput);
            }

            if (!string.IsNullOrEmpty(input.TitleInput) && input.CategoryInput != 0)
            {
                query = await _dataContext.Books.CountAsync(c => c.Title.Contains(input.TitleInput));
            }

            if (input.CategoryInput != 0 && string.IsNullOrEmpty(input.TitleInput) && string.IsNullOrEmpty(input.AuthorInput))
            {
                query = await _dataContext.Books.CountAsync(c => c.CategoryId == input.CategoryInput);
            }
            return query;
        }

        /// <summary>
        /// 根据isbn查询图书
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public async Task<EntityBook> FindBookByisbnAsync(string isbn)
        {
            var user =  _dataContext.Books.FirstOrDefaultAsync(x => x.ISBN == isbn);
            return await user;
        }

        /// <summary>
        /// 添加借阅记录
        /// </summary>
        /// <param name="borrowRecord"></param>
        /// <returns></returns>
        public async Task<bool> AddBorrowRecordAsync(EntityBorrowRecord borrowRecord)
        {
             _dataContext.BorrowRecords.Add(borrowRecord);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 查询图书总数
        /// </summary>
        /// <returns></returns>
        public async Task<int> FindBookTotalCount()
        {
            return await _dataContext.Books.CountAsync();
        }
    }
}
