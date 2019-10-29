using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ABPCodeGenerator.Models;
using ABPCodeGenerator.Utilities;
using Microsoft.AspNetCore.Cors;
using System.Data.SqlClient;
using ABPCodeGenerator.Filters;

namespace ABPCodeGenerator.Controllers
{
    [EnableCors("AllowAll")]
    public class BasicSettingController : BaseController
    {
        public BasicSettingController(ILogger<BaseController> logger) : base(logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 验证输入的数据库字符串是否能够正确的连接到数据库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ParameterNullOrEmptyFilter]
        public IActionResult ValidateConnectionString(string connectionString)
        {
            var errorMessage = string.Empty;
            var errorCode = AppConfig.ErrorCodes.NONE;

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                }
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
