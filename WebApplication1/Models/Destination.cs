using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace travelling.agency.Models
{
    public class Destination
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int DestinationId { get; set; }

        [Display(Name = "Country")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Country must be between 1 and 50 character in length.")]
        public String country { get; set; }

        [Display(Name = "City")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "City must be between 1 and 50 character in length.")]
        public String city { get; set; }

        [Display(Name = "Name of Hotel")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name of hotel must be between 1 and 50 character in length.")]
        public String hotel { get; set; }

        public virtual List<Vacation> vacations { get; set; }

        public Destination()
        {
            vacations = new List<Vacation>();
        }

    }
}
