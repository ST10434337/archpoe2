namespace GLMSMVC.Models.ViewModels
{
    public class ServiceRequestIndexViewModel
    {
        public List<ServiceRequestIndexItemViewModel> Items { get; set; } = new();
    }

    public class ServiceRequestIndexItemViewModel
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ContractStatus { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CostZar { get; set; }
        public string ServiceRequestStatus { get; set; } = string.Empty;
    }
}
