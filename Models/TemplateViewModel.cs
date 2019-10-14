using ABPCodeGenerator.Utilities;
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

    public class ColumnInfo
    {
        private static readonly string String = "string";

        public string TableName { get; set; }
        public string PrimaryKey { get; set; }
        public string ColumnId { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string Length { get; set; }
        public string Precision { get; set; }
        public string Scale { get; set; }
        public string AllowNull { get; set; }
        public string Default { get; set; }
        public string IsIdentity { get; set; }
        public string Desc { get; set; }

        public string GetCSharpDataType()
        {
            return TypeHelper.SqlServerType2CSharpType(this.DataType) + (IsNullable() ? "?" : "");
        }

        public bool IsNullable()
        {
            return this.AllowNull.ToLower() == "y" && TypeHelper.SqlServerType2CSharpType(this.DataType).ToLower() != String;
        }
    }
}
