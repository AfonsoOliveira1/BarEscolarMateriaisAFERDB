using System.ComponentModel.DataAnnotations;

namespace APiConsumer.Models
{ 
    public class CreateWeekViewModel
    {
        [Required(ErrorMessage = "Please select a start date.")]
        [DataType(DataType.Date)]
        public DateTime WeekStart { get; set; }
    }

}
