using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeTraining
    {
        public long IntTrainingId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public string StrTrainingTitle { get; set; }
        public long? IntInstituteId { get; set; }
        public string StrInstituteName { get; set; }
        public bool IsForeign { get; set; }
        public long IntCountryId { get; set; }
        public string StrCountry { get; set; }
        public DateTime? DteStartDate { get; set; }
        public DateTime? DteEndDate { get; set; }
        public DateTime? DteExpiryDate { get; set; }
        public long? IntTrainingFileUrlId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
