namespace JobSearchApp.Domain.Entities
{
    public class Job
    {
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DueDate { get; set; }

        public decimal Budget { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public Guid? AcceptedBy { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
