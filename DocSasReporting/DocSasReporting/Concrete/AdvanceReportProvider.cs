using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSasReporting.Models;
using DocSasReporting.managers;
using DocSasReporting.DataSetModels;
using DocSasReporting.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DocSasReporting.Concrete
{
    public class AdvanceReportProvider : IAdvanceReportProvider
    {
        private readonly ReportContext _context;
        public AdvanceReportProvider(ReportContext context)
        {
            _context = context;
        }

      
        public  async Task<IEnumerable<AdvanceReportView>> LoadReportTablesAsync()
        {
            try
            {
                return await _context.AdvanceReportViews.Where(x=>x.IsValid).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("LoadReportTables", ex);
            }
       
        }

        public async Task<IEnumerable<TableSchema>> GetTableSchemaAsync(int dataViewId)
        {
            try
            {
                List<TableSchema> tableSchema = null;

                var data = await  _context.AdvanceReportViews.Where(x => x.ReportViewId == dataViewId).ToListAsync();

                if (data.Any())
                {

                    StringBuilder sb = new StringBuilder("GetTableSchema");

                    var procParameter = new object[1] { data.First().ViewName };

                    sb.Append(" @p0");

                    var result = await _context.Set<TableSchema>().FromSql(sb.ToString(), parameters: procParameter).ToListAsync();

                    tableSchema = result;

                }

                return tableSchema;
            }
            catch (Exception ex)
            {

                throw new Exception("GetTableSchema", ex);
            }

        }


        public async Task<IEnumerable<AdvanceSavedReport>> LoadSavedReportsAsync()
        {
            try
            {
                var result = await _context.AdvanceSavedReports.ToListAsync();
                return result.OrderByDescending(x => x.ReportId).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception("LoadSavedReports", ex);
            }
        
        }

        public  async Task DeleteReportAsync(int reportId)
        {
            try
            {
                var report = await _context.AdvanceSavedReports.Where(x => x.ReportId == reportId).ToListAsync();

                if (report.Any())
                {
                    _context.AdvanceSavedReports.Remove(report.First());

                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {

                throw new Exception("DeleteReport", ex);
            }
            

        }


        public  async Task<ReportResult> RunReportAsync(SavedReport report, string connectionString = null)
        {
            if(string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = _context.ConnectionString;
            }
            var reportResult = new ReportResult();
            List<string> selectList = new List<string>();
            List<string> filterCriteria = new List<string>();
            List<string> groupCriteria = new List<string>();
            List<string> dateTypeFilters = new List<string>();

            try
            {
                bool isChart = false;
                string tableName = null;
                string orderByColumn = string.Empty;
                bool orderByIsDesc = false;
                if (report != null && report.ReportData != null)
                {
                    var data = await _context.AdvanceReportViews.Where(x => x.ReportViewId == report.ReportData.SelectedViewId).ToListAsync();
                    if (data.Any())
                    {
                        var table = data.First();
                        tableName = table.ViewName;
                        if (report.ReportData.SelectedFields != null && report.ReportData.SelectedFields.Any())
                        {
                            if (!report.ReportType.Equals(AdvanceReportType.List.ToString()))
                            {
                                isChart = true;
                                foreach (var item in report.ReportData.SelectedFields)
                                {
                                    if (ReportManager.GroupOnlyAggregate.Contains(item.SelectedFieldAggr))
                                    {
                                        selectList.Add(item.FieldName);
                                        groupCriteria.Add(item.FieldName);
                                    }
                                    else if (ReportManager.NonGroupAggregate.Contains(item.SelectedFieldAggr))
                                    {
                                        selectList.Add(string.Concat(item.FieldName, ReportManager.EqualDelimeter, item.SelectedFieldAggr));
                                    }
                                    else
                                    {
                                        selectList.Add(string.Concat(item.FieldName, ReportManager.EqualDelimeter, item.SelectedFieldAggr));
                                        groupCriteria.Add(string.Concat(item.FieldName, ReportManager.EqualDelimeter, item.SelectedFieldAggr));

                                    }
                                }

                            }
                            else
                            {
                                selectList = report.ReportData.SelectedFields.Select(x => x.FieldName).ToList();
                            }

                        }

                        if (report.ReportData.Filters != null && report.ReportData.Filters.Any())
                        {
                            filterCriteria = report.ReportData.Filters.Select(x => string.Concat(x.AndOr, ReportManager.EqualDelimeter, x.Field.FieldName, ReportManager.EqualDelimeter, x.Operator, ReportManager.EqualDelimeter, x.FieldValue)).ToList();
                            dateTypeFilters = report.ReportData.Filters.Where(x => x.Field.IsDate).Select(x => x.Field.FieldName).ToList();
                        }

                        if (report.ReportData.SortBy != null && !string.IsNullOrWhiteSpace(report.ReportData.SortBy.SortField))
                        {
                            if (isChart)
                            {
                                var field = report.ReportData.SelectedFields.FirstOrDefault(x => x.FieldName == report.ReportData.SortBy.SortField);
                                if (field != null)
                                {
                                    if (ReportManager.DateFunctionOrderByMap.ContainsKey(field.SelectedFieldAggr))
                                    {
                                        orderByColumn = string.Concat(field.FieldName, ReportManager.EqualDelimeter, ReportManager.DateFunctionOrderByMap[field.SelectedFieldAggr]);
                                        groupCriteria.Add(orderByColumn);
                                    }

                                }
                            }

                            if (string.IsNullOrWhiteSpace(orderByColumn))
                                orderByColumn = report.ReportData.SortBy.SortField;



                            orderByIsDesc = report.ReportData.SortBy.SortType.Equals(ReportManager.SqlDesc.Trim());

                        }

                        QueryOption queryOption = new QueryOption(connectionString, tableName, selectList)
                        {
                            WhereCriteria = filterCriteria,
                            OrderByColumn = orderByColumn,
                            IsOrderByDesc = orderByIsDesc,
                            GroupByColumns = groupCriteria,
                            IsChartQuery = isChart,
                            IsGetDataColumns = isChart,
                            DateTypeFilters = dateTypeFilters
                        };

                        reportResult = ReportManager.RunQuery(queryOption);

                    }

                }
            }
            catch (Exception ex)
            {

                throw new Exception("RunReport", ex);
            }
          
            return reportResult; ;
        }

        public async Task SaveReportAsync(SavedReport report)
        {
            try
            {
                if (report != null)
                {
                    var advanceSavedReport = new AdvanceSavedReport()
                    {
                        ReportName = report.ReportName,
                        ReportDesc = string.IsNullOrWhiteSpace(report.ReportDesc) ? string.Empty : report.ReportDesc,
                        ReportType = report.ReportType,
                        SavedDateUTC = DateTime.UtcNow,
                        ReportData = report.ReportData.ToJson()
                    };


                    if (report.ReportId.HasValue)
                    {
                        advanceSavedReport.ReportId = report.ReportId.Value;
                        _context.AdvanceSavedReports.Update(advanceSavedReport);

                    }
                    else
                    {
                        _context.AdvanceSavedReports.Add(advanceSavedReport);
                    }

                    await _context.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {

                throw new Exception("SaveReport", ex);
            }
            
        }

    }
}
