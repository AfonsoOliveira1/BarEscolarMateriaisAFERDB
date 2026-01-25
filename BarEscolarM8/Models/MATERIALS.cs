namespace APiConsumer.Models
{
    public class MATERIALS
    {
        public int id { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public decimal price { get; set; }
        public int categoryid { get; set; }
        public int? stock { get; set; }

        // opcional: navegação curta para a categoria
        //public MATERIALCATEGORIES category { get; set; }
    }
}