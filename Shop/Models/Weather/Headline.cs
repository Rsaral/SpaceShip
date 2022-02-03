using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models.Weather
{
    public class Headline
    {
        public String EffectiveDate { get; set; }
        public Int64 EffectiveEpochDate { get; set; }
        public Int32 Severity { get; set; }
        public String Text { get; set; }
        public String Category { get; set; }
        public String EndDate { get; set; }
        public Int64 EndEpochDate { get; set; }
        public String MobileLink { get; set; }
        public String Link { get; set; }

}
}
