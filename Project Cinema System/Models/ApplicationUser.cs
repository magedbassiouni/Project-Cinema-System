
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Project_Cinema_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; }
            = new List<Booking>();
    }
}
