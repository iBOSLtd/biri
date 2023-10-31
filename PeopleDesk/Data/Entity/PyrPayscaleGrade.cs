using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrPayscaleGrade
    {
        public long IntPayscaleGradeId { get; set; }
        public string StrPayscaleGradeName { get; set; }
        public string StrPayscaleGradeCode { get; set; }
        public long IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long IntShortOrder { get; set; }
        public decimal? NumMinSalary { get; set; }
        public decimal? NumMaxSalary { get; set; }
        public string StrDepentOn { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
