using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocSasReporting.DataSetModels
{
    public class TableSchema
    {
        [Key]
        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }
    }
}
