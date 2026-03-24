using System.ComponentModel.DataAnnotations;

namespace JobSearchApp.Domain.DTO.Contractor
{
    public class ContractorData
    {
        public Guid ContractorId { get; set; }

        [Required]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        public int Rating { get; set; }
    }
}
