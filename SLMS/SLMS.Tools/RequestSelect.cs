using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Tools
{
    public class RequestSelect
    {
        /// <summary>
        /// 图书名称
        /// </summary>
        public string TitleInput { get; set; }


        /// <summary>
        /// 作者
        /// </summary>
        public string AuthorInput { get; set; }


        /// <summary>
        /// 类别id
        /// </summary>
        public int CategoryInput { get; set; }


        ///<summary>
        /// 页码
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        public int Limit { get; set; }
    }
}
