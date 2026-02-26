using System.ComponentModel.DataAnnotations;

namespace BarEscolarM8.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "O username é obrigatório")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Passwordhash { get; set; }

        [Required]
        public string Role { get; set; } = "STUDENT";
    }
}
