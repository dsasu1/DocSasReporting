using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSasReporting.Models
{
    [Serializable]
    public class ReportResult
    {

        public object ResultData { get; set; }
        public object ResultColums { get; set; }
    }


    [Serializable]
    public class SavedReport
    {
        public int? ReportId { get; set; }
        public string ReportName { get; set; }

        public string ReportType { get; set; }
        public string ReportDesc { get; set; }

        public string SelectedViewName { get; set; }

        public ReportDataObject ReportData { get; set; }


    }

    [Serializable]
    public class ReportDataObject
    {
        public int SelectedViewId { get; set; }

        public List<Field> SelectedFields { get; set; }

        public List<Filter> Filters { get; set; }

        public Sort SortBy { get; set; }
    }

    [Serializable]
    public class Field
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }

        public bool IsDate { get; set; }

        public string SelectedFieldAggr { get; set; }

    }

    [Serializable]
    public class Sort
    {
        public string SortField { get; set; }

        public string SortType { get; set; }
    }

    [Serializable]
    public class FieldAggregate
    {
        public string Aggregate { get; set; }

        public string Title { get; set; }
    }


    [Serializable]
    public class Filter
    {
        public string AndOr { get; set; }
        public Field Field { get; set; }

        public string Operator { get; set; }

        public object FieldValue { get; set; }
    }

    public enum AdvanceReportType
    {
        List,
        Line,
        Bar,
        Pie
    }
}
