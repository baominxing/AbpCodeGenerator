using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Controllers.Dtos
{


    public class ListTableColumnDto
    {
        public string TableName { get; set; }
    }

    public class ValidateSqlServerConnectionDto
    {
        [Required]
        public string SqlServerConnectionString { get; set; }
    }
}
