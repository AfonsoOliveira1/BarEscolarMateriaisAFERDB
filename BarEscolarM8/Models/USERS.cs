namespace APiConsumer.Models
{
    public class USERS
    {
        public string id { get; set; }
        public string username { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string passwordhash { get; set; }
        // mantive string role para compatibilidade com seu modelo atual
        public int? role { get; set; }
    }
}