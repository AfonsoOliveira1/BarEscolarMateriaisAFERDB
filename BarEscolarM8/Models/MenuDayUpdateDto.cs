namespace BarEscolarM8.Models
{
    public class MenuDayUpdateDto
    {
        public int Id { get; set; }
        public int? Menuweekid { get; set; }
        public DateOnly? Date { get; set; }
        public string Option { get; set; }
        public string MainDish { get; set; }
        public string Soup { get; set; }
        public string Dessert { get; set; }
        public string Notes { get; set; }
        public int? MaxSeats { get; set; }
    }
}
