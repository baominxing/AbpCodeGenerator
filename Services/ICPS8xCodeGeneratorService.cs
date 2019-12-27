using ABPCodeGenerator.Controllers.CPS8x;
using ABPCodeGenerator.Core.Entities;
using System.Collections.Generic;

namespace ABPCodeGenerator.Services
{
    public interface ICPS8xCodeGeneratorService : IService
    {
        List<dynamic> ListDatabaseTableName(ListDatabaseTableNameInputDto input);

        List<ColumnInfo> ListDatabaseTableColumn(ListDatabaseTableColumnInputDto input);

        string GenerateCode(List<ColumnInfo> selectedDatabaseTableColumnList, GenerateCodeInputDto input);
    }
}
