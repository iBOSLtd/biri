using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class TrainingAssesmentQuestionOption
    {
        public long IntOptionId { get; set; }
        public string StrOption { get; set; }
        public long IntQuestionId { get; set; }
        public long NumPoints { get; set; }
        public DateTime? DteLastAction { get; set; }
        public long? IntActionBy { get; set; }
        public long? IntOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
