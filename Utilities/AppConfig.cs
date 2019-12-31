using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Utilities
{
    public static class AppConfig
    {

        public static class ErrorCodes
        {
            public readonly static string NONE = "0";

            public readonly static string ERROR = "1";
        }

        public static class DatabaseTableColumn
        {
            public static bool IsPrimaryKey { get; set; }

            public static bool IsNullable { get; set; }
        }

        public static class CPS8x
        {
            public static readonly List<string> AuditedFieldList = new List<string> { "Id", "CreationTime", "CreatorUserId", "LastModifierUserId", "LastModificationTime", "IsDeleted", "DeleterUserId", "DeletionTime" };
        }
    }
}
