using System.ComponentModel.DataAnnotations;

namespace BarEscolarM8.Models
{
    public class CategoryDto
    {
        [Url(ErrorMessage = "Insere um URL válido.")]
        public string? ImageUrl { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
