using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew
{
    public class QueryResponseObjectResult
    {
        public string Message { get; set; }
        public QueryResponseResult Result { get; set; }
        public bool Succeeded { get; set; }
        public PageCounter PagerCounter { get; set; }
        public void InitPagerCounter ()
        {
            PagerCounter = new PageCounter();
            PagerCounter.Current = 1;
            PagerCounter.Last = Math.Ceiling(Convert.ToDecimal(Result.count / Result.inscNumberPerPage));
        }
    }
}
