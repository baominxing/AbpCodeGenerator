using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ABPCodeGenerator.Filters;
using ABPCodeGenerator.Services;
using ABPCodeGenerator.Utilities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ABPCodeGenerator.Controllers
{
    public class GenerateCodeController : BaseController
    {
        private readonly ITemplateService templateService;

        public GenerateCodeController(
            ITemplateService templateService,
            ILogger<BaseController> logger) : base(logger)
        {
            this.templateService = templateService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ParameterNullOrEmptyFilter]
        public IActionResult ListDatabaseTableName(string connectionString)
        {
            var errorMessage = string.Empty;
            var errorCode = AppConfig.ErrorCodes.NONE;
            var resultList = new List<dynamic>();

            try
            {
                resultList = this.templateService.ListDatabaseTableName(connectionString);
            }
            catch (Exception ex)
            {
                errorCode = AppConfig.ErrorCodes.ERROR;
                errorMessage = ex.Message;
            }

            return new JsonResult(new { errorCode, errorMessage, data = resultList });
        }

        [HttpPost]
        [ParameterNullOrEmptyFilter]
        public IActionResult ListDatabaseTableColumn(string connectionString, string databaseTableName)
        {
            var errorMessage = string.Empty;
            var errorCode = AppConfig.ErrorCodes.NONE;
            var resultList = new List<dynamic>();

            try
            {
                resultList = this.templateService.ListDatabaseTableColumn(connectionString, databaseTableName);
            }
            catch (Exception ex)
            {
                errorCode = AppConfig.ErrorCodes.ERROR;
                errorMessage = ex.Message;
            }

            return new JsonResult(new { errorCode, errorMessage, data = JsonConvert.SerializeObject(resultList) });
        }

        [HttpPost]
        [ParameterNullOrEmptyFilter]
        public IActionResult GenerateCode(string connectionString, string databaseTableName, List<string> columnIdList)
        {
            var errorMessage = string.Empty;
            var errorCode = AppConfig.ErrorCodes.NONE;

            try
            {
                //取出所选表所有的列
                var originalDatabaseTableColumnList = this.templateService.ListDatabaseTableColumn(connectionString, databaseTableName);
                //取出页面上选择的列
                var selectedDatabaseTableColumnList = originalDatabaseTableColumnList.Where(s => columnIdList.Contains(s.ColumnId.ToString())).ToList();
                //取出页面上选择的列
                this.templateService.GenerateCode(selectedDatabaseTableColumnList);

            }
            catch (Exception ex)
            {
                errorCode = AppConfig.ErrorCodes.ERROR;
                errorMessage = ex.Message;
            }

            return new JsonResult(new { errorCode, errorMessage });
        }
    }
}