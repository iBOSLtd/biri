using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class training
    {
        public long IntTrainingId { get; set; }
        public string StrTrainingName { get; set; }
        public string StrTrainingCode { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public bool IsActive { get; set; }
        public long IntActionBy { get; set; }
        public DateTime DteActionDate { get; set; }
    }
}
