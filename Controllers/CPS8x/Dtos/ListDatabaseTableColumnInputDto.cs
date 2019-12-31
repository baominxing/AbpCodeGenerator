using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Controllers.CPS8x
{
    public class ListDatabaseTableColumnInputDto
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
    }
}
