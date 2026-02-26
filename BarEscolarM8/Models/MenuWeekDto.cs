namespace BarEscolarM8.Models  
{
    public class MenuWeekDto
    {
        public int Id { get; set; }
        public string Weekstart { get; set; }

        public IEnumerable<MenuDayInWeekDto> MenuDays { get; set; }
    }
}
