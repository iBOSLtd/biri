using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class CafCafeterium
    {
        public int IntRow { get; set; }
        public int? IntEnroll { get; set; }
        public int? IntGroup { get; set; }
        public int? IntType { get; set; }
        public int? IntMealOption { get; set; }
        public string StrNarration { get; set; }
        public int? IntActionBy { get; set; }
        public DateTime? DteAction { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
