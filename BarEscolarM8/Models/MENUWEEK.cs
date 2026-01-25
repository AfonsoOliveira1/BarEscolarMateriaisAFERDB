using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APiConsumer.Models
{
    public class MENUWEEK
    {
        public int Id { get; set; }
        public string weekstart { get; set; }

        public List<MENUDAY> menudays { get; set; }
    }
}