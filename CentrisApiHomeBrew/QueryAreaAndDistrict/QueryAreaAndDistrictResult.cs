using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew.QueryAreaAndDistrict
{
    public class QueryAreaAndDistrictResult
    {
        public int Level { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public List<QueryAreaAndDistrictMatch> Matches { get; set; }
        public string Type { get; set; }
        public string TypeId { get; set; }
    }
}
