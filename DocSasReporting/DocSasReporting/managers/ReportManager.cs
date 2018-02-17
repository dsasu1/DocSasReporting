using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocSasReporting.Models;

namespace DocSasReporting.managers
{
    public class ReportManager
    {
        public static HashSet<string> NonGroupAggregate = new HashSet<string>()
        {
            {"COUNT"},
            {"SUM"},
            {"AVG"},
            {"MAX"},
            {"MIN"}
        };

        public static HashSet<string> GroupOnlyAggregate = new HashSet<string>()
        {
            {"GROUP"}
        };

        public static Dictionary<string, string> DateFunctionOrderByMap = new Dictionary<string, string>()
        {
            {"DATENAME=MONTH" , "MONTH"},
            {"DATENAME=WEEKDAY","DATEPART=WEEKDAY" }

        };

        //  public const string HyphenDelimeter = "-";
        public const string EqualDelimeter = "=";
        public const string SpaceDelimeter = " ";
        public const string CommaDelimeter = " , ";
        private const string LikeEquality = "like";
        private const string BetweenEquality = "between";
        private const string SqlSelect = "SELECT ";
        private const string SqlWhere = " WHERE ";
        private const string SqlTop = "TOP ";
        private const string SqlOrderBy = " ORDER BY ";
        private const string SqlGroupBy = " GROUP BY ";
        private const string SqlFrom = " FROM ";
        private const string SqlSelectAll = "*";
        public const string SqlDesc = " DESC";
        public const string SqlAsc = " ASC";
        public const string SqlOpenBracket = " [";
        public const string SqlClosedBracket = "] ";
        private const string SqlOpenHandleKey = "EXEC Syscfg_handleopenkey ";
        private const string SqlDecryptFormat = "(CONVERT(VARCHAR(max), Decryptbykey({0})) )";
        private const string CastToDate = "CAST({0} AS Date)";


        public enum WherePart : int
        {
            Condition = 0,
            Column = 1,
            Equality = 2,
            Value = 3
        }

        public static readonly Dictionary<string, string> Equality = new Dictionary<string, string>
        {
            {"equal", "="},
            {"notequal", "!="},
            {"less", "<"},
            {"lessequal", "<="},
            {"greater", ">"},
            {"greaterequal", ">="},
            {"isnull", "IS NULL"},
            {"isnotnull", "IS NOT NULL"},
            {"between", "BETWEEN"},
            {"like", "LIKE"}
        };

        public static readonly HashSet<string> NonParametizeEquality = new HashSet<string>()
        {
           {"isnull"},
           {"isnotnull"}
        };


        /// <summary>
        /// Gets the number of records.
        /// </summary>
        /// <param name="recordNumber">The record number.</param>
        /// <returns></returns>
        private static int GetNumberOfRecords(int? recordNumber)
        {
            var limit = 0;

            var limitValue = recordNumber;

            if (limitValue.HasValue && limitValue.Value > 0)
            {
                return limitValue.Value;
            }


            return limit;
        }

        /// <summary>
        /// Gets the where statement.
        /// </summary>
        /// <param name="whereCriteria">The where criteria.</param>
        /// <returns></returns>
        private static string GetWhereStatement(List<string> whereCriteria, List<string> dateTypeFilters = null)
        {
            var sb = new StringBuilder(SqlWhere);

            var count = 0;
            var criteriaCount = whereCriteria != null ? whereCriteria.Count : 0;
            var isOnlyOneCriteria = criteriaCount == 1;


            foreach (var item in whereCriteria)
            {
                var valueArray = item.Split(new string[] { EqualDelimeter }, StringSplitOptions.None);

                if (valueArray.Length.Equals(4))
                {
                    var condition = valueArray[(int)WherePart.Condition];
                    var column = valueArray[(int)WherePart.Column];
                    var equalValue = valueArray[(int)WherePart.Equality];
                    var equality = GetEquality(equalValue);
                    var value = GetValue(valueArray[(int)WherePart.Equality], column, valueArray[(int)WherePart.Value]);


                    var formattedColumn = SqlOpenBracket + column + SqlClosedBracket;

                    if (dateTypeFilters != null && dateTypeFilters.Any())
                    {
                        if (dateTypeFilters.Contains(column))
                        {
                            formattedColumn = string.Format(CastToDate, formattedColumn);
                        }
                    }

                    if (isOnlyOneCriteria)
                    {
                        condition = string.Empty;
                    }


                    sb.Append(string.Format(" {0} {1} {2} {3}", condition, formattedColumn, equality, NonParametizeEquality.Contains(equalValue) ? value : value + count));

                    if (!NonParametizeEquality.Contains(equalValue))
                    {
                        count++;
                    }


                }

            }

            if (criteriaCount.Equals(0))
            {
                return string.Empty;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the order by statement.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        /// <param name="isDesc">if set to <c>true</c> [is desc].</param>
        /// <returns></returns>
        private static string GetOrderByStatement(string orderBy, bool isDesc = false, bool isChart = false)
        {
            var order = SqlOrderBy;

            if (string.IsNullOrEmpty(orderBy))
            {
                return string.Empty;
            }

            var descAsc = isDesc ? SqlDesc : SqlAsc;

            return string.Concat(order, GetChartColumnNames(new List<string>() { orderBy }, false), descAsc);
        }

        /// <summary>
        /// Gets the equality.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string GetEquality(string value)
        {
            return Equality[value];
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="equality">The equality.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string GetValue(string equality, string column, string value)
        {
            if (NonParametizeEquality.Contains(equality))
            {
                return string.Empty;
            }
            else if (equality.Equals(BetweenEquality))
            {
                return string.Format("@{0}BTOne AND @{0}BTTwo", column.Trim().Replace(" ", ""));
            }


            return string.Format("@{0}", column.Trim().Replace(" ", ""));
        }

        /// <summary>
        /// Generates the where parameters.
        /// </summary>
        /// <param name="whereCriteria">The where criteria.</param>
        /// <returns></returns>
        private static Dictionary<string, object> GenerateWhereParams(List<string> whereCriteria, bool isChart = false)
        {
            var sqlParams = new Dictionary<string, object>();

            var count = 0;
            foreach (var item in whereCriteria)
            {
                var valueArray = item.Split(new string[] { EqualDelimeter }, StringSplitOptions.None);

                if (valueArray.Length.Equals(4))
                {
                    var condition = valueArray[(int)WherePart.Condition];
                    var column = valueArray[(int)WherePart.Column];
                    var equality = valueArray[(int)WherePart.Equality];
                    var value = valueArray[(int)WherePart.Value];

                    if (BetweenEquality.Equals(equality))
                    {
                        var valueSplit = value.Split(new string[] { "AND" }, StringSplitOptions.None);

                        if (valueSplit.Length > 1)
                        {
                            var valueOne = valueSplit[0];
                            var valueTwo = valueSplit[1];

                            sqlParams.Add(string.Format("@{0}BTOne", column.Trim().Replace(" ", "")), valueOne.Trim());
                            sqlParams.Add(string.Format("@{0}BTTwo", column.Trim().Replace(" ", "")), valueTwo.Trim());
                        }

                    }
                    else if (!NonParametizeEquality.Contains(equality))
                    {
                        sqlParams.Add(string.Format("@{0}", column.Trim().Replace(" ", "")) + count, equality.Equals(LikeEquality) ? string.Format("%{0}%", value) : value);
                        count++;
                    }

                }
            }

            return sqlParams;
        }

        /// <summary>
        /// Generates the table select table.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        private static string GenerateTableSelectTable(List<string> columns, string tableName, int limit = 0,  bool isChart = false)
        {
            var sb = new StringBuilder();

            sb.Append(string.Format(" {0} {1} ", SqlSelect, limit > 0 ? SqlTop + limit : string.Empty));

            if (isChart)
            {
                sb.Append(GetChartColumnNames(columns));

            }
            else
            {
                var count = 0;
                foreach (var item in columns)
                {
                    sb.Append(string.Format("{0}", count.Equals(0) ? string.Concat(SqlOpenBracket, item, SqlClosedBracket) : CommaDelimeter + string.Concat(SqlOpenBracket, item, SqlClosedBracket)));
                    count++;
                }

                //Means no column selection was made. Select all columns
                if (count.Equals(0))
                {
                    sb.Append(SqlSelectAll);
                }
            }


            sb.Append(SqlFrom + tableName);

            return sb.ToString();
        }

        private static string GetChartColumnNames(List<string> columns, bool isSelect = true)
        {
            StringBuilder build = new StringBuilder();
            var alias = string.Empty;
            var formattedColumn = string.Empty;
            var count = 0;
            foreach (var item in columns)
            {
                var splitArray = item.Split(new string[] { EqualDelimeter }, StringSplitOptions.None);

                if (splitArray != null)
                {
                    var column = string.Concat(SqlOpenBracket, splitArray[0], SqlClosedBracket);
                    if (splitArray.Length == 1)
                    {
                        build.Append(string.Format("{0}", count.Equals(0) ? column : CommaDelimeter + column));
                    }
                    else if (splitArray.Length == 2)
                    {
                        if (isSelect)
                        {
                            alias = string.Concat("AS ", SqlOpenBracket, splitArray[1], " of ", splitArray[0], SqlClosedBracket);
                        }

                        formattedColumn = string.Format("{0}({1}) {2}", splitArray[1], column, alias);
                        build.Append(string.Format("{0}", count.Equals(0) ? formattedColumn : CommaDelimeter + formattedColumn));

                    }
                    else if (splitArray.Length == 3)
                    {
                        if (isSelect)
                        {
                            alias = string.Concat("AS ", column);
                        }

                        formattedColumn = string.Format("{0}({1},{2}) {3}", splitArray[1], splitArray[2], column, alias);
                        build.Append(string.Format("{0}", count.Equals(0) ? formattedColumn : CommaDelimeter + formattedColumn));

                    }

                }

                count++;
            }

            return build.ToString();
        }



        private static string GenerateGroupByStatement(List<string> columns)
        {
            StringBuilder build = new StringBuilder(SqlGroupBy);

            build.Append(GetChartColumnNames(columns, false));


            return build.ToString();
        }

        /// <summary>
        /// Generates the query.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="sqlParams">The SQL parameters.</param>
        /// <returns></returns>
        private static string GenerateQuery(QueryOption queryOptions, out Dictionary<string, object> sqlParams, List<string> dateTypeFilters = null)
        {
            sqlParams = null;

            var limit = GetNumberOfRecords(queryOptions.NumberofRecords);

            var selectStatement = GenerateTableSelectTable(queryOptions.SelectColumns, queryOptions.TableName, limit,  queryOptions.IsChartQuery);

            var whereStatement = GetWhereStatement(queryOptions.WhereCriteria, dateTypeFilters);

            var groupStatement = string.Empty;
            if (queryOptions.IsChartQuery && queryOptions.GroupByColumns != null && queryOptions.GroupByColumns.Any())
            {
                groupStatement = GenerateGroupByStatement(queryOptions.GroupByColumns);
            }

            if (!string.IsNullOrWhiteSpace(whereStatement))
            {
                sqlParams = GenerateWhereParams(queryOptions.WhereCriteria);
            }

            var orderByStatement = GetOrderByStatement(queryOptions.OrderByColumn, queryOptions.IsOrderByDesc);

            return string.Concat(selectStatement, whereStatement, groupStatement, orderByStatement);

        }

        public static ReportResult RunQuery(QueryOption queryOption)
        {
            ReportResult rptResult = new ReportResult();
            Dictionary<string, object> sqlParams;

            var query = GenerateQuery(queryOption, out sqlParams, queryOption.DateTypeFilters);

            var data = ExecuteTextTable(query, queryOption.TableName, queryOption.ConnectionString, sqlParams, queryOption.ReportTimeOut);

            rptResult.ResultData = data;

            if (queryOption.IsGetDataColumns && data != null)
            {

                var listColumnNames = new List<string>();

                foreach (DataColumn column in data.Columns)
                {
                    listColumnNames.Add(column.ColumnName);
                }

                rptResult.ResultColums = listColumnNames;
            }

            return rptResult;
        }

        public static DataTable ExecuteTextTable(string textCommand, string tableName, string connString, Dictionary<string, object> htParams, int? sqlCmdTimeOut = null)
        {
            DataTable dtRet = new DataTable(tableName);

                using (var dc = new SqlConnection(connString))
                {
                    //Initialize this to empty for calls that don't pass parameters
                    if (htParams == null)
                        htParams = new Dictionary<string, object>();

                    using (var cmd = new SqlCommand(textCommand, dc))
                    {
                        if (sqlCmdTimeOut.HasValue)
                        {
                            cmd.CommandTimeout = sqlCmdTimeOut.Value;
                        }
                        cmd.CommandType = CommandType.Text;
                        AssignParameters(cmd, htParams);

                        var da = new SqlDataAdapter(cmd);

                        try
                        {
                            da.Fill(dtRet);
                        }
                        catch (Exception ex)
                        {
                          throw new Exception("ExecuteTextTable", ex);
                        }
                     
                    }
                }
            
            return dtRet;
        }

        private static void AssignParameters(SqlCommand cmd, Dictionary<string, object> htParams)
        {
            foreach (var paramKey in htParams.Keys)
            {
                var dataKey = paramKey;
                if (!dataKey.StartsWith("@"))
                    dataKey = "@" + dataKey;

                if (htParams[paramKey] != null)
                {
                    var paramValue = htParams[paramKey];

                   
                     cmd.Parameters.AddWithValue(dataKey, htParams[paramKey]);
                    
                }
            }
        }

    }
}
