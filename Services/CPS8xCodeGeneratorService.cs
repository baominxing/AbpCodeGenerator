using ABPCodeGenerator.Controllers.CPS8x;
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
    public class CPS8xCodeGeneratorService : ICPS8xCodeGeneratorService
    {
        private readonly string projectName = "WIMI.BTL";
        private readonly IRazorViewEngine razorViewEngine;
        private readonly ITempDataProvider tempDataProvider;
        private readonly IServiceProvider serviceProvider;

        public CPS8xCodeGeneratorService(
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            this.razorViewEngine = razorViewEngine;
            this.tempDataProvider = tempDataProvider;
            this.serviceProvider = serviceProvider;
        }

        public List<dynamic> ListDatabaseTableName(ListDatabaseTableNameInputDto input)
        {
            var resultList = new List<dynamic>();
            var executeSql = $@"select object_id id,name text from sys.tables order by text";
            var parameters = new { };

            using (var conn = new SqlConnection(input.ConnectionString))
            {
                var queryList = conn.Query<dynamic>(executeSql, parameters).ToList();

                foreach (var item in queryList)
                {
                    resultList.Add(new { id = item.id, text = item.text });
                }
            }

            return resultList;
        }

        public List<ColumnConfigInfo> ListDatabaseTableColumn(ListDatabaseTableColumnInputDto input)
        {
            var resultList = new List<ColumnConfigInfo>();
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


            using (var conn = new SqlConnection(input.ConnectionString))
            {
                resultList = conn.Query<ColumnConfigInfo>(executeSql, sqlParameters).ToList();
            }

            return resultList;
        }

        /// <summary>
        /// 生成运行代码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GenerateCode(GenerateCodeInputDto input)
        {
            var targetBaseFolder = Path.Combine($"GeneratedCodes/{DateTime.Now.ToString("yyyyMMdd")}");
            var targetSubFolder = string.Empty;
            var targetFile = string.Empty;
            var targetFilePath = string.Empty;
            var renderedString = string.Empty;
            var relativeFolderPath = string.Empty;

            if (!Directory.Exists(targetBaseFolder))
            {
                Directory.CreateDirectory(targetBaseFolder);
            }

            //清空目录
            foreach (var file in Directory.GetFiles(targetBaseFolder))
            {
                File.Delete(file);
            }

            #region Readme.txt
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = $"Readme.txt";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Readme", new ReadmeViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;

            #endregion

            #region Application/EntityDto
            targetSubFolder = Path.Combine($"{input.ProjectName}.Application/{input.ModuleName}/Dtos");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = $"{input.EntityName}Dto.cs";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Application/Dtos/Template_EntityDto", new EntityDtoViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;

            #endregion

            #region Application/EntityInputDto
            targetSubFolder = Path.Combine($"{input.ProjectName}.Application/{input.ModuleName}/Dtos");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = $"{input.EntityName}InputDto.cs";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Application/Dtos/Template_EntityInputDto", new EntityInputDtoViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;

            #endregion

            #region Application/IAppService
            targetSubFolder = Path.Combine($"{input.ProjectName}.Application/{input.ModuleName}");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = $"I{ input.EntityName}AppService.cs";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Application/Template_IAppService", new IAppServiceViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Application/AppService
            targetSubFolder = Path.Combine($"{input.ProjectName}.Application/{input.ModuleName}");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = $"{input.EntityName}AppService.cs";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Application/Template_AppService", new AppServiceViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/Controller
            targetSubFolder = Path.Combine($"{input.ProjectName}.Web/Controllers/{input.ModuleName}");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = $"{input.EntityName}Controller.cs";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Web/Controller/Template_Controller", new ControllerViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/ViewModel
            targetSubFolder = Path.Combine($"{input.ProjectName}.Web/Models/{input.ModuleName}");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = $"{input.EntityName}ViewModel.cs";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Web/ViewModel/Template_ViewModel", new ViewModelViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/CreateOrUpdateModal.cshtml
            targetSubFolder = Path.Combine($"{input.ProjectName}.Web/Views/{input.ModuleName}/{input.PageName}");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = "CreateOrUpdateModal.cshtml";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Web/Views/Template_CreateOrUpdateModalCshtml", new CreateOrUpdateModalCshtmlViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/CreateOrUpdateModal.js
            targetSubFolder = Path.Combine($"{input.ProjectName}.Web/Views/{input.ModuleName}/{input.PageName}");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = "CreateOrUpdateModal.js";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Web/Views/Template_CreateOrUpdateModalJs", new CreateOrUpdateModalJsViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/Index.cshtml
            targetSubFolder = Path.Combine($"{input.ProjectName}.Web/Views/{input.ModuleName}/{input.PageName}");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = "Index.cshtml";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Web/Views/Template_IndexCshtml", new IndexCshtmlViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region Web/Index.js
            targetSubFolder = Path.Combine($"{input.ProjectName}.Web/Views/{input.ModuleName}/{input.PageName}");
            relativeFolderPath = Path.Combine(targetBaseFolder, targetSubFolder);
            targetFile = "Index.js";

            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            if (File.Exists(Path.Combine(relativeFolderPath, targetFile)))
            {
                File.Delete(Path.Combine(relativeFolderPath, targetFile));
            }

            using (File.Create(Path.Combine(relativeFolderPath, targetFile)))
            {

            }

            renderedString = this.RenderToStringAsync("CPS8x/Templates/Web/Views/Template_IndexJs", new IndexJsViewModel()
            {
                ProjectName = input.ProjectName,
                ModuleName = input.ModuleName,
                PageName = input.PageName,
                EntityName = input.EntityName,
                EntityNamespace = input.EntityNamespace,
                EntityPrimaryKeyType = input.EntityPrimaryKeyType,
                TableName = input.TableName,
                Sorting = input.EntitySortingColumnName,
                ColumnConfigList = input.ColumnConfigList
            }).Result;

            targetFilePath = Path.Combine(relativeFolderPath, targetFile);

            File.WriteAllText(targetFilePath, renderedString, Encoding.UTF8);

            renderedString = string.Empty;
            #endregion

            #region 压缩生成的文件
            //清空原有文件
            var zipFilePath = Path.Combine(targetBaseFolder, $"{DateTime.Now.ToString("yyyyMMdd")}.zip");

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            var zip = new ZipHelper();

            zip.ZipFileFromDirectory(targetBaseFolder, zipFilePath, 5);
            #endregion

            return zipFilePath;
        }

        /// <summary>
        ///  通过模拟访问页面，生成返回的文件的字符串流
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
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

                await viewResult.View.RenderAsync(viewContext).ConfigureAwait(true);

                return sw.ToString();
            }
        }
    }
}
