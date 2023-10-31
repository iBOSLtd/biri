using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class UserType
    {
        public long IntUserTypeId { get; set; }
        public string StrUserType { get; set; }
        public bool? IsActive { get; set; }
    }
}
