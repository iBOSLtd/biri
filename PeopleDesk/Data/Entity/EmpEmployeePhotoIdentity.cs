using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeePhotoIdentity
    {
        public long IntEmployeePhotoIdentityId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public long? IntProfilePicFileUrlId { get; set; }
        public long? IntProfilePicFormalFileUrlId { get; set; }
        public long? IntSignatureFileUrlId { get; set; }
        public string StrNid { get; set; }
        public long? IntNidfileUrlId { get; set; }
        public string StrBirthId { get; set; }
        public long? IntBirthIdfileUrlId { get; set; }
        public string StrPassport { get; set; }
        public long? IntPassportFileUrlId { get; set; }
        public string StrNationality { get; set; }
        public string StrBiography { get; set; }
        public string StrHobbies { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
