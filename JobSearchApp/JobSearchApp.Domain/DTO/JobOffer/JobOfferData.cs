namespace JobSearchApp.Domain.DTO.JobOffer
{
    public class JobOfferData
    {
        public Guid JobOfferId { get; set; }

        public Guid JobId { get; set; }

        public Guid ContractorId { get; set; }

        public string? ContractorName { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime? CreatedAt { get; set; }

    }
}
