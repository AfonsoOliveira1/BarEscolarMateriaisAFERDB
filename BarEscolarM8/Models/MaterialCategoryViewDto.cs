namespace BarEscolarM8.Models
{
    public class MaterialCategoryViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<MaterialViewModel>? Materials { get; set; }
    }
}
