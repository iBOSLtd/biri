using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeRelativesContact
    {
        public long IntEmployeeRelativesContactId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public string StrRelativesName { get; set; }
        public long IntRelationShipId { get; set; }
        public string StrRelationship { get; set; }
        public string StrPhone { get; set; }
        public string StrEmail { get; set; }
        public string StrAddress { get; set; }
        public bool IsEmergencyContact { get; set; }
        public long? IntGrantorNomineeTypeId { get; set; }
        public string StrGrantorNomineeType { get; set; }
        public string StrNid { get; set; }
        public string StrBirthId { get; set; }
        public DateTime? DteDateOfBirth { get; set; }
        public string StrRemarks { get; set; }
        public long? IntPictureFileUrlId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
