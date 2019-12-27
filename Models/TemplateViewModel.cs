using ABPCodeGenerator.Core.Entities;
using System.Collections.Generic;

namespace ABPCodeGenerator.Models
{
    public class TemplateViewModel
    {
        public string ProjectName = "WIMI.BTL";

        public string ModuleName = "TestEntity";

        public string PageName = "TestEntity";

        public string EntityName = "TestEntity";

        public string EntityNamespace = "WIMI.BTL.TestEntity";

        public string EntityPrimaryKeyType = "int";

        public string TableName = "TestEntities";

        public string Sorting = "Id";

        public List<ColumnInfo> AllColumnList = new List<ColumnInfo>();

        public List<ColumnInfo> SearchColumnList = new List<ColumnInfo>();

        public List<ColumnInfo> TableColumnList = new List<ColumnInfo>();
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
