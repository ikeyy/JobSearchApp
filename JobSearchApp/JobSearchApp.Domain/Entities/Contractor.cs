namespace JobSearchApp.Domain.Entities
{
    public class Contractor
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
