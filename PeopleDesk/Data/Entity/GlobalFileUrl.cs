using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GlobalFileUrl
    {
        public long IntDocumentId { get; set; }
        public string StrTableReferrence { get; set; }
        public string StrFileServerId { get; set; }
        public string StrRefferenceDescription { get; set; }
        public long? IntDocumentTypeId { get; set; }
        public string StrDocumentName { get; set; }
        public decimal NumFileSize { get; set; }
        public string StrFileExtension { get; set; }
        public string StrServerLocation { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsProcess { get; set; }
    }
}
