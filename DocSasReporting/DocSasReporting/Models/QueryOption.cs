using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSasReporting.Models
{
    [Serializable]
    public class QueryOption
    {
        public QueryOption(string connectString, string tableName, List<string> selectColumns)
        {
            ConnectionString = connectString;
            TableName = tableName;
            SelectColumns = selectColumns;
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the numberof records.
        /// </summary>
        /// <value>
        /// The numberof records.
        /// </value>
        public int? NumberofRecords { get; set; }

        /// <summary>
        /// Gets or sets the select columns.
        /// </summary>
        /// <value>
        /// The select columns.
        /// </value>
        public List<string> SelectColumns { get; set; }

        /// <summary>
        /// Gets or sets the where criteria.
        /// </summary>
        /// <value>
        /// The where criteria.
        /// </value>
        public List<string> WhereCriteria { get; set; }

        /// <summary>
        /// Gets or sets the group by columns.
        /// </summary>
        /// <value>
        /// The group by columns.
        /// </value>
        public List<string> GroupByColumns { get; set; }

        /// <summary>
        /// Gets or sets the order by column.
        /// </summary>
        /// <value>
        /// The order by column.
        /// </value>
        public string OrderByColumn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is order by desc.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is order by desc; otherwise, <c>false</c>.
        /// </value>
        public bool IsOrderByDesc { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is chart query.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is chart query; otherwise, <c>false</c>.
        /// </value>
        public bool IsChartQuery { get; set; }

        /// <summary>
        /// Gets or sets the report time out.
        /// </summary>
        /// <value>
        /// The report time out.
        /// </value>
        public int? ReportTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is get data columns.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is get data columns; otherwise, <c>false</c>.
        /// </value>
        public bool IsGetDataColumns { get; set; }

        /// <summary>
        /// Gets or sets the date type filters.
        /// </summary>
        /// <value>
        /// The date type filters.
        /// </value>
        public List<string> DateTypeFilters { get; set; }


  
    }
}
