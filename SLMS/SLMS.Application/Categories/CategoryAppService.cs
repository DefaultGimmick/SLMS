using Microsoft.EntityFrameworkCore;
using SLMS.Models.Dtos.Category;
using SLMS.Models.Entities;
using SLMS.Models.SLMS.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var query =  _dataContext.Categorys.Select(c => new CategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName
            });
                
            return await query.ToListAsync();
        }
    }
}
