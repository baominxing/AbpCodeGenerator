using ABPCodeGenerator.Controllers;
using ABPCodeGenerator.Core.Entities;
using ABPCodeGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Services
{
    public interface ITemplateService : IService
    {
        List<dynamic> ListDatabaseTableName(ListDatabaseTableNameInputDto input);

        List<ColumnInfo> ListDatabaseTableColumn(ListDatabaseTableColumnInputDto input);

        string GenerateCode(List<ColumnInfo> selectedDatabaseTableColumnList);
    }
}
