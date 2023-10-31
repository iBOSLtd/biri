using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterWorkplaceGroup
    {
        public long IntWorkplaceGroupId { get; set; }
        public string StrWorkplaceGroup { get; set; }
        public string StrWorkplaceGroupCode { get; set; }
        public bool? IsActive { get; set; }
        public long IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
    }
}
