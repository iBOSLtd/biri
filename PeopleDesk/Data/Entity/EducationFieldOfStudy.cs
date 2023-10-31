using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EducationFieldOfStudy
    {
        public long IntEducationFieldOfStudyId { get; set; }
        public string StrEducationFieldOfStudy { get; set; }
        public bool? IsActive { get; set; }
    }
}
