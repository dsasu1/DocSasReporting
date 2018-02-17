using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;

namespace DocSasReporting.managers
{
    public static class ExtManager
    {
        public static string ToJson(this object value)
        {
            var result = JsonConvert.SerializeObject(value);

            return result;
        }
    }
}
