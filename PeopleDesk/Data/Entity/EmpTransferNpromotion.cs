using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpTransferNpromotion
    {
        public long IntTransferNpromotionId { get; set; }
        public long IntEmployeeId { get; set; }
        public string StrEmployeeName { get; set; }
        public string StrTransferNpromotionType { get; set; }
        public long IntAccountId { get; set; }
        public long? IntBusinessUnitIdFrom { get; set; }
        public long? IntWorkplaceGroupIdFrom { get; set; }
        public long? IntWorkplaceIdFrom { get; set; }
        public long? IntWingIdFrom { get; set; }
        public long? IntSoldDepoIdFrom { get; set; }
        public long? IntRegionIdFrom { get; set; }
        public long? IntAreaIdFrom { get; set; }
        public long? IntTerritoryIdFrom { get; set; }
        public long? IntDepartmentIdFrom { get; set; }
        public long? IntDesignationIdFrom { get; set; }
        public long? IntSupervisorIdFrom { get; set; }
        public long? IntLineManagerIdFrom { get; set; }
        public long? IntDottedSupervisorIdFrom { get; set; }
        public long IntBusinessUnitId { get; set; }
        public long IntWorkplaceGroupId { get; set; }
        public long IntWorkplaceId { get; set; }
        public long? IntWingId { get; set; }
        public long? IntSoldDepoId { get; set; }
        public long? IntRegionId { get; set; }
        public long? IntAreaId { get; set; }
        public long? IntTerritoryId { get; set; }
        public long IntDepartmentId { get; set; }
        public long IntDesignationId { get; set; }
        public long IntSupervisorId { get; set; }
        public long IntLineManagerId { get; set; }
        public long? IntDottedSupervisorId { get; set; }
        public DateTime DteEffectiveDate { get; set; }
        public DateTime? DteReleaseDate { get; set; }
        public long? IntAttachementId { get; set; }
        public string StrRemarks { get; set; }
        public long IntPipelineHeaderId { get; set; }
        public long IntCurrentStage { get; set; }
        public long IntNextStage { get; set; }
        public string StrStatus { get; set; }
        public bool? IsJoined { get; set; }
        public bool IsPipelineClosed { get; set; }
        public bool IsReject { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
        public long? IntRejectedBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        public bool IsProcess { get; set; }
        public long? IntSubstitutionEmployeeId { get; set; }
    }
}
