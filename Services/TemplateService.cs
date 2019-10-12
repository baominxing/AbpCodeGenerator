using ABPCodeGenerator.Models;
using ABPCodeGenerator.Utilities;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ABPCodeGenerator.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly string projectName = "WIMI.BTL";
        private readonly IRazorViewEngine razorViewEngine;
        private readonly ITempDataProvider tempDataProvider;
        private readonly IServiceProvider serviceProvider;

        public TemplateService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            this.razorViewEngine = razorViewEngine;
            this.tempDataProvider = tempDataProvider;
            this.serviceProvider = serviceProvider;
        }

        public List<dynamic> ListDatabaseTableName(string connectionString)
        {
            var resultList = new List<dynamic>();
            var executeSql = $@"select object_id id,name text from sys.tables";
            var parameters = new { };

            var cachedResult = CacheHelper.Get<List<dynamic>>("ListDatabaseTableName");

            if (cachedResult != null)
            {
                resultList = cachedResult;

                return resultList;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                var queryList = conn.Query<dynamic>(executeSql, parameters).ToList();

                foreach (var item in queryList)
                {
                    resultList.Add(new { id = item.id, text = item.text });
                }
            }

            CacheHelper.Set("ListDatabaseTableName", resultList);

            return resultList;
        }

        public List<dynamic> ListDatabaseTableColumn(string connectionString, string databaseTableName)
        {
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
       'ColumnId' = A.colid,
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

            var cachedResult = CacheHelper.Get<List<dynamic>>($"ListDatabaseTableName#{databaseTableName}");

            if (cachedResult != null)
            {
                resultList = cachedResult;

                return resultList;
            }


            using (var conn = new SqlConnection(connectionString))
            {
                resultList = conn.Query<dynamic>(executeSql, sqlParameters).ToList();
            }

            CacheHelper.Set($"ListDatabaseTableName#{databaseTableName}", resultList);

            return resultList;
        }

        /// <summary>
        /// 生成运行代码
        /// </summary>
        /// <param name="selectedDatabaseTableColumnList"></param>
        public void GenerateCode(List<dynamic> selectedDatabaseTableColumnList)
        {

            var result = this.RenderToStringAsync("Templates/Template1", new TemplateViewModel() { Name = "Fred.Bao" });

            #region 权限
            //var template = "Hello @Model.Name, welcome to use RazorEngine!";
            //var result = Engine.Razor.RunCompile(template, "templateKey1", null, new { Name = "World" });
            //System.IO.File.WriteAllText($"/GeneratedCodes/{DateTime.Now.ToString("yyyyMMddHHmmss")}/outputPage.html", result);
            #endregion

            #region 菜单

            #endregion

            #region Views
            //.cshtml

            //.js
            #endregion

            #region Application

            #endregion

            #region MyRegion

            #endregion
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = this.serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = this.razorViewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, this.tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return sw.ToString();
            }
        }
    }
}
