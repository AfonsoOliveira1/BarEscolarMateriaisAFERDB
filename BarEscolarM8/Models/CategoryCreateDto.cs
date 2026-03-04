using System.ComponentModel.DataAnnotations;

namespace BarEscolarM8.Models
{
    public class CategoryCreateDto
    {
        [Url(ErrorMessage = "Insere um URL válido.")]
        public string? ImageUrl { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
