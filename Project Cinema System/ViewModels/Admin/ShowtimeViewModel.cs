
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Project_Cinema_System.ViewModels.Admin
{
    public class ShowtimeViewModel
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public int CinemaId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [Range(1, 10000)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 10000)]
        public int TotalTickets { get; set; }

        [Required]
        public string Status { get; set; } = "Scheduled";

        public IEnumerable<SelectListItem> Movies { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Cinemas { get; set; }
            = new List<SelectListItem>();
    }
}
