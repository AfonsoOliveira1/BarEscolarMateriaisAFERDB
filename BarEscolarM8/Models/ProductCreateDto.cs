namespace BarEscolarM8.Models
{
    public class ProductCreateDto
    {
        public decimal? Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public int? Kcal { get; set; }
        public int? Protein { get; set; }
        public int? Fat { get; set; }
        public int? Carbs { get; set; }
        public int? Salt { get; set; }
        public string Allergens { get; set; }
        public int? Stock { get; set; }
        public bool? IsActive { get; set; }
    }
}
