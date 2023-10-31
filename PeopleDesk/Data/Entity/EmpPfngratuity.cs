using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpPfngratuity
    {
        public long IntPfngratuityId { get; set; }
        public long IntAccountId { get; set; }
        public bool IsHasPfpolicy { get; set; }
        public long IntNumOfEligibleYearForBenifit { get; set; }
        public decimal NumEmployeeContributionOfBasic { get; set; }
        public decimal NumEmployerContributionOfBasic { get; set; }
        public long IntNumOfEligibleMonthForPfinvestment { get; set; }
        public bool IsHasGratuityPolicy { get; set; }
        public long IntNumOfEligibleYearForGratuity { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool IsEmployeeBased { get; set; }
    }
}
