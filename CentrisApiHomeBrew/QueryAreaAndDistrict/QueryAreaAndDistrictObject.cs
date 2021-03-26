using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew.QueryAreaAndDistrict
{
    public class QueryAreaAndDistrictObject
    {
        public List<QueryAreaAndDistrictResult> Result { get; set; }
        public bool Succeeded { get; set; }
        public object Error { get; set; }
        public object Message { get; set; }
    }
}
