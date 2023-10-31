namespace PeopleDesk.Data
{
    public class BaseVM
    {
        public long accountId { get; set; } = -1;
        public long businessUnitId { get; set; } = -1;
        public long workplaceGroupId { get; set; } = -1;
        public string loginId { get; set; } = "";
        public long employeeId { get; set; } = -1;
        public bool isOfficeAdmin { get; set; } = false;
        public bool isOwner { get; set; } = false;
        public int isSupNLMORManagement { get; set; } = 0;
        public List<long?>? businessUnitList { get; set; }
        public List<long?>? workplaceGroupList { get; set; }
        public List<long?>? workplaceList { get; set; }
        public List<long?>? wingList { get; set; }
        public List<long?>? soleDepoList { get; set; }
        public List<long?>? regionList { get; set; }
        public List<long?>? areaList { get; set; }
        public List<long?>? territoryList { get; set; }

        public bool isAuthorize { get; set; } = false;
    }

    public class PayloadIsAuthorizeVM
    {
        public long accountId { get; set; } = -1;
        public bool isOfficeAdmin { get; set; } = false;
        public bool isOwner { get; set; } = false;
        public int isSupNLMORManagement { get; set; } = -1;
        public long businessUnitId { get; set; } = -1;
        public long workplaceGroupId { get; set; } = -1;
        public long workplaceId { get; set; } = -1;
        public long wingId { get; set; } = -1;
        public long soleDepoId { get; set; } = -1;
        public long regionId { get; set; } = -1;
        public long areaId { get; set; } = -1;
        public long territoryId { get; set; } = -1;
    }
}
