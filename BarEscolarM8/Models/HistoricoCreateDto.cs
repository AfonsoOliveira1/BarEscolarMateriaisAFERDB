namespace BarEscolarM8.Models
{
    public class HistoricoCreateDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int Categoryid { get; set; }
        public int StockQuantidade { get; set; }
        public string Userid { get; set; }
        public int Materialid { get; set; }
    }
}
