using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ABPCodeGenerator.Filters;
using ABPCodeGenerator.Utilities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ABPCodeGenerator.Controllers
{
    public class TemplateController : BaseController
    {
        public TemplateController(ILogger<BaseController> logger) : base(logger)
        {
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
            var executeSql = $@"select object_id id,name text from sys.tables";
            var parameters = new { };

            try
            {

                using (var conn = new SqlConnection(connectionString))
                {
                    var queryList = conn.Query<dynamic>(executeSql, parameters).ToList();

                    foreach (var item in queryList)
                    {
                        resultList.Add(new { id = item.id, text = item.text });
                    }

                }
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
            var executeSql = $@"
SELECT TableName = D.name,
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
       'Desc' = ''
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
            var sqlParameters = new { tablename = databaseTableName };

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    resultList = conn.Query<dynamic>(executeSql, sqlParameters).ToList();
                }
            }
            catch (Exception ex)
            {
                errorCode = AppConfig.ErrorCodes.ERROR;
                errorMessage = ex.Message;
            }

            return new JsonResult(new { errorCode, errorMessage, data = JsonConvert.SerializeObject(resultList) });
        }
    }
}