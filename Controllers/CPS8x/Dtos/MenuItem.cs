using System.Collections.Generic;

namespace ABPCodeGenerator.Controllers.CPS8x.Dtos
{
    public class MenuItem
    {
        /// <summary>
        /// 是否是页面
        /// </summary>
        public bool IsPage { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 页面代码
        /// </summary>
        public string PageCode { get; set; }

        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 页面路径
        /// </summary>
        public string PageUrl { get; set; }

        public List<MenuItem> SubMenuItemList { get; set; } = new List<MenuItem>();
    }
}
