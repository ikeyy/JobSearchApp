namespace JobSearchApp.Domain.Entities
{
    public class JobOffer
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public Guid ContractorId { get; set; }

        public Contractor Contractor { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
