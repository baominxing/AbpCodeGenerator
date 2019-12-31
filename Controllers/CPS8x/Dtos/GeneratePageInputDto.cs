using ABPCodeGenerator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Controllers.CPS8x
{
    public class GeneratePageInputDto
    {
        public string ConnectionString { get; set; }

        public string ProjectName { get; set; }

        public string ModuleName { get; set; }

        public string PageName { get; set; }

        public string EntityName { get; set; }

        public string EntityPrimaryKeyType { get; set; }

        public string TableName { get; set; }

        public string EntityNamespace { get; set; }

        public string EntitySortingColumnName { get; set; }

        public List<ColumnConfigInfo> ColumnConfigList { get; set; } = new List<ColumnConfigInfo>();
    }
}
