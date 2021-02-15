using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew
{
    public class Property
    {
        public string MLS { get; set; }
        public string Price { get; set; }
        public string Address { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public string NbBathroom { get; set; }
        public string NbBedroom { get; set; }
        public bool IsNewProperty { get; set; }
    }
}
