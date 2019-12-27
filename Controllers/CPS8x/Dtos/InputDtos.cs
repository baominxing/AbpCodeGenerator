using System.ComponentModel.DataAnnotations;

namespace ABPCodeGenerator.Controllers.CPS8x.Dtos
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
