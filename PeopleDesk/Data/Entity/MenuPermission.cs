using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MenuPermission
    {
        public long IntMenuPermissionId { get; set; }
        public long? IntModuleId { get; set; }
        public string StrModuleName { get; set; }
        public long IntMenuId { get; set; }
        public string StrMenuName { get; set; }
        public string StrIsFor { get; set; }
        public long IntEmployeeOrRoleId { get; set; }
        public string StrEmployeeName { get; set; }
        public long IntAccountId { get; set; }
        public bool? IsView { get; set; }
        public bool? IsCreate { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsDelete { get; set; }
        public bool IsActive { get; set; }
        public bool IsForWeb { get; set; }
        public bool IsForApps { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}
