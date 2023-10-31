using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class ExternalTraining
    {
        public long IntExternalTrainingId { get; set; }
        public string StrTrainingName { get; set; }
        public string StrResourcePersonName { get; set; }
        public DateTime DteDate { get; set; }
        public long IntDepartmentId { get; set; }
        public string StrDepartmentName { get; set; }
        public string StrOrganizationCategory { get; set; }
        public long IntBatchSize { get; set; }
        public long? IntPresentParticipant { get; set; }
        public string StrRemarks { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public long? IntActionBy { get; set; }
        public DateTime? DteActionDate { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedDate { get; set; }
    }
}
