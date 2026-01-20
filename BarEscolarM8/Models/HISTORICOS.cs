namespace APiConsumer.Models
{
    public class HISTORICOS
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal? price { get; set; }
        public int? categoryid { get; set; }
        public int? stock { get; set; }
        public int? userid { get; set; }
    }
}