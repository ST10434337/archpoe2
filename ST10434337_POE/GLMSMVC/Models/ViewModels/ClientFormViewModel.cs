using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GLMSMVC.Models.ViewModels
{
    public class ClientFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Contact Details")]
        [StringLength(100)]
        public string? ContactDetails { get; set; }

        [Required]
        [Display(Name = "Region")]
        public string Region { get; set; } = string.Empty;

        public List<SelectListItem> RegionOptions { get; set; } = new();
    }
}
