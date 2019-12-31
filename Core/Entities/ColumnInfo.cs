using ABPCodeGenerator.Utilities;
using System;

namespace ABPCodeGenerator.Core.Entities
{
    /// <summary>
    /// 列字段配置表
    /// </summary>
    public class ColumnConfigInfo
    {
        private static readonly string String = "STRING";
        private static readonly string Nullable = "Y";

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

        public bool IsSearchFiled { get; set; }

        public bool IsAuditFiled { get; set; }

        public bool IsEnumField { get; set; }

        public bool IsDtoFiled { get; set; } = true;

        public string EnumFieldName { get; set; }

        public string MultiLanguage_CN { get; set; }

        public string MultiLanguage_US { get; set; }

        public string GetCSharpDataType()
        {
            return TypeHelper.SqlServerType2CSharpType(this.DataType) + (IsNullable() ? "?" : "");
        }

        public bool IsNullable()
        {
            return AllowNull.ToLower() == Nullable.ToLower() && TypeHelper.SqlServerType2CSharpType(DataType).ToLower() != String.ToLower();
        }

        public dynamic GetColumnTypeDefaultValue()
        {
            var type = TypeHelper.SqlServerType2CSharpType(this.DataType);

            switch (type)
            {
                case "byte":
                case "short":
                case "int":
                case "long":
                case "decimal":
                case "float":
                case "double":
                case "Single":
                case "Int16":
                    return 0;
                case "string":
                    return "string.Empty";
                case "DateTime":
                    return "DateTime.Now";
                case "bool":
                    return "false";
                case "Guid":
                    return "Guid.NewGuid()";
                default:
                    return "string.Empty";
            }
        }

        public bool IsDateTimeType() => TypeHelper.SqlServerType2CSharpType(this.DataType).ToLower() == "DateTime".ToLower();

        public bool IsStringType() => TypeHelper.SqlServerType2CSharpType(this.DataType).ToLower() == "String".ToLower();

        public bool IsNumberType()
        {
            var type = TypeHelper.SqlServerType2CSharpType(this.DataType);

            switch (type)
            {
                case "byte":
                case "short":
                case "int":
                case "long":
                case "decimal":
                case "float":
                case "double":
                case "Single":
                case "Int16":
                    return true;
                case "string":
                case "DateTime":
                case "bool":
                case "Guid":
                default:
                    return false;
            }
        }
    }
}
