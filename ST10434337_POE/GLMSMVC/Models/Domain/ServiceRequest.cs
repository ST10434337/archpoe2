using GLMSMVC.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMSMVC.Models.Domain
{
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        public Contract? Contract { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalAmount { get; set; }

        [StringLength(10)]
        public string SourceCurrency { get; set; } = "USD";

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZar { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;

        public bool UsedAPI { get; set; } = false; // Flag catch if API down see: ExchangeRateProvider () Repo update! 
    }
}
