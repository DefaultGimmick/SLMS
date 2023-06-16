using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SLMS.Application.Books;
using SLMS.Infrastructure.Caching;
using SLMS.Infrastructure.MessageQueue;
using SLMS.Models.Dtos.Book;
using SLMS.Models.Entities;
using SLMS.Tools;
using System;
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
        private readonly RedisContext _redisContext;
        private readonly MessageProducer _messageProducer;
        private readonly PageUtils _pageUtils;


        public BookController(IBookAppService bookAppService, RedisContext redisContext, MessageProducer messageProducer)
        {
            _bookAppService = bookAppService;
            _redisContext = redisContext;
            _messageProducer = messageProducer;
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
            var bookDtoCache = await _redisContext.GetObjectListAsync<BookDto>("Books");
            if (bookDtoCache.Count == 0)
            {
                var booksDb = await _bookAppService.GetAllBookAsync();
                foreach (var item in booksDb)
                {
                    await _redisContext.SetObjectListAsync("Books", item);
                }
            }
            await  _redisContext.SetTimeoutAsync("Books", new TimeSpan(0, 1, 0));
            var bookDtos = await _redisContext.GetObjectListAsync<BookDto>("Books");
            var tables = _pageUtils.Divide(select.Limit, select.Page, bookDtos);
            int count = await _bookAppService.FindBookTotalCount();
            return Ok(new {msg = "成功",books = tables, totalCount =  count});
        }

        /// <summary>
        /// 获取图书详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BookDetails/{id:int}")]
        public async Task<ActionResult<BookDetailsDto>> GetBookDetails(int id)
        {
            var books = await _bookAppService.GetBookDetailsAsync(id);
            if (books == null)
            {
                return BadRequest("no bookdetail");
            }
            return Ok(books);
        }

        /// <summary>
        /// 根据条件查询图书
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Query")]
        public async Task<ActionResult> GetBookByAuthor(RequestSelect input)
        {
            var bookDtos = await _bookAppService.QueryBookAsync(input);
            var bookCount = await _bookAppService.QueryBookCountByInputAsync(input);
            if (bookDtos == null){
                return BadRequest("no book");
            }
            var tables = _pageUtils.Divide(input.Limit, input.Page, bookDtos);
            return Ok(new{msg = "成功",books = tables,totalCount = bookCount});
        }

        /// <summary>
        /// 显示借阅图书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Borrow/{Id:int}")]
        [Authorize]
        public async Task<ActionResult<BookBorrowDto>> BorrowBook(int id)
        {          
            var books = await _bookAppService.GetBookDetailsAsync(id);
            var token = HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..];
            var user = await _redisContext.GetObjectAsync<EntityUser>(token);
            if (user == null){
                return Unauthorized("用户信息已过期，请重新登录");
            }
            BookBorrowDto book = new BookBorrowDto()
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
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Borrow")]
        [Authorize]
        public async Task<IActionResult> BorrowBook([FromBody] BookBorrowDto input)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..];
            var user = await _redisContext.GetObjectAsync<EntityUser>(token);
            if (user == null){
                return Unauthorized("用户信息已过期，请重新登录");
            }
            var book = await _bookAppService.FindBookByisbnAsync(input.ISBN);
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
        /// <param name="bookDto"></param>
        [HttpPost("Inventory")]
        [Authorize]
        public async Task<IActionResult> AddBook([FromBody] BookStorageDto bookDto)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString()["Bearer ".Length..];
            var user = await _redisContext.GetObjectAsync<EntityUser>(token);
            if (user == null){
                return Unauthorized("用户信息已过期，请重新登录");
            }
            if(user.UserType == 2){
                return Unauthorized("用户信息权限不够");
            }
            var inventory = new EntityInventory{
                RemainingQuantity = bookDto.TotalQuantity,
                TotalQuantity = bookDto.TotalQuantity,
            };
            var book = new EntityBook{
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                Publisher = bookDto.Publisher,
                Price = bookDto.Price,
                BookshelfNumber = bookDto.BookshelfNumber,
                CategoryId = bookDto.CategoryId,
                UserId = 1,
                Inventory = inventory
            };

            //发送到Rabbitmq中
            var json = JsonConvert.SerializeObject(book);
            var body = Encoding.UTF8.GetBytes(json);
            _messageProducer.Publish("BookStorage",body);
            //写入redis
            await _redisContext.SetObjectListAsync("Books", json);
            return Ok();
        }
    }
}
