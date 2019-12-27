using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ABPCodeGenerator.Core.Entities;
using ABPCodeGenerator.Filters;
using ABPCodeGenerator.Services;
using ABPCodeGenerator.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ABPCodeGenerator.Controllers.CPS8x
{
    public class GenerateCodeController : BaseController
    {
        private readonly ICPS8xCodeGeneratorService templateService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public GenerateCodeController(
            ICPS8xCodeGeneratorService templateService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<BaseController> logger) : base(logger)
        {
            this.templateService = templateService;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View("~/Views/CPS8x/GenerateCode/Index.cshtml");
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
                resultList = this.templateService.ListDatabaseTableName(input);
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
            var zipFilePath = string.Empty;
            try
            {
                //取出所选表所有的列
                var originalDatabaseTableColumnList = this.templateService.ListDatabaseTableColumn(new ListDatabaseTableColumnInputDto() { ConnectionString = input.ConnectionString, TableName = input.TableName });
                //取出页面上选择的列
                var selectedDatabaseTableColumnList = originalDatabaseTableColumnList.Where(s => input.ColumnIdList.Contains(s.ColumnId)).ToList();
                //取出页面上选择的列
                zipFilePath = this.templateService.GenerateCode(selectedDatabaseTableColumnList, input);

            }
            catch (Exception ex)
            {
                errorCode = AppConfig.ErrorCodes.ERROR;
                errorMessage = ex.Message;
            }

            return new JsonResult(new { errorCode, errorMessage, zipFilePath = zipFilePath });
        }

        [ParameterNullOrEmptyFilter]
        public FileResult DownloadGeneratedZipFile(string relativeFilePath)
        {
            string filePath = Path.Combine(this.webHostEnvironment.ContentRootPath, relativeFilePath);

            FileStream fstrm = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);

            return File(fstrm, "application/octet-stream", Guid.NewGuid() + ".zip"); //welcome.txt是客户端保存的名字
        }
    }

    public class ListDatabaseTableNameInputDto
    {
        public string ConnectionString { get; set; }
    }

    public class ListDatabaseTableColumnInputDto
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
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