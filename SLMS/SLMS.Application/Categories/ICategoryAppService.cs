using SLMS.Models.Dtos.Category;
using System.Collections.Generic;
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
