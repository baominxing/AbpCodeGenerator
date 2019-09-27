using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ABPCodeGenerator.Models;
using ABPCodeGenerator.Utilities;
using Microsoft.AspNetCore.Cors;
using System.Data.SqlClient;
using Dapper;
using Newtonsoft.Json;

namespace ABPCodeGenerator.Controllers
{
    [EnableCors("AllowAll")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var connectionString = AppConfig.SqlServerConnectionString;

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

        [HttpGet]
        public IActionResult ListTableName()
        {
            var resultList = new List<dynamic>();
            var executeSql = $@"select object_id id,name text from sys.tables";
            var parameters = new { };

            using (var conn = new SqlConnection(AppConfig.SqlServerConnectionString))
            {
                var queryList = conn.Query<dynamic>(executeSql, parameters).ToList();

                foreach (var item in queryList)
                {
                    resultList.Add(new { id = item.id, text = item.text });
                }

            }

            return new JsonResult(resultList);
        }

        [HttpPost]
        public IActionResult ListTableColumn(ListTableColumnDto input)
        {
            if (string.IsNullOrEmpty(input.TableName))
            {
                throw new ArgumentNullException($"参数不能为空");
            }
            var tablename = input.TableName;
            var resultList = new List<dynamic>();
            var executeSql = $@"
SELECT TableName = CASE
                WHEN A.colorder = 1 THEN
                    D.name
                ELSE
                    ''
            END,
       PrimaryKey = CASE
                WHEN EXISTS
                     (
                         SELECT 1
                         FROM sysobjects
                         WHERE xtype = 'PK'
                               AND parent_obj = A.id
                               AND name IN (
                                               SELECT name
                                               FROM sysindexes
                                               WHERE indid IN (
                                                                  SELECT indid FROM sysindexkeys WHERE id = A.id AND colid = A.colid
                                                              )
                                           )
                     ) THEN
                    '√'
                ELSE
                    ''
            END,
       'ColumnName' = A.name,
       'DataType' = UPPER(B.name),
       'Length' = CASE
                      WHEN A.length = -1 THEN
                          'MAX'
                      ELSE
                          CONVERT(NVARCHAR(100), A.length)
                  END,
       'Precision' = CASE
                         WHEN COLUMNPROPERTY(A.id, A.name, 'PRECISION') = -1 THEN
                             0
                         ELSE
                             CONVERT(NVARCHAR(100), A.length)
                     END,
       'Scale' = ISNULL(COLUMNPROPERTY(A.id, A.name, 'Scale'), 0),
       'AllowNull' = CASE
                          WHEN A.isnullable = 1 THEN
                              'Y'
                          ELSE
                              'N'
                      END,
       'Default' = '',
       'IsIdentity' = CASE
                          WHEN COLUMNPROPERTY(A.id, A.name, 'IsIdentity') = 1 THEN
                              'Y'
                          ELSE
                              'N'
                      END,
       'Desc.' = ''
FROM syscolumns A
    LEFT JOIN systypes B
        ON A.xusertype = B.xusertype
    INNER JOIN sysobjects D
        ON A.id = D.id
           AND D.xtype = 'U'
           AND D.name <> 'dtproperties'
    LEFT JOIN syscomments E
        ON A.cdefault = E.id
    LEFT JOIN sys.extended_properties G
        ON A.id = G.major_id
           AND A.colid = G.minor_id
    LEFT JOIN sys.extended_properties F
        ON D.id = F.major_id
           AND F.minor_id = 0
WHERE D.name = @tablename
ORDER BY A.id,
         A.colorder;

";
            var sqlParameters = new { tablename = tablename };

            using (var conn = new SqlConnection(AppConfig.SqlServerConnectionString))
            {
                var queryList = conn.Query<dynamic>(executeSql, sqlParameters).ToList();

                foreach (var item in queryList)
                {
                    resultList.Add(new { id = item.id, text = item.text });
                }

            }

            return new JsonResult(resultList);
        }
    }

    public class ListTableColumnDto
    {
        public string TableName { get; set; }
    }
}
