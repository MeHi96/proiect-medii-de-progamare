using MessagePack;
using Microsoft.AspNetCore.Identity;

namespace travelling.agency.Models
{
    public enum ACCEPTED
    {
        PROCESSING, ACCEPTED, REJECTED
    }
    public class Bookings
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int BookingsId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }
        public virtual Vacation Vacation { get; set; }
        public int VacationId { get; set; }

        public ACCEPTED Accepted { get; set; }

    }
}
