using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EducationDegree
    {
        public long IntEmployeeEducationId { get; set; }
        public string StrEducationDegree { get; set; }
        public bool? IsActive { get; set; }
    }
}
