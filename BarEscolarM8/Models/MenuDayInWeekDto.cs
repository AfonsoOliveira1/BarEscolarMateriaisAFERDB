namespace BarEscolarM8.Models
{
    public class MenuDayInWeekDto
    {
        public int Id { get; set; }
        public string Option { get; set; }
        public DateOnly Date { get; set; }
        public string MainDish { get; set; }
        public string Soup { get; set; }
        public string Dessert { get; set; }
        public string Notes { get; set; }
    }
}
