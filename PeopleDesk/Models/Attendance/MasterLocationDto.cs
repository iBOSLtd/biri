namespace PeopleDesk.Models.Attendance
{
    public class MasterLocationDto
    {
    }

    public partial class MasterLocationRegisterDTO
    {
        public long IntMasterLocationId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessId { get; set; }
        public string StrLongitude { get; set; }
        public string StrLatitude { get; set; }
        public string StrLocationCode { get; set; }
        public string StrPlaceName { get; set; }
        public string StrAddress { get; set; }
        public bool? IsActive { get; set; }
        public int ActionBy { get; set; }
    }

    public partial class GetMasterLocationRegisterDTO
    {
        public long IntMasterLocationId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessId { get; set; }
        public string StrLongitude { get; set; }
        public string StrLatitude { get; set; }
        public string StrLocationCode { get; set; }
        public string StrPlaceName { get; set; }
        public string StrAddress { get; set; }
        public bool? IsActive { get; set; }
        public long? IntPipelineHeaderId { get; set; }
        public long? IntCurrentStage { get; set; }
        public long? IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public bool? IsPipelineClosed { get; set; }
        public bool? IsReject { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }

    public class MasterLoctionRegistrationDdl
    {
        public long Value { get; set; }
        public string Label { get; set; }
        public long IntMasterLocationId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessId { get; set; }
        public string StrLongitude { get; set; }
        public string StrLatitude { get; set; }
        public string StrPlaceName { get; set; }
        public string StrAddress { get; set; }
        public string StrLocationCOde { get; set; }
        public string LocationLog { get; set; }
        public int Count { get; set; }
        public string StrStatus { get; set; }
    }

    public class CreateNUpdateMasterLocationEployeeWise
    {
        public long IntEmployeeId { get; set; }
        public long IntAccountId { get; set; }
        public string strEmployeeName { get; set; }
        public long IntActionBy { get; set; }
        public List<MasterLocationEmployeeWise> ListLocations { get; set; }
    }
    public class MasterLocationEmployeeWise
    {
        public long MasterLocationId { get; set; }
        public bool IsCreate { get; set; }

    }

    public class CreateNUpdateMasterLocationWise
    {
        public long MasterLocationId { get; set; }
        public long IntActionBy { get; set; }
        public long IntAccountId { get; set; }

        public List<MasterLocationEmployeeLocationWise> ListEmployee { get; set; }

    }

    public class MasterLocationEmployeeLocationWise
    {
        public long IntEmployeeId { get; set; }
        public string strEmployeeName { get; set; }
        public bool IsCreate { get; set; }

    }

    public class LocationWiseEmployeeList
    {
        public long? intMasterLocationId  {get;set;}
        public long? IntEmployeeBasicInfoId  {get;set;}
        public string? StrEmployeeName {get;set;}
        public string? StrEmployeeCode  {get;set;}
        public long? IntDesignationId  {get;set;}
        public string? StrDesignation  {get;set;}
        public long? IntDepartmentId {get;set;}
        public string? StrDepartment {get;set;}
        public bool? IsChecked  {get;set;}
        public string? StrStatus { get; set; }
    }
}
