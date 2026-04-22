namespace GLMSMVC.Models.ViewModels
{
    public class ContractDetailsViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ServiceLevel { get; set; }
        public string ContractStatus { get; set; } = string.Empty;
        public bool HasFile { get; set; }
    }
}
