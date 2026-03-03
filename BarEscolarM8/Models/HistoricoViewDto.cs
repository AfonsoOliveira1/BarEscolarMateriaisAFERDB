namespace BarEscolarM8.Models
{
    public class HistoricoViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int Categoryid { get; set; }
        public int StockQuantidade { get; set; }

        public string Userid { get; set; }
        public string UserName { get; set; }

        public int Materialid { get; set; }
        public string MaterialName { get; set; }
    }
}
