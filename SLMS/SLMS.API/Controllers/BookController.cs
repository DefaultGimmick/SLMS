using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SLMS.Application.Books;
using SLMS.Infrastructure.Caching;
using SLMS.Infrastructure.MessageQueue;
using SLMS.Models.Dtos.Book;
using SLMS.Models.Entities;
using SLMS.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")] //允许跨域
    public class BookController : ControllerBase
    {
        private readonly IBookAppService _bookAppService;
        private readonly RedisContext m_redisContext;
        private readonly MessageProducer m_messageProducer;
        private readonly PageUtils _pageUtils;


        public BookController(IBookAppService bookAppService, RedisContext redisContext, MessageProducer messageProducer)
        {
            _bookAppService = bookAppService;
            m_redisContext = redisContext;
            m_messageProducer = messageProducer;
            _pageUtils = new PageUtils();
        }

        /// <summary>
        /// 获取图书列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("BookList")]
        public async Task<ActionResult> GetAllBookListAsync(RequestSelect select)
        {
            //判断缓存是否为空
            var bookDTOCache = await m_redisContext.GetObjectListAsync<BookDTO>("Books");
            if (bookDTOCache.Count == 0)
            {
                var booksDb = await _bookAppService.GetAllBookAsync();
                foreach (var item in booksDb)
                {
                    await m_redisContext.SetObjectListAsync("Books", item);
                }
            }
            await  m_redisContext.SetTimeoutAsync("Books", new TimeSpan(0, 1, 0));
            var bookDTOs = await m_redisContext.GetObjectListAsync<BookDTO>("Books");
            var tables = _pageUtils.Divide(select.Limit, select.Page, bookDTOs);
            int count = await _bookAppService.FindBookTotalCount();
            return Ok(new {msg = "成功",books = tables, totalCount =  count});
        }

        /// <summary>
        /// 获取图书详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BookDetails/{Id:int}")]
        public async Task<ActionResult<BookDetailsDTO>> GetBookDetails(int Id)
        {
            var books = await _bookAppService.GetBookDetailsAsync(Id);
            if (books == null)
            {
                return BadRequest("no bookdetail");
            }
            return Ok(books);
        }

        /// <summary>
        /// 根据条件查询图书
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("Query")]
        public async Task<ActionResult> GetBookByAuthor(RequestSelect input)
        {
            var bookDTOs = await _bookAppService.QueryBookAsync(input);
            var bookCount = await _bookAppService.QueryBookCountByInputAsync(input);
            if (bookDTOs == null){
                return BadRequest("no book");
            }
            var tables = _pageUtils.Divide(input.Limit, input.Page, bookDTOs);
            return Ok(new{msg = "成功",books = tables,totalCount = bookCount});
        }

        /// <summary>
        /// 显示借阅图书
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("Borrow/{Id:int}")]
        [Authorize]
        public async Task<ActionResult<BookBorrowDTO>> BorrowBook(int Id)
        {          
            var books = await _bookAppService.GetBookDetailsAsync(Id);
            var token = HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..];
            var user = await m_redisContext.GetObjectAsync<EntityUser>(token);
            if (user == null){
                return Unauthorized("用户信息已过期，请重新登录");
            }
            BookBorrowDTO book = new BookBorrowDTO()
            {
                ISBN = books.ISBN,
                Publisher = books.Publisher,
                Title = books.Title,
                Author = books.Author,
                UserNumber = user.UserNumber,
                UserName = user.UserName,
            };
            return Ok(book);
        }

        /// <summary>
        /// 借阅图书
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("Borrow")]
        [Authorize]
        public async Task<IActionResult> BorrowBook([FromBody] BookBorrowDTO input)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..];
            var user = await m_redisContext.GetObjectAsync<EntityUser>(token);
            if (user == null){
                return Unauthorized("用户信息已过期，请重新登录");
            }
            var book = await _bookAppService.FindBookByISBNAsync(input.ISBN);
            EntityBorrowRecord entity = new EntityBorrowRecord(){
                UserId = user.Id,
                BookId = book.Id,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddMonths(3),
            };
            await _bookAppService.AddBorrowRecordAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 添加图书
        /// </summary>
        /// <param name="bookDTO"></param>
        [HttpPost("Inventory")]
        [Authorize]
        public async Task<IActionResult> AddBook([FromBody] BookStorageDTO bookDTO)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..];
            var user = await m_redisContext.GetObjectAsync<EntityUser>(token);
            if (user == null){
                return Unauthorized("用户信息已过期，请重新登录");
            }
            if(user.UserType == 2){
                return Unauthorized("用户信息权限不够");
            }
            var Inventory = new EntityInventory{
                RemainingQuantity = bookDTO.TotalQuantity,
                TotalQuantity = bookDTO.TotalQuantity,
            };
            var book = new EntityBook{
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                ISBN = bookDTO.ISBN,
                Publisher = bookDTO.Publisher,
                Price = bookDTO.Price,
                BookshelfNumber = bookDTO.BookshelfNumber,
                CategoryId = bookDTO.CategoryId,
                UserId = 1,
                Inventory = Inventory
            };

            //发送到Rabbitmq中
            var json = JsonConvert.SerializeObject(book);
            var body = Encoding.UTF8.GetBytes(json);
            m_messageProducer.Publish("BookStorage",body);
            //写入redis
            await m_redisContext.SetObjectListAsync("Books", json);
            return Ok();
        }
    }
}
