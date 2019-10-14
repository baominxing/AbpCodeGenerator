using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Utilities
{
    public class TypeHelper
    {
        public static string SqlServerType2CSharpType(string sqlServerTypeString)
        {
            string typestring = string.Empty;

            switch (sqlServerTypeString.ToLower())
            {
                case "int":
                    typestring = "int";
                    break;
                case "text":
                    typestring = "string";
                    break;
                case "bigint":
                    typestring = "long";
                    break;
                case "binary":
                    typestring = "byte[]";
                    break;
                case "bit":
                    typestring = "bool";
                    break;
                case "char":
                    typestring = "string";
                    break;
                case "datetime":
                    typestring = "DateTime";
                    break;
                case "decimal":
                    typestring = "decimal";
                    break;
                case "float":
                    typestring = "double";
                    break;
                case "image":
                    typestring = "byte[]";
                    break;
                case "money":
                    typestring = "decimal";
                    break;
                case "nchar":
                    typestring = "string";
                    break;
                case "ntext":
                    typestring = "string";
                    break;
                case "numeric":
                    typestring = "decimal";
                    break;
                case "nvarchar":
                    typestring = "string";
                    break;
                case "real":
                    typestring = "Single";
                    break;
                case "smalldatetime":
                    typestring = "DateTime";
                    break;
                case "smallint":
                    typestring = "Int16";
                    break;
                case "smallmoney":
                    typestring = "decimal";
                    break;
                case "timestamp":
                    typestring = "DateTime";
                    break;
                case "tinyint":
                    typestring = "byte";
                    break;
                case "uniqueidentifier":
                    typestring = "Guid";
                    break;
                case "varbinary":
                    typestring = "byte[]";
                    break;
                case "varchar":
                    typestring = "string";
                    break;
                case "variant":
                    typestring = "object";
                    break;
                default:
                    typestring = "string";
                    break;
            }

            return typestring;
        }
    }
}
