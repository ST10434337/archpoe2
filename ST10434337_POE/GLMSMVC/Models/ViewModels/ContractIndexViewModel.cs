using GLMSMVC.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMSMVC.Models.ViewModels
{
    public class ContractIndexViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ContractStatus? Status { get; set; }

        public List<SelectListItem> StatusOptions { get; set; } = new();
        public List<ContractIndexItemViewModel> Items { get; set; } = new();
    }

    public class ContractIndexItemViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ServiceLevel { get; set; }
        public string ContractStatus { get; set; } = string.Empty;
        public bool CanEdit { get; set; }
    }
}
