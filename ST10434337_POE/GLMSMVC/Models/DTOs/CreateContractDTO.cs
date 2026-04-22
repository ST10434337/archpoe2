using GLMSMVC.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace GLMSMVC.Models.DTOs
{
    public class CreateContractDTO
    {
        [Required]
        public int ClientId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public ServiceLevel? ServiceLevel { get; set; }
    }
}
