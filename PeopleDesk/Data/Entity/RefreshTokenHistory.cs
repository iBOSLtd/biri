using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class RefreshTokenHistory
    {
        public long IntAutoId { get; set; }
        public string StrLogInId { get; set; }
        public string StrRefreshToken { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
    }
}
