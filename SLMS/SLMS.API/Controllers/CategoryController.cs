using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SLMS.Application.Categories;
using SLMS.Models.Dtos.Category;
using SLMS.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")] //允许跨域
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryAppService _categoryAppService;

        public CategoryController(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }

        /// <summary>
        /// 获取类别集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CategoryList")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategoryList()
        {
            var category = await _categoryAppService.GetCategoryListAsync();
            if (category == null)
            {
                return BadRequest(string.Empty);
            }
            return Ok(category);
        }
    }
}
