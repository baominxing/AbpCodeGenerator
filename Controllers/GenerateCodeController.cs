using System;
using System.Collections.Generic;
using System.Linq;
using ABPCodeGenerator.Core.Entities;
using ABPCodeGenerator.Filters;
using ABPCodeGenerator.Services;
using ABPCodeGenerator.Utilities;
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
        public IActionResult ListDatabaseTableName(ListDatabaseTableNameInputDto input)
        {
            var errorMessage = string.Empty;
            var errorCode = AppConfig.ErrorCodes.NONE;
            var resultList = new List<dynamic>();

            try
            {
                resultList = this.templateService.ListDatabaseTableName(input.ConnectionString);
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
        public IActionResult ListDatabaseTableColumn(ListDatabaseTableColumnInputDto input)
        {
            var errorMessage = string.Empty;
            var errorCode = AppConfig.ErrorCodes.NONE;
            var resultList = new List<ColumnInfo>();

            try
            {
                resultList = this.templateService.ListDatabaseTableColumn(input);
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
        public IActionResult GenerateCode(GenerateCodeInputDto input)
        {
            var errorMessage = string.Empty;
            var errorCode = AppConfig.ErrorCodes.NONE;

            try
            {
                //取出所选表所有的列
                var originalDatabaseTableColumnList = this.templateService.ListDatabaseTableColumn(input);
                //取出页面上选择的列
                var selectedDatabaseTableColumnList = originalDatabaseTableColumnList.Where(s => input.ColumnIdList.Contains(s.ColumnId)).ToList();
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

    public class ListDatabaseTableNameInputDto
    {
        public string ConnectionString { get; set; }
    }

    public class ListDatabaseTableColumnInputDto
    {
        public string ConnectionString { get; internal set; }
        public object TableName { get; internal set; }
    }

    public class GenerateCodeInputDto
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

        public List<string> ColumnIdList { get; set; } = new List<string>();

        public List<ColumnInfo> AllColumnList { get; set; } = new List<ColumnInfo>();

        public List<ColumnInfo> SearchColumnList { get; set; } = new List<ColumnInfo>();

        public List<ColumnInfo> TableColumnList { get; set; } = new List<ColumnInfo>();
    }
}