namespace BarEscolarM8.Models
{
    public class MaterialViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Categoryid { get; set; }

        public int? Stock { get; set; }
    }
}
