using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TrainingAssesmentQuestion
    {
        public long IntQuestionId { get; set; }
        public string StrQuestion { get; set; }
        public long IntScheduleId { get; set; }
        public bool IsPreAssesment { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DteLastActionDate { get; set; }
        public long? IntActionBy { get; set; }
        public bool IsRequired { get; set; }
        public string StrInputType { get; set; }
        public long? IntOrder { get; set; }
    }
}
