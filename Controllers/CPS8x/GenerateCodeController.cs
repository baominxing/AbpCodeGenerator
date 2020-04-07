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
            var resultList = new List<ColumnConfigInfo>();

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
        public IActionResult GenerateCode(GeneratePageInputDto input)
        {
            var errorMessage = string.Empty;
            var errorCode = AppConfig.ErrorCodes.NONE;
            var zipFilePath = string.Empty;
            try
            {
                //标识审计属性
                foreach (var column in input.ColumnConfigList)
                {
                    if (AppConfig.CPS8x.AuditedFieldList.Contains(column.ColumnName))
                    {
                        column.IsAuditFiled = true;
                    }
                }

                //取出页面上选择的列
                zipFilePath = this.templateService.GeneratePage(input);
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
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/octet-stream", Guid.NewGuid() + ".zip"); //welcome.txt是客户端保存的名字

        }
    }
}