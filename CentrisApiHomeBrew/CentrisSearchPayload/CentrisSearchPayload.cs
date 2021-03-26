using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew
{
    public class Filter
    {
        public string MatchType { get; set; }
        public string Text { get; set; }
        public string Id { get; set; }
    }

    public class FieldsValue
    {
        public string fieldId { get; set; }
        public object value { get; set; }
        public string fieldConditionId { get; set; }
        public string valueConditionId { get; set; }
    }

    public class Query
    {
        public int UseGeographyShapes { get; set; }
        public List<Filter> Filters { get; set; }
        public List<FieldsValue> FieldsValues { get; set; }
    }

    public class CentrisSearchPayload
    {
        public Query query { get; set; }
        public bool isHomePage { get; set; }
    }



}
