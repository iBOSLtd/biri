using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class CafCafeteriaDetail
    {
        public int IntRow { get; set; }
        public int? IntEnroll { get; set; }
        public DateTime? DteMeal { get; set; }
        public int? IntMealFor { get; set; }
        public int? IntCountMeal { get; set; }
        public int? IntSpendMeal { get; set; }
        public bool? IsOwnGuest { get; set; }
        public bool? IsPayable { get; set; }
        public string StrNarration { get; set; }
        public int? IntActionBy { get; set; }
        public DateTime? DteAction { get; set; }
        public bool? YsnMenualEntry { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}
