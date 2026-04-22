namespace GLMSMVC.Models.ViewModels
{
    public class ServiceRequestDetailsViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int ContractId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ServiceLevel { get; set; }
        public string ServiceRequestStatus { get; set; } = string.Empty;
        public bool CanDownloadFile { get; set; }
    }
}
