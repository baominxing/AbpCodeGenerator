using ABPCodeGenerator.Controllers.CPS8x;
using ABPCodeGenerator.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ABPCodeGenerator.Services
{
    public interface ICPS8xCodeGeneratorService : IService
    {
        List<dynamic> ListDatabaseTableName(ListDatabaseTableNameInputDto input);

        List<ColumnConfigInfo> ListDatabaseTableColumn(ListDatabaseTableColumnInputDto input);

        string GeneratePage(GeneratePageInputDto input);

        string GenerateDto(GeneratePageInputDto input);
    }
}
