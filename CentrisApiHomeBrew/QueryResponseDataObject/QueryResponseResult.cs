using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew.QueryResponseDataObject
{
    public class QueryResponseResult
    {
        public string html { get; set; }
        public decimal count { get; set; }
        public int inscNumberPerPage { get; set; }
        public string title { get; set; }
    }
}
