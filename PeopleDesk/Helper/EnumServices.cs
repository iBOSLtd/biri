using System.ComponentModel;

namespace PeopleDesk.Helper
{
    public enum ApprovalEnum
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum UserCategoriesTypeEnum
    {
        [Description("All")]
        All = 1,

        [Description("BusinessUnit Wise")]
        BusinessUnitWise = 2,

        [Description("Department Wise")]
        DepartmentWise = 3,

        [Description("Designation Wise")]
        DesignationWise = 4,

        [Description("UserGroup Wise")]
        UserGroupWise = 5,

        [Description("Individual")]
        Individual = 6

    }
    public enum PolicyAreaTypeEnum
    {
        BusinessUnit = 1,
        Department = 2
    }

    public enum AssetRequisitionStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Denied = 3,
    }
}
