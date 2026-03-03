namespace BarEscolarM8.Models
{
    public class ProdxCat
    {
        public IEnumerable<ProductDto> Products { get; set; }
        public IEnumerable<CategoryDto> Categorys { get; set; }
    }
}
