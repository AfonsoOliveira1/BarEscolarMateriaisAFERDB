using APiConsumer.Models;

namespace APiConsumer.Models
{
    public class MATERIALCATEGORIES
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        // Optional: Reference to materials in this category
        public List<MATERIALS>? Materials { get; set; }
    }
}
