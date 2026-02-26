using System.Text.Json.Serialization;

namespace BarEscolarM8.Models
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public UserViewModelJSON User { get; set; }
        public string Message { get; set; }
        public string SessionId { get; set; }// novo id de sessao
        public string Status { get; set; }
    }

    public class UserViewModelJSON
    {
        [JsonPropertyName("nameid")]
        public string Id { get; set; } = "";

        [JsonPropertyName("unique_name")]
        public string UserName { get; set; } = "";

        [JsonPropertyName("FullName")]
        public string FullName { get; set; } = "";

        [JsonPropertyName("email")]
        public string Email { get; set; } = "";

        [JsonPropertyName("role")]
        public string Role { get; set; } = "STUDENT";
    }
}
