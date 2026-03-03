namespace BarEscolarM8.Models
{
    public class WeekXUsers
    {
        public IEnumerable<MenuWeekDto> MenuWeek { get; set; }
        public IEnumerable<UserReadDto> Users { get; set; }
        public Counts Counts { get; set; }
    }
    public class Counts
    {
        public int weeksCount { get; set; }
        public int usersCount { get; set; }
        public int daysCount { get; set; }
        public int matCount { get; set; }
        public int matcatCount { get; set; }
        public int prodCount { get; set; }
        public int catCount { get; set; }
    }
}
