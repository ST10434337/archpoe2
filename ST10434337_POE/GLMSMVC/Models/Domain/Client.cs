using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GLMSMVC.Models.Domain
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Contract Details (Phone or Email)")]
        [StringLength(100)]
        public string? ContactDetails { get; set; } // Like cell or email

        [Required]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;// Select from dropdown, Save country Code. 


        public ICollection<Contract> Contracts { get; set; } = new List<Contract>(); // Has Many
    }
}
