using System.ComponentModel.DataAnnotations;

namespace JobSearchApp.Domain.DTO.Job
{
    public class JobData
    {
        public Guid JobId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public decimal Budget { get; set; }

        public string Status { get; set; } = "Open";

        [Required]
        public string Description { get; set; }

        public Guid? AcceptedBy { get; set; }

        public Guid CustomerId { get; set; }
    }
}
