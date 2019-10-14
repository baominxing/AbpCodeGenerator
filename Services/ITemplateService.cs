using ABPCodeGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Services
{
    public interface ITemplateService : IService
    {
        List<dynamic> ListDatabaseTableName(string connectionString);

        List<ColumnInfo> ListDatabaseTableColumn(string connectionString, string databaseTableName);

        void GenerateCode(List<ColumnInfo> selectedDatabaseTableColumnList);
    }
}
