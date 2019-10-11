using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Utilities
{
    public class AppConfig
    {
        public static string SqlServerConnectionString { get; set; }

        public class ErrorCodes
        {
            public const string NONE = "0";

            public const string ERROR = "1";
        }
    }
}
