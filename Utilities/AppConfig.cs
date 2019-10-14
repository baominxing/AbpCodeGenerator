using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Utilities
{
    public class AppConfig
    {

        public class ErrorCodes
        {
            public const string NONE = "0";

            public const string ERROR = "1";
        }

        public class DatabaseTableColumn
        {
            public bool IsPrimaryKey { get; set; }

            public bool IsNullable { get; set; }
        }
    }
}
