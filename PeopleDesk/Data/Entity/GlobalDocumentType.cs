using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GlobalDocumentType
    {
        public long IntDocumentTypeId { get; set; }
        public string StrDocumentType { get; set; }
        public long IntOwnerType { get; set; }
        public bool? IsActive { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
