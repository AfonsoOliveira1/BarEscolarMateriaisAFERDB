using System.Collections.Generic;

namespace APiConsumer.Models
{
    public class MENUWEEK
    {
        public int id { get; set; }
        public string weekstart { get; set; }

        public List<MENUDAY> menudays { get; set; }
    }
}