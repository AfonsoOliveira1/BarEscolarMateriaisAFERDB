using System.Collections.Generic;

namespace APiConsumer.Models
{
    public class MENUDAY
    {
        public int id { get; set; }
        public int? menuweekid { get; set; }
        public DateOnly? date { get; set; }
        public string dateString { get; set; }
        public string option { get; set; }
        public string maindish { get; set; }
        public string soup { get; set; }
        public string dessert { get; set; }
        public string notes { get; set; }
        public int? maxseats { get; set; }

        public MENUWEEK menuweek { get; set; }
        public List<ORDERITEMS> orderitems { get; set; }
    }
}