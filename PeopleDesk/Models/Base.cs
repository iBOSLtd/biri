namespace PeopleDesk.Models
{
    public class Base
    {
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long IntUpdatedBy { get; set; }
    }
    public class PaginationBaseVM
    {
        public long CurrentPage { get; set; } = 0;
        public long TotalCount { get; set; } = 0;
        public long PageSize { get; set; } = 0;
    }
}
