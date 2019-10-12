using ABPCodeGenerator.Templates;
using ABPCodeGenerator.Utilities;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ABPCodeGenerator.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly string projectName = "WIMI.BTL";
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
        public void GenerateCode(IEnumerable<dynamic> selectedDatabaseTableColumnList)
        {
            #region 权限
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
    }
}
