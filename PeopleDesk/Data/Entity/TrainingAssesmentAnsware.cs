using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TrainingAssesmentAnsware
    {
        public long IntAnswerId { get; set; }
        public long IntQuestionId { get; set; }
        public long IntOptionId { get; set; }
        public string StrOption { get; set; }
        public long NumMarks { get; set; }
        public DateTime? DteLastAction { get; set; }
        public bool? IsActive { get; set; }
        public long? IntActionBy { get; set; }
        public long? IntRequisitionId { get; set; }
    }
}
