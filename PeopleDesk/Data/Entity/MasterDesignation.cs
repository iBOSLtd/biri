using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterDesignation
    {
        public long IntDesignationId { get; set; }
        public string StrDesignation { get; set; }
        public string StrDesignationCode { get; set; }
        public long? IntPositionId { get; set; }
        public long? IntRankingId { get; set; }
        public long? IntPayscaleGradeId { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
