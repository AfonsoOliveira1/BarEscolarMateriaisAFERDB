namespace BarEscolarM8.Models
{
    public class SaldoRequestDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoToUpdate { get; set; }
    }
}
