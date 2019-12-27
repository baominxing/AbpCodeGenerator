using ABPCodeGenerator.Controllers.CPS8x.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABPCodeGenerator.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        public SidebarViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await Task.Run(() => { return ListMenuItem(); }).ConfigureAwait(true);

            return View(items);
        }

        private List<MenuItem> ListMenuItem()
        {
            return new List<MenuItem>()
            {
                new MenuItem()
                {
                    IsPage=false, ModuleName="CPS8.x代码生成管理", PageName="",PageUrl="", SubMenuItemList=new List<MenuItem>()
                    {
                        new MenuItem(){
                            IsPage = true, ModuleName = "CPS8.x代码生成管理", PageCode="Dashboard",PageName="首页说明",PageUrl="/Dashboard"
                        },
                        new MenuItem {
                            IsPage = true, ModuleName = "CPS8.x代码生成管理", PageCode="BasicSetting",PageName = "基础配置", PageUrl = "/BasicSetting"
                        },
                        new MenuItem {
                            IsPage = true, ModuleName = "CPS8.x代码生成管理", PageCode="GenerateCode",PageName = "代码生成", PageUrl = "/GenerateCode"
                        }
                    }
                },
            };
        }
    }
}
