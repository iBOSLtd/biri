using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class PyrBonusGenerateRow
    {
        public long IntRowId { get; set; }
        public long? IntBonusHeaderId { get; set; }
        public long? IntEmployeeId { get; set; }
        public string StrEmployeeCode { get; set; }
        public string StrEmployeeName { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string StrEmploymentTypeName { get; set; }
        public long? IntDepartmentId { get; set; }
        public string StrDepartmentName { get; set; }
        public long? IntDesignationId { get; set; }
        public string StrDesignationName { get; set; }
        public long? IntReligionId { get; set; }
        public string StrReligionName { get; set; }
        public long? IntWorkPlaceGroupId { get; set; }
        public string StrWorkPlaceGroupName { get; set; }
        public long? IntWorkPlaceId { get; set; }
        public string StrWorkPlaceName { get; set; }
        public long? IntPayrollGroupId { get; set; }
        public string StrPayrollGroupName { get; set; }
        public DateTime? DteJoiningDate { get; set; }
        public string StrServiceLength { get; set; }
        public decimal? NumSalary { get; set; }
        public decimal? NumBasic { get; set; }
        public decimal? NumBonusAmount { get; set; }
        public long? IntBankId { get; set; }
        public string StrBankName { get; set; }
        public long? IntBankBranchId { get; set; }
        public string StrBankBranchName { get; set; }
        public string StrRoutingNumber { get; set; }
        public string StrBankAccountNumber { get; set; }
        public long? IntBonusId { get; set; }
        public string StrBonusName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
