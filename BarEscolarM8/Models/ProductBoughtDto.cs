namespace BarEscolarM8.Models
{
    public class ProductBoughtDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateOnly? Date { get; set; }
        public decimal? Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Stock { get; set; }
        public int? Qtd { get; set; }
        public bool? IsActive { get; set; }
        public string CategoryName { get; set; }

    }
}
