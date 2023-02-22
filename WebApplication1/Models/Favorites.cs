using Microsoft.AspNetCore.Identity;
using travelling.agency.Models;

namespace WebApplication1.Models
{
    public class Favorites
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int FavoritesId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }
        public virtual Vacation Vacation { get; set; }
        public int VacationId { get; set; }

    }
}
