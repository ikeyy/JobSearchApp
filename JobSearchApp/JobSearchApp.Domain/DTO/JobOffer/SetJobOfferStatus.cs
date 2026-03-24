namespace JobSearchApp.Domain.DTO.JobOffer
{
    public class SetJobOfferStatus
    {
        public Guid JobOfferId { get; set; }

        public Guid CustomerId { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
