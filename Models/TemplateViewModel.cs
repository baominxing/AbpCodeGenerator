using ABPCodeGenerator.Core.Entities;
using System.Collections.Generic;

namespace ABPCodeGenerator.Models
{
    public class TemplateViewModel
    {
        public string ProjectName = "WIMI.BTL";

        public string ModuleName = "";

        public string PageName = "";

        public string EntityName = "";

        public string EntityNamespace = "";

        public string EntityPrimaryKeyType = "";

        public string TableName = "";

        public string Sorting = "Id";

        public List<ColumnConfigInfo> ColumnConfigList = new List<ColumnConfigInfo>();
    }

    public class ReadmeViewModel : TemplateViewModel
    {

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

    public class ControllerViewModel : TemplateViewModel
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
