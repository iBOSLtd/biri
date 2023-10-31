using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Gender
    {
        public long IntGenderId { get; set; }
        public string StrGender { get; set; }
        public string StrGenderCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
