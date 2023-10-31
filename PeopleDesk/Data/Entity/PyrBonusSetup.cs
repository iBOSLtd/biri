using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrBonusSetup
    {
        public long IntBonusSetupId { get; set; }
        public long? IntBonusId { get; set; }
        public string StrBonusName { get; set; }
        public string StrBonusDescription { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long? IntReligion { get; set; }
        public string StrReligionName { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string StrEmploymentType { get; set; }
        public long? IntMinimumServiceLengthMonth { get; set; }
        public long? IntMaximumServiceLengthMonth { get; set; }
        public string StrBonusPercentageOn { get; set; }
        public decimal? NumBonusPercentage { get; set; }
        public DateTime? DteServerDateTime { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
