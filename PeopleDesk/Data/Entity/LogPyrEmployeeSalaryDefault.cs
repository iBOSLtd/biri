using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class LogPyrEmployeeSalaryDefault
    {
        public long IntLogEmployeeSalaryDefaultId { get; set; }
        public long IntEmployeeId { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public long? IntPayrollGroupId { get; set; }
        public long? IntPayscaleGradeId { get; set; }
        public long? IntPayscaleGradeAdditionId { get; set; }
        public decimal? NumBasic { get; set; }
        public decimal? NumHouseAllowance { get; set; }
        public decimal? NumMedicalAllowance { get; set; }
        public decimal? NumConveyanceAllowance { get; set; }
        public decimal? NumWashingAllowance { get; set; }
        public decimal? NumCbadeduction { get; set; }
        public decimal? NumSpecialAllowance { get; set; }
        public decimal? NumGrossSalary { get; set; }
        public decimal? NumTotalSalary { get; set; }
        public long? IntEffectiveMonth { get; set; }
        public long? IntEffectiveYear { get; set; }
        public decimal? ReqNumBasic { get; set; }
        public decimal? ReqNumHouseAllowance { get; set; }
        public decimal? ReqNumMedicalAllowance { get; set; }
        public decimal? ReqNumConveyanceAllowance { get; set; }
        public decimal? ReqNumWashingAllowance { get; set; }
        public decimal? ReqNumCbadeduction { get; set; }
        public decimal? ReqNumSpecialAllowance { get; set; }
        public decimal? ReqNumGrossSalary { get; set; }
        public decimal? ReqNumTotalSalary { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public long? IntLogCreatedByUserId { get; set; }
        public DateTime? DteLogInsertCreatedAt { get; set; }
    }
}
