using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocSasReporting.DataSetModels
{
    [Table("AdvanceReportViews")]
    public class AdvanceReportView
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ReportViewId { get; set; }
        public string ViewName { get; set; }
        public string ViewFriendlyName { get; set; }
        public bool IsValid { get; set; }
    }
}
