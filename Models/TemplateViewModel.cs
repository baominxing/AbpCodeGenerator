using ABPCodeGenerator.Core.Entities;
using System.Collections.Generic;

namespace ABPCodeGenerator.Models
{
    public class TemplateViewModel
    {
        public string ProjectName = "WIMI.BTL";

        public string ModuleName = "Order";

        public string PageName = "OrderMaintain";

        public string EntityName = "Order";

        public string EntityPrimaryKeyType = "long";

        public string TableName = "Orders";

        public string Sorting = "Id";

        public List<ColumnInfo> AllColumnList = new List<ColumnInfo>();

        public List<ColumnInfo> SearchColumnList = new List<ColumnInfo>();

        public List<ColumnInfo> TableColumnList = new List<ColumnInfo>();
    }

    public class EntityDtoViewModel : TemplateViewModel
    {

    }

    public class EntityInputDtoViewModel : TemplateViewModel
    {

    }

    public class IAppServiceViewModel : TemplateViewModel
    {

    }

    public class AppServiceViewModel : TemplateViewModel
    {

    }

    public class PermissionNamesViewModel : TemplateViewModel
    {

    }

    public class AuthorizationProviderViewModel : TemplateViewModel
    {

    }

    public class ControllerViewModel : TemplateViewModel
    {

    }

    public class NavigationProviderViewModel : TemplateViewModel
    {

    }

    public class PageNamesViewModel : TemplateViewModel
    {

    }

    public class ViewModelViewModel : TemplateViewModel
    {

    }

    public class CreateOrUpdateModalCshtmlViewModel : TemplateViewModel
    {

    }

    public class CreateOrUpdateModalJsViewModel : TemplateViewModel
    {

    }

    public class IndexCshtmlViewModel : TemplateViewModel
    {

    }

    public class IndexJsViewModel : TemplateViewModel
    {

    }
}
