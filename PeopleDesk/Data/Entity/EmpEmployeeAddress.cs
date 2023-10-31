using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeAddress
    {
        public long IntEmployeeAddressId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public long IntAddressTypeId { get; set; }
        public string StrAddressType { get; set; }
        public long? IntCountryId { get; set; }
        public string StrCountry { get; set; }
        public long? IntDivisionId { get; set; }
        public string StrDivision { get; set; }
        public long? IntDistrictOrStateId { get; set; }
        public string StrDistrictOrState { get; set; }
        public long? IntThanaId { get; set; }
        public string StrThana { get; set; }
        public string StrAddressDetails { get; set; }
        public long? IntPostOfficeId { get; set; }
        public string StrPostOffice { get; set; }
        public string StrZipOrPostCode { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
