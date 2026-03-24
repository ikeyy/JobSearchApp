namespace JobSearchApp.Domain.DTO.Job
{
    public class JobSearchParams
    {
        public string? Description { get; set; }
        public string? Status { get; set; }
        public decimal? MinBudget { get; set; } = 0;
        public decimal? MaxBudget { get; set; } = 0;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
