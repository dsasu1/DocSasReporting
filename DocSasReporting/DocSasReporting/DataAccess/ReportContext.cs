using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSasReporting.DataSetModels;

namespace DocSasReporting.DataAccess
{
    public class ReportContext : DbContext
    {
        public readonly string ConnectionString;
        public ReportContext(DbContextOptions<ReportContext> options): base(options)
        {
            foreach (var item in options.Extensions)
            {
                if (item is Microsoft.EntityFrameworkCore.Infrastructure.Internal.SqlServerOptionsExtension)
                {
                    var sqlExt = item as Microsoft.EntityFrameworkCore.Infrastructure.Internal.SqlServerOptionsExtension;

                    ConnectionString = sqlExt.ConnectionString;
                  
                }
               
            }
        }

        public DbSet<AdvanceReportView> AdvanceReportViews { get; set; }
        public DbSet<AdvanceSavedReport> AdvanceSavedReports { get; set; }

        public DbSet<TableSchema> TableSchema { get; set; }

       //
    }
}
