using Microsoft.EntityFrameworkCore;
using SLMS.Models.Dtos.Category;
using SLMS.Models.SLMS.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLMS.Application.Categories
{
    public class CategoryAppService:ICategoryAppService
    {
        private readonly SLMSDBContext _dataContext;

        public CategoryAppService(SLMSDBContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// 获取类别集合
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoryDto>> GetCategoryListAsync()
        {
            var query =  _dataContext.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName
            });
                
            return await query.ToListAsync();
        }
    }
}
