using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocSasReporting.DataSetModels
{
    [Table("AdvanceSavedReports")]
    public class AdvanceSavedReport
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportDesc { get; set; }
        public string ReportType { get; set; }
        public string ReportData { get; set; }
        public DateTime SavedDateUTC { get; set; }
    }
}
