using System.ComponentModel.DataAnnotations;

namespace BarEscolarM8.Models
{
    public class LoginModel
    {
        [Required]
        public string EmailorUsername { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Passwordhash { get; set; } = "";
    }
}
