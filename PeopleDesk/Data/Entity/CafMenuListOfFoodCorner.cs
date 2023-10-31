using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class CafMenuListOfFoodCorner
    {
        public long IntAutoId { get; set; }
        public string StrDayName { get; set; }
        public string StrMenu { get; set; }
        public string StrStatus { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime DteUpdatedAt { get; set; }
        public long IntUpdatedBy { get; set; }
    }
}
