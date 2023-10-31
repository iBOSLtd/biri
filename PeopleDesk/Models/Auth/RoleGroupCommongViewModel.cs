using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;

namespace PeopleDesk.Models.Auth
{
    public class RoleGroupCommongViewModel
    {
        public RoleGroupHeader Header { get; set; }
        public List<RoleGroupRow> Rows { get; set; }
    }

    public class UserGroupPaginationViewModel
    {
        public long CurrentPage { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public IEnumerable<UserGroupHeader> Data { get; set; }

    }
}
