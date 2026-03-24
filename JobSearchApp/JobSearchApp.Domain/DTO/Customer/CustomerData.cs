using System.ComponentModel.DataAnnotations;

namespace JobSearchApp.Domain.DTO.Customer
{
    public class CustomerData
    {
        public Guid? CustomerId { get; set; }

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;
    }
}
