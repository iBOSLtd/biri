using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeEducation
    {
        public long IntEmployeeEducationId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public long IntInstituteId { get; set; }
        public string StrInstituteName { get; set; }
        public bool IsForeign { get; set; }
        public long? IntCountryId { get; set; }
        public string StrCountry { get; set; }
        public long? IntEducationDegreeId { get; set; }
        public string StrEducationDegree { get; set; }
        public long? IntEducationFieldOfStudyId { get; set; }
        public string StrEducationFieldOfStudy { get; set; }
        public string StrCgpa { get; set; }
        public string StrOutOf { get; set; }
        public DateTime? DteStartDate { get; set; }
        public DateTime? DteEndDate { get; set; }
        public long? IntCertificateFileUrlId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
