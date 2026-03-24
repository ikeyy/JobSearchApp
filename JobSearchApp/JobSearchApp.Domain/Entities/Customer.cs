namespace JobSearchApp.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
