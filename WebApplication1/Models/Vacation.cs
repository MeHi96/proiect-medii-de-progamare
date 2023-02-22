using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace travelling.agency.Models
{
    public class Vacation
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int VacationId { get; set; }

        [Display(Name = "Start Date")]
        public DateTime startDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime endDate { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100.000.")]
        public float price { get; set; }
        public virtual Destination Destination { get; set; }
        public int DestinationId { get; set; }

        [Display(Name = "Created by")]
        public IdentityUser IdentityUser { get; set; }
        public virtual List<Bookings> bookings { get; set; }

        public Vacation()
        {
            bookings = new List<Bookings>();
        }

    }
}
