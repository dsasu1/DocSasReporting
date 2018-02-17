using DocSasReporting.DataSetModels;
using DocSasReporting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSasReporting.Concrete
{
    public interface IAdvanceReportProvider
    {

        Task<IEnumerable<AdvanceReportView>> LoadReportTablesAsync();
        Task<IEnumerable<AdvanceSavedReport>> LoadSavedReportsAsync();
        Task DeleteReportAsync(int reportId);
        Task<ReportResult> RunReportAsync(SavedReport report, string connectionString = null);
        Task SaveReportAsync(SavedReport report);
        Task<IEnumerable<TableSchema>> GetTableSchemaAsync(int dataViewId);
    }
}
