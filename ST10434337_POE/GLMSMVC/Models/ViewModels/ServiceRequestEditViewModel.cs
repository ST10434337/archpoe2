using GLMSMVC.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GLMSMVC.Models.ViewModels
{
    public class ServiceRequestEditViewModel
    {
        public int Id { get; set; }
        public int ContractId { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string ContractStatus { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Original Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount cannot be negative.")]
        public decimal OriginalAmount { get; set; }

        [Required]
        [Display(Name = "Currency / Country")]
        public string CurrencyCode { get; set; } = "USD";

        [Display(Name = "Cost (ZAR)")]
        public decimal CostZar { get; set; }

        [Display(Name = "Service Request Status")]
        public ServiceRequestStatus Status { get; set; }

        [Display(Name = "Use live API conversion")]
        public bool UseApi { get; set; } = true;

        public bool IsReadOnly { get; set; }
        public string? ApiError { get; set; }

        public List<SelectListItem> CurrencyOptions { get; set; } = new();
        public List<SelectListItem> StatusOptions { get; set; } = new();
    }
}
