using GLMSMVC.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMSMVC.Models.Domain
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; } // Client Name Dropdown Select

        public Client? Client { get; set; } // Has one

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public ServiceLevel? ServiceLevel { get; set; } // Standard, Express, Gold DropDown Select

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        public bool IsPaused { get; set; } = false;

        [StringLength(255)]
        public string? FilePath { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; } // For Concurrency Conflict on Edit    

        public ServiceRequest? ServiceRequest { get; set; } // Has one


        //[NotMapped]
        //public IContractState? ContractState { get; set; }

    }
}
