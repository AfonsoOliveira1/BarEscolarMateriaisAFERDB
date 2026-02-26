namespace BarEscolarM8.Models
{
    public class WeekXUsers
    {
        public IEnumerable<MenuWeekDto> MenuWeek { get; set; }
        public IEnumerable<UserReadDto> Users { get; set; }
    }
}
