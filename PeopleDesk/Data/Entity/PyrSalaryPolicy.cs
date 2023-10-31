using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrSalaryPolicy
    {
        public long IntPolicyId { get; set; }
        public long IntAccountId { get; set; }
        public string StrPolicyName { get; set; }
        public bool IsSalaryCalculationShouldBeActual { get; set; }
        public long IntGrossSalaryDevidedByDays { get; set; }
        public long IntGrossSalaryRoundDigits { get; set; }
        public bool IsGrossSalaryRoundUp { get; set; }
        public bool IsGrossSalaryRoundDown { get; set; }
        public long IntNetPayableSalaryRoundDigits { get; set; }
        public bool IsNetPayableSalaryRoundUp { get; set; }
        public bool IsNetPayableSalaryRoundDown { get; set; }
        public bool IsSalaryShouldBeFullMonth { get; set; }
        public long IntPreviousMonthStartDay { get; set; }
        public long IntNextMonthEndDay { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
