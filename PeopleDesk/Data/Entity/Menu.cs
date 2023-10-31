using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Menu
    {
        public long IntMenuId { get; set; }
        public string StrHashCode { get; set; }
        public long IntMenuLabelId { get; set; }
        public long IntParentMenuId { get; set; }
        public string StrMenuName { get; set; }
        public string StrMenuNameWeb { get; set; }
        public string StrMenuNameApps { get; set; }
        public string StrTo { get; set; }
        public string StrToForApps { get; set; }
        public string StrIcon { get; set; }
        public string StrIconForApps { get; set; }
        public long IntMenuSerial { get; set; }
        public long? IntMenuSerialForApps { get; set; }
        public bool IsExpandable { get; set; }
        public bool IsForCommon { get; set; }
        public bool IsMenuForView { get; set; }
        public bool IsActive { get; set; }
        public bool IsForApps { get; set; }
        public bool IsForWeb { get; set; }
        public bool IsHasApproval { get; set; }
    }
}
