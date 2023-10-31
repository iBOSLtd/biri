using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpEmployeeFile
    {
        public long IntEmployeeFileId { get; set; }
        public long IntEmployeeBasicInfoId { get; set; }
        public long IntDocumentTypeId { get; set; }
        public string StrFileTitle { get; set; }
        public long IntEmployeeFileUrlId { get; set; }
        public string StrTags { get; set; }
        public bool? IsActive { get; set; }
        public long IntWorkplaceId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
