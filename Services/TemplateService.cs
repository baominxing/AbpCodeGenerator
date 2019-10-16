using ABPCodeGenerator.Core.Entities;
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
using System.Text;
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
            var executeSql = $@"select object_id id,name text from sys.tables order by text";
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

        public List<ColumnInfo> ListDatabaseTableColumn(ListDatabaseTableColumnInputDto input)
        {
            var resultList = new List<ColumnInfo>();
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
WHERE D.name = @TableName
ORDER BY A.id,
         A.colorder;

";
            var sqlParameters = new { TableName = input.TableName };

            var cachedResult = CacheHelper.Get<List<ColumnInfo>>($"ListDatabaseTableName#{input.TableName}");

            if (cachedResult != null)
            {
                resultList = cachedResult;

                return resultList;
            }


            using (var conn = new SqlConnection(input.ConnectionString))
            {
                resultList = conn.Query<ColumnInfo>(executeSql, sqlParameters).ToList();
            }

            CacheHelper.Set($"ListDatabaseTableName#{input.TableName}", resultList);

            return resultList;
        }

        /// <summary>
        /// 生成运行代码
        /// </summary>
        /// <param name="selectedDatabaseTableColumnList"></param>
        public void GenerateCode(List<ColumnInfo> selectedDatabaseTableColumnList)
        {
            var targetFolder = Path.Combine($"GeneratedCodes/{DateTime.Now.ToString("yyyyMMdd")}");
            var targetFile = string.Empty;
            var renderedString = string.Empty;

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            #region Application/EntityDto
            targetFile = "OrderDto.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Application/Dtos/Template_EntityDto", new EntityDtoViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;

            #endregion

            #region Application/EntityInputDto
            targetFile = "OrderInputDto.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Application/Dtos/Template_EntityInputDto", new EntityInputDtoViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;

            #endregion

            #region Application/IAppService
            targetFile = "IOrderAppService.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Application/Template_IAppService", new IAppServiceViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Application/AppService
            targetFile = "OrderAppService.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Application/Template_AppService", new AppServiceViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Core/AuthorizationProvider
            targetFile = "BtlAuthorizationProvider.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Core/Authentication/Template_AuthorizationProvider", new AuthorizationProviderViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Core/PermissionNames
            targetFile = "PermissionNames.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Core/Authentication/Template_PermissionNames", new PermissionNamesViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/NavigationProvider
            targetFile = "NavigationProvider.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Web/App_Start/Navigation/Template_NavigationProvider", new NavigationProviderViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/PageNames
            targetFile = "PageNames.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Web/App_Start/Navigation/Template_PageNames", new PageNamesViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/Controller
            targetFile = "Controller.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Web/Controller/Template_Controller", new ControllerViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion


            #region Web/ViewModel
            targetFile = "ViewModel.cs";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Web/ViewModel/Template_ViewModel", new ViewModelViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/CreateOrUpdateModal.cshtml
            targetFile = "CreateOrUpdateModal.cshtml";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Web/Views/Template_CreateOrUpdateModalCshtml", new CreateOrUpdateModalCshtmlViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/CreateOrUpdateModal.js
            targetFile = "CreateOrUpdateModal.js";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Web/Views/Template_CreateOrUpdateModalJs", new CreateOrUpdateModalJsViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/Index.cshtml
            targetFile = "Index.cshtml";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Web/Views/Template_IndexCshtml", new IndexCshtmlViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/Index.js
            targetFile = "Index.js";

            if (File.Exists(Path.Combine(targetFolder, targetFile)))
            {
                File.Delete(Path.Combine(targetFolder, targetFile));

                using (File.Create(Path.Combine(targetFolder, targetFile)))
                {

                }
            }

            renderedString = this.RenderToStringAsync("Templates/Web/Views/Template_IndexJs", new IndexJsViewModel() { AllColumnList = selectedDatabaseTableColumnList }).Result;

            File.WriteAllText(Path.Combine(targetFolder, targetFile), renderedString, Encoding.UTF8);

            renderedString = string.Empty;
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
