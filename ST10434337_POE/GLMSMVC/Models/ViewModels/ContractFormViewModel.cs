using GLMSMVC.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GLMSMVC.Models.ViewModels
{
    public class ContractFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Client Name")]
        public int ClientId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Service Level")]
        public ServiceLevel? ServiceLevel { get; set; }

        [Display(Name = "Pause Contract")]
        public bool PauseContract { get; set; }

        [Display(Name = "Agreement PDF")]
        public IFormFile? File { get; set; }

        public string? ExistingFilePath { get; set; }

        public List<SelectListItem> ClientOptions { get; set; } = new();
        public List<SelectListItem> ServiceLevelOptions { get; set; } = new();
    }
}
