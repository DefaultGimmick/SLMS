using SLMS.Models.Dtos.Category;
using SLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SLMS.Application.Categories
{
    public interface ICategoryAppService
    {
        /// <summary>
        /// 获取类别集合
        /// </summary>
        /// <returns></returns>
        public Task<List<CategoryDto>> GetCategoryListAsync();       
    }
}
