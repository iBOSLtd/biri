using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PeopleDesk.Data.Entity;

namespace PeopleDesk.Data
{
    public partial class PeopleDeskContext : DbContext
    {
        public PeopleDeskContext()
        {
        }

        public PeopleDeskContext(DbContextOptions<PeopleDeskContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountBankDetail> AccountBankDetails { get; set; }
        public virtual DbSet<AccountPackage> AccountPackages { get; set; }
        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<AnnouncementRow> AnnouncementRows { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<AssetAssign> AssetAssigns { get; set; }
        public virtual DbSet<AssetDirectAssign> AssetDirectAssigns { get; set; }
        public virtual DbSet<AssetRequisition> AssetRequisitions { get; set; }
        public virtual DbSet<AssetTransaction> AssetTransactions { get; set; }
        public virtual DbSet<AssetTransfer> AssetTransfers { get; set; }
        public virtual DbSet<AssetType> AssetTypes { get; set; }
        public virtual DbSet<AssetsRegister> AssetsRegisters { get; set; }
        public virtual DbSet<AttendanceLateNotifyLog> AttendanceLateNotifyLogs { get; set; }
        public virtual DbSet<BackgroundJobFailureHistory> BackgroundJobFailureHistories { get; set; }
        public virtual DbSet<BackgroundJobSuccessHistory> BackgroundJobSuccessHistories { get; set; }
        public virtual DbSet<BankBranch> BankBranches { get; set; }
        public virtual DbSet<BankWallet> BankWallets { get; set; }
        public virtual DbSet<BloodGroup> BloodGroups { get; set; }
        public virtual DbSet<CafCafeteriaDetail> CafCafeteriaDetails { get; set; }
        public virtual DbSet<CafCafeterium> CafCafeteria { get; set; }
        public virtual DbSet<CafMenuListOfFoodCorner> CafMenuListOfFoodCorners { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<EducationDegree> EducationDegrees { get; set; }
        public virtual DbSet<EducationFieldOfStudy> EducationFieldOfStudies { get; set; }
        public virtual DbSet<EmpEmployeeAddress> EmpEmployeeAddresses { get; set; }
        public virtual DbSet<EmpEmployeeBankDetail> EmpEmployeeBankDetails { get; set; }
        public virtual DbSet<EmpEmployeeBasicInfo> EmpEmployeeBasicInfos { get; set; }
        public virtual DbSet<EmpEmployeeBasicInfoDetail> EmpEmployeeBasicInfoDetails { get; set; }
        public virtual DbSet<EmpEmployeeDocumentManagement> EmpEmployeeDocumentManagements { get; set; }
        public virtual DbSet<EmpEmployeeEducation> EmpEmployeeEducations { get; set; }
        public virtual DbSet<EmpEmployeeFile> EmpEmployeeFiles { get; set; }
        public virtual DbSet<EmpEmployeeIncrement> EmpEmployeeIncrements { get; set; }
        public virtual DbSet<EmpEmployeeJobHistory> EmpEmployeeJobHistories { get; set; }
        public virtual DbSet<EmpEmployeePhotoIdentity> EmpEmployeePhotoIdentities { get; set; }
        public virtual DbSet<EmpEmployeeRelativesContact> EmpEmployeeRelativesContacts { get; set; }
        public virtual DbSet<EmpEmployeeSeparation> EmpEmployeeSeparations { get; set; }
        public virtual DbSet<EmpEmployeeTraining> EmpEmployeeTrainings { get; set; }
        public virtual DbSet<EmpEmploymentTypeWiseLeaveBalance> EmpEmploymentTypeWiseLeaveBalances { get; set; }
        public virtual DbSet<EmpExpenseApplication> EmpExpenseApplications { get; set; }
        public virtual DbSet<EmpExpenseDocument> EmpExpenseDocuments { get; set; }
        public virtual DbSet<EmpExpenseType> EmpExpenseTypes { get; set; }
        public virtual DbSet<EmpJobExperience> EmpJobExperiences { get; set; }
        public virtual DbSet<EmpLoanApplication> EmpLoanApplications { get; set; }
        public virtual DbSet<EmpLoanSchedule> EmpLoanSchedules { get; set; }
        public virtual DbSet<EmpLoanType> EmpLoanTypes { get; set; }
        public virtual DbSet<EmpManualAttendanceSummary> EmpManualAttendanceSummaries { get; set; }
        public virtual DbSet<EmpPfInvestmentHeader> EmpPfInvestmentHeaders { get; set; }
        public virtual DbSet<EmpPfInvestmentRow> EmpPfInvestmentRows { get; set; }
        public virtual DbSet<EmpPfngratuity> EmpPfngratuities { get; set; }
        public virtual DbSet<EmpPfwithdraw> EmpPfwithdraws { get; set; }
        public virtual DbSet<EmpSalaryCertificateRequest> EmpSalaryCertificateRequests { get; set; }
        public virtual DbSet<EmpSeparationType> EmpSeparationTypes { get; set; }
        public virtual DbSet<EmpSocialMedium> EmpSocialMedia { get; set; }
        public virtual DbSet<EmpTax> EmpTaxes { get; set; }
        public virtual DbSet<EmpTransferNpromotion> EmpTransferNpromotions { get; set; }
        public virtual DbSet<EmpTransferNpromotionRoleExtension> EmpTransferNpromotionRoleExtensions { get; set; }
        public virtual DbSet<EmpTransferNpromotionUserRole> EmpTransferNpromotionUserRoles { get; set; }
        public virtual DbSet<EmpWorklineConfig> EmpWorklineConfigs { get; set; }
        public virtual DbSet<EmployeeBulkUpload> EmployeeBulkUploads { get; set; }
        public virtual DbSet<ExternalTraining> ExternalTrainings { get; set; }
        public virtual DbSet<FiscalYear> FiscalYears { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<GeneratedHashCode> GeneratedHashCodes { get; set; }
        public virtual DbSet<GlobalBankBranch> GlobalBankBranches { get; set; }
        public virtual DbSet<GlobalCulture> GlobalCultures { get; set; }
        public virtual DbSet<GlobalDocumentType> GlobalDocumentTypes { get; set; }
        public virtual DbSet<GlobalFileUrl> GlobalFileUrls { get; set; }
        public virtual DbSet<GlobalInstitute> GlobalInstitutes { get; set; }
        public virtual DbSet<GlobalOrganogramTree> GlobalOrganogramTrees { get; set; }
        public virtual DbSet<GlobalPipelineHeader> GlobalPipelineHeaders { get; set; }
        public virtual DbSet<GlobalPipelineRow> GlobalPipelineRows { get; set; }
        public virtual DbSet<GlobalUserUrl> GlobalUserUrls { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemCategory> ItemCategories { get; set; }
        public virtual DbSet<ItemUom> ItemUoms { get; set; }
        public virtual DbSet<LogPyrEmployeeSalaryDefault> LogPyrEmployeeSalaryDefaults { get; set; }
        public virtual DbSet<LogPyrEmployeeSalaryOther> LogPyrEmployeeSalaryOthers { get; set; }
        public virtual DbSet<LogTimeAttendanceHistory> LogTimeAttendanceHistories { get; set; }
        public virtual DbSet<LveLeaveApplication> LveLeaveApplications { get; set; }
        public virtual DbSet<LveLeaveBalance> LveLeaveBalances { get; set; }
        public virtual DbSet<LveLeaveType> LveLeaveTypes { get; set; }
        public virtual DbSet<LveMovementApplication> LveMovementApplications { get; set; }
        public virtual DbSet<LveMovementType> LveMovementTypes { get; set; }
        public virtual DbSet<ManagementDashboardPermission> ManagementDashboardPermissions { get; set; }
        public virtual DbSet<MasterAnnouncementType> MasterAnnouncementTypes { get; set; }
        public virtual DbSet<MasterBusinessUnit> MasterBusinessUnits { get; set; }
        public virtual DbSet<MasterDashboardComponent> MasterDashboardComponents { get; set; }
        public virtual DbSet<MasterDashboardComponentPermission> MasterDashboardComponentPermissions { get; set; }
        public virtual DbSet<MasterDepartment> MasterDepartments { get; set; }
        public virtual DbSet<MasterDesignation> MasterDesignations { get; set; }
        public virtual DbSet<MasterEmploymentType> MasterEmploymentTypes { get; set; }
        public virtual DbSet<MasterLocationRegister> MasterLocationRegisters { get; set; }
        public virtual DbSet<MasterPosition> MasterPositions { get; set; }
        public virtual DbSet<MasterTaxChallanConfig> MasterTaxChallanConfigs { get; set; }
        public virtual DbSet<MasterTerritoryType> MasterTerritoryTypes { get; set; }
        public virtual DbSet<MasterWorkplace> MasterWorkplaces { get; set; }
        public virtual DbSet<MasterWorkplaceGroup> MasterWorkplaceGroups { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<MenuPermission> MenuPermissions { get; set; }
        public virtual DbSet<NotificationCategoriesType> NotificationCategoriesTypes { get; set; }
        public virtual DbSet<NotificationCategory> NotificationCategories { get; set; }
        public virtual DbSet<NotificationDetail> NotificationDetails { get; set; }
        public virtual DbSet<NotificationMaster> NotificationMasters { get; set; }
        public virtual DbSet<NotificationPermissionDetail> NotificationPermissionDetails { get; set; }
        public virtual DbSet<NotificationPermissionMaster> NotificationPermissionMasters { get; set; }
        public virtual DbSet<NotifySendFailedLog> NotifySendFailedLogs { get; set; }
        public virtual DbSet<OrganizationType> OrganizationTypes { get; set; }
        public virtual DbSet<OverTimeConfiguration> OverTimeConfigurations { get; set; }
        public virtual DbSet<OverTimeConfigurationDetail> OverTimeConfigurationDetails { get; set; }
        public virtual DbSet<PipelineTypeDdl> PipelineTypeDdls { get; set; }
        public virtual DbSet<PolicyAcknowledge> PolicyAcknowledges { get; set; }
        public virtual DbSet<PolicyCategory> PolicyCategories { get; set; }
        public virtual DbSet<PolicyHeader> PolicyHeaders { get; set; }
        public virtual DbSet<PolicyRow> PolicyRows { get; set; }
        public virtual DbSet<PostOffice> PostOffices { get; set; }
        public virtual DbSet<PushNotifyDeviceRegistration> PushNotifyDeviceRegistrations { get; set; }
        public virtual DbSet<PyrArearSalaryGenerateHeader> PyrArearSalaryGenerateHeaders { get; set; }
        public virtual DbSet<PyrArearSalaryGenerateRequest> PyrArearSalaryGenerateRequests { get; set; }
        public virtual DbSet<PyrArearSalaryGenerateRequestRow> PyrArearSalaryGenerateRequestRows { get; set; }
        public virtual DbSet<PyrArearSalaryGenerateRow> PyrArearSalaryGenerateRows { get; set; }
        public virtual DbSet<PyrBonusGenerateHeader> PyrBonusGenerateHeaders { get; set; }
        public virtual DbSet<PyrBonusGenerateRow> PyrBonusGenerateRows { get; set; }
        public virtual DbSet<PyrBonusName> PyrBonusNames { get; set; }
        public virtual DbSet<PyrBonusSetup> PyrBonusSetups { get; set; }
        public virtual DbSet<PyrEmpSalaryAdditionNdeduction> PyrEmpSalaryAdditionNdeductions { get; set; }
        public virtual DbSet<PyrEmployeeSalaryDefault> PyrEmployeeSalaryDefaults { get; set; }
        public virtual DbSet<PyrEmployeeSalaryElementAssignHeader> PyrEmployeeSalaryElementAssignHeaders { get; set; }
        public virtual DbSet<PyrEmployeeSalaryElementAssignRow> PyrEmployeeSalaryElementAssignRows { get; set; }
        public virtual DbSet<PyrGrossWiseBasicConfig> PyrGrossWiseBasicConfigs { get; set; }
        public virtual DbSet<PyrIouadjustmentHistory> PyrIouadjustmentHistories { get; set; }
        public virtual DbSet<PyrIouapplication> PyrIouapplications { get; set; }
        public virtual DbSet<PyrIoudocument> PyrIoudocuments { get; set; }
        public virtual DbSet<PyrPayrollElementAndRulesTest> PyrPayrollElementAndRulesTests { get; set; }
        public virtual DbSet<PyrPayrollElementType> PyrPayrollElementTypes { get; set; }
        public virtual DbSet<PyrPayrollGroup> PyrPayrollGroups { get; set; }
        public virtual DbSet<PyrPayrollSalaryGenerateRequest> PyrPayrollSalaryGenerateRequests { get; set; }
        public virtual DbSet<PyrPayscaleGrade> PyrPayscaleGrades { get; set; }
        public virtual DbSet<PyrSalaryBreakdownHeader> PyrSalaryBreakdownHeaders { get; set; }
        public virtual DbSet<PyrSalaryBreakdownRow> PyrSalaryBreakdownRows { get; set; }
        public virtual DbSet<PyrSalaryGenerateHeader> PyrSalaryGenerateHeaders { get; set; }
        public virtual DbSet<PyrSalaryGenerateRow> PyrSalaryGenerateRows { get; set; }
        public virtual DbSet<PyrSalaryPolicy> PyrSalaryPolicies { get; set; }
        public virtual DbSet<RefreshTokenHistory> RefreshTokenHistories { get; set; }
        public virtual DbSet<Religion> Religions { get; set; }
        public virtual DbSet<RoleBridgeWithDesignation> RoleBridgeWithDesignations { get; set; }
        public virtual DbSet<RoleExtensionHeader> RoleExtensionHeaders { get; set; }
        public virtual DbSet<RoleExtensionRow> RoleExtensionRows { get; set; }
        public virtual DbSet<RoleGroupHeader> RoleGroupHeaders { get; set; }
        public virtual DbSet<RoleGroupRow> RoleGroupRows { get; set; }
        public virtual DbSet<TerritorySetup> TerritorySetups { get; set; }
        public virtual DbSet<Thana> Thanas { get; set; }
        public virtual DbSet<TimeAttendanceDailySummary> TimeAttendanceDailySummaries { get; set; }
        public virtual DbSet<TimeAttendanceProcessRequestHeader> TimeAttendanceProcessRequestHeaders { get; set; }
        public virtual DbSet<TimeAttendanceProcessRequestRow> TimeAttendanceProcessRequestRows { get; set; }
        public virtual DbSet<TimeCalender> TimeCalenders { get; set; }
        public virtual DbSet<TimeEmpOverTime> TimeEmpOverTimes { get; set; }
        public virtual DbSet<TimeEmpOverTimeUpload> TimeEmpOverTimeUploads { get; set; }
        public virtual DbSet<TimeEmployeeAttendance> TimeEmployeeAttendances { get; set; }
        public virtual DbSet<TimeEmployeeExcOffday> TimeEmployeeExcOffdays { get; set; }
        public virtual DbSet<TimeEmployeeHoliday> TimeEmployeeHolidays { get; set; }
        public virtual DbSet<TimeEmployeeOffday> TimeEmployeeOffdays { get; set; }
        public virtual DbSet<TimeEmployeeOffdayReassign> TimeEmployeeOffdayReassigns { get; set; }
        public virtual DbSet<TimeExceptionOffdayGroup> TimeExceptionOffdayGroups { get; set; }
        public virtual DbSet<TimeFixedRoasterSetupDetail> TimeFixedRoasterSetupDetails { get; set; }
        public virtual DbSet<TimeFixedRoasterSetupMaster> TimeFixedRoasterSetupMasters { get; set; }
        public virtual DbSet<TimeOffday> TimeOffdays { get; set; }
        public virtual DbSet<TimeOffdayGroup> TimeOffdayGroups { get; set; }
        public virtual DbSet<TimeRemoteAttendance> TimeRemoteAttendances { get; set; }
        public virtual DbSet<TimeRemoteAttendanceRegistration> TimeRemoteAttendanceRegistrations { get; set; }
        public virtual DbSet<TimeRemoteAttendanceSetup> TimeRemoteAttendanceSetups { get; set; }
        public virtual DbSet<TrainingAssesmentAnsware> TrainingAssesmentAnswares { get; set; }
        public virtual DbSet<TrainingAssesmentQuestion> TrainingAssesmentQuestions { get; set; }
        public virtual DbSet<TrainingAssesmentQuestionOption> TrainingAssesmentQuestionOptions { get; set; }
        public virtual DbSet<TrainingAttendance> TrainingAttendances { get; set; }
        public virtual DbSet<TrainingRequisition> TrainingRequisitions { get; set; }
        public virtual DbSet<TrainingSchedule> TrainingSchedules { get; set; }
        public virtual DbSet<TskBoard> TskBoards { get; set; }
        public virtual DbSet<TskGroupMember> TskGroupMembers { get; set; }
        public virtual DbSet<TskProject> TskProjects { get; set; }
        public virtual DbSet<TskTaskDetail> TskTaskDetails { get; set; }
        public virtual DbSet<Uom> Uoms { get; set; }
        public virtual DbSet<Url> Urls { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroupHeader> UserGroupHeaders { get; set; }
        public virtual DbSet<UserGroupRow> UserGroupRows { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<training> training { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=10.209.99.244;Initial Catalog=PeopleDeskAkijBiri;User ID=pepo;Password=pepo@dev;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.IntAccountId);

                entity.ToTable("Account", "core");

                entity.HasIndex(e => e.StrShortCode, "UQ__Account__29D7A135F9C10423")
                    .IsUnique();

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteDateOfOnboard)
                    .HasColumnType("date")
                    .HasColumnName("dteDateOfOnboard");

                entity.Property(e => e.DteExpireDate)
                    .HasColumnType("date")
                    .HasColumnName("dteExpireDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountPackageId).HasColumnName("intAccountPackageId");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntLogoUrlId).HasColumnName("intLogoUrlId");

                entity.Property(e => e.IntMaxEmployee).HasColumnName("intMaxEmployee");

                entity.Property(e => e.IntMinEmployee).HasColumnName("intMinEmployee");

                entity.Property(e => e.IntOwnerEmployeeId).HasColumnName("intOwnerEmployeeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntUrlId).HasColumnName("intUrlId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsBlock).HasColumnName("isBlock");

                entity.Property(e => e.IsEmployeeCodeAutoGenerated).HasColumnName("isEmployeeCodeAutoGenerated");

                entity.Property(e => e.IsFree).HasColumnName("isFree");

                entity.Property(e => e.IsInternCloseMenual).HasColumnName("isInternCloseMenual");

                entity.Property(e => e.IsLoan).HasColumnName("isLoan");

                entity.Property(e => e.IsLoggedInWithOtp).HasColumnName("isLoggedInWithOTP");

                entity.Property(e => e.IsOvertimeAutoCalculated)
                    .HasColumnName("isOvertimeAutoCalculated")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsProbationaryCloseMenual).HasColumnName("isProbationaryCloseMenual");

                entity.Property(e => e.IsProvidentFund).HasColumnName("isProvidentFund");

                entity.Property(e => e.IsTax).HasColumnName("isTax");

                entity.Property(e => e.NumFileStorageQuaota)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numFileStorageQuaota");

                entity.Property(e => e.NumPackageFileStorageQuaota)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numPackageFileStorageQuaota");

                entity.Property(e => e.NumPrice)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numPrice");

                entity.Property(e => e.StrAccountName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strAccountName");

                entity.Property(e => e.StrAccountPackageName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strAccountPackageName");

                entity.Property(e => e.StrAddress)
                    .HasMaxLength(250)
                    .HasColumnName("strAddress");

                entity.Property(e => e.StrBin)
                    .HasMaxLength(250)
                    .HasColumnName("strBIN");

                entity.Property(e => e.StrCurrency)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strCurrency");

                entity.Property(e => e.StrEmail)
                    .HasMaxLength(250)
                    .HasColumnName("strEmail");

                entity.Property(e => e.StrMobileNumber)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strMobileNumber");

                entity.Property(e => e.StrNid)
                    .HasMaxLength(250)
                    .HasColumnName("strNID");

                entity.Property(e => e.StrOwnerName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strOwnerName");

                entity.Property(e => e.StrShortCode)
                    .HasMaxLength(50)
                    .HasColumnName("strShortCode");

                entity.Property(e => e.StrWebsite)
                    .HasMaxLength(250)
                    .HasColumnName("strWebsite");
            });

            modelBuilder.Entity<AccountBankDetail>(entity =>
            {
                entity.HasKey(e => e.IntAccountBankDetailsId);

                entity.ToTable("AccountBankDetails", "core");

                entity.Property(e => e.IntAccountBankDetailsId).HasColumnName("intAccountBankDetailsId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBankBranchId).HasColumnName("intBankBranchId");

                entity.Property(e => e.IntBankOrWalletType).HasColumnName("intBankOrWalletType");

                entity.Property(e => e.IntBankWalletId).HasColumnName("intBankWalletId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrAccountName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strAccountName");

                entity.Property(e => e.StrAccountNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strAccountNo");

                entity.Property(e => e.StrBankWalletName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBankWalletName");

                entity.Property(e => e.StrBranchName)
                    .HasMaxLength(250)
                    .HasColumnName("strBranchName");

                entity.Property(e => e.StrDistrict)
                    .HasMaxLength(250)
                    .HasColumnName("strDistrict");

                entity.Property(e => e.StrRoutingNo)
                    .HasMaxLength(50)
                    .HasColumnName("strRoutingNo");

                entity.Property(e => e.StrSwiftCode)
                    .HasMaxLength(50)
                    .HasColumnName("strSwiftCode");
            });

            modelBuilder.Entity<AccountPackage>(entity =>
            {
                entity.HasKey(e => e.IntAccountPackageId)
                    .HasName("PK__AccountP__055624335A88D07F");

                entity.ToTable("AccountPackage", "core");

                entity.Property(e => e.IntAccountPackageId).HasColumnName("intAccountPackageId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntExpireInDays).HasColumnName("intExpireInDays");

                entity.Property(e => e.IntMaxEmployee).HasColumnName("intMaxEmployee");

                entity.Property(e => e.IntMinEmployee).HasColumnName("intMinEmployee");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsFree).HasColumnName("isFree");

                entity.Property(e => e.NumFileStorageQuaota)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numFileStorageQuaota");

                entity.Property(e => e.NumPrice)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numPrice");

                entity.Property(e => e.StrAccountPackageName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strAccountPackageName");
            });

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.HasKey(e => e.IntAnnouncementId);

                entity.ToTable("Announcement", "saas");

                entity.Property(e => e.IntAnnouncementId).HasColumnName("intAnnouncementId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteExpiredDate)
                    .HasColumnType("date")
                    .HasColumnName("dteExpiredDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntTypeId).HasColumnName("intTypeId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrDetails)
                    .IsRequired()
                    .HasColumnName("strDetails");

                entity.Property(e => e.StrTitle)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTitle");

                entity.Property(e => e.StrTypeName)
                    .HasMaxLength(250)
                    .HasColumnName("strTypeName");
            });

            modelBuilder.Entity<AnnouncementRow>(entity =>
            {
                entity.HasKey(e => e.IntAnnouncementRowId);

                entity.ToTable("AnnouncementRow", "saas");

                entity.Property(e => e.IntAnnouncementRowId).HasColumnName("intAnnouncementRowId");

                entity.Property(e => e.IntAnnoucementId).HasColumnName("intAnnoucementId");

                entity.Property(e => e.IntAnnouncementReferenceId).HasColumnName("intAnnouncementReferenceId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrAnnounceCode)
                    .HasMaxLength(50)
                    .HasColumnName("strAnnounceCode");

                entity.Property(e => e.StrAnnouncementFor)
                    .HasMaxLength(50)
                    .HasColumnName("strAnnouncementFor");
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.IntAssetId)
                    .HasName("PK_Asset_1");

                entity.ToTable("Asset", "saas");

                entity.Property(e => e.IntAssetId).HasColumnName("intAssetId");

                entity.Property(e => e.DteAcquisitionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAcquisitionDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteDepreciationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteDepreciationDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.DteWarrantyDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteWarrantyDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAcquisitionValue).HasColumnName("intAcquisitionValue");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDepreciationValue).HasColumnName("intDepreciationValue");

                entity.Property(e => e.IntInvoiceValue).HasColumnName("intInvoiceValue");

                entity.Property(e => e.IntItemId).HasColumnName("intItemId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrAssetCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strAssetCode");

                entity.Property(e => e.StrDescription)
                    .HasMaxLength(500)
                    .HasColumnName("strDescription");

                entity.Property(e => e.StrItemName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strItemName");

                entity.Property(e => e.StrSupplierMobileNo)
                    .HasMaxLength(50)
                    .HasColumnName("strSupplierMobileNo");

                entity.Property(e => e.StrSupplierName)
                    .HasMaxLength(250)
                    .HasColumnName("strSupplierName");
            });

            modelBuilder.Entity<AssetAssign>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("AssetAssign", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteAssignDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAssignDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.DteWithdrawDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteWithdrawDate");

                entity.Property(e => e.IntAssetId).HasColumnName("intAssetId");

                entity.Property(e => e.IntAssetRegisterId).HasColumnName("intAssetRegisterId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrAssetName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strAssetName");

                entity.Property(e => e.StrEmployeeName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrRegisterCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strRegisterCode");

                entity.Property(e => e.StrRemarks)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strRemarks");
            });

            modelBuilder.Entity<AssetDirectAssign>(entity =>
            {
                entity.HasKey(e => e.IntAssetDirectAssignId)
                    .HasName("PK_AssetDirectAssign_1");

                entity.ToTable("AssetDirectAssign", "saas");

                entity.Property(e => e.IntAssetDirectAssignId).HasColumnName("intAssetDirectAssignId");

                entity.Property(e => e.DteAssignDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAssignDate");

                entity.Property(e => e.DteCreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntItemId).HasColumnName("intItemId");

                entity.Property(e => e.IntItemQuantity).HasColumnName("intItemQuantity");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsAcknowledged).HasColumnName("isAcknowledged");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrStatus)
                    .HasMaxLength(50)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<AssetRequisition>(entity =>
            {
                entity.HasKey(e => e.IntAssetRequisitionId)
                    .HasName("PK_AssetRequisition_1");

                entity.ToTable("AssetRequisition", "saas");

                entity.Property(e => e.IntAssetRequisitionId).HasColumnName("intAssetRequisitionId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteReqisitionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteReqisitionDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntItemId).HasColumnName("intItemId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntReqisitionQuantity).HasColumnName("intReqisitionQuantity");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsAcknowledged).HasColumnName("isAcknowledged");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDenied).HasColumnName("isDenied");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(500)
                    .HasColumnName("strRemarks");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<AssetTransaction>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("AssetTransaction", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteTransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteTransactionDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAssetId).HasColumnName("intAssetId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntRegisterId).HasColumnName("intRegisterId");

                entity.Property(e => e.IntTransactionTypeId).HasColumnName("intTransactionTypeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsApprovedOrRejected).HasColumnName("isApprovedOrRejected");

                entity.Property(e => e.StrRemarks)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strRemarks");
            });

            modelBuilder.Entity<AssetTransfer>(entity =>
            {
                entity.HasKey(e => e.IntAssetTransferId);

                entity.ToTable("AssetTransfer", "saas");

                entity.Property(e => e.IntAssetTransferId).HasColumnName("intAssetTransferId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteTransferDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteTransferDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntFromEmployeeId).HasColumnName("intFromEmployeeId");

                entity.Property(e => e.IntItemId).HasColumnName("intItemId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntToEmployeeId).HasColumnName("intToEmployeeId");

                entity.Property(e => e.IntTransferQuantity).HasColumnName("intTransferQuantity");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsAcknowledged).HasColumnName("isAcknowledged");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(500)
                    .HasColumnName("strRemarks");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<AssetType>(entity =>
            {
                entity.HasKey(e => e.IntAssetTypeId);

                entity.ToTable("AssetType", "saas");

                entity.Property(e => e.IntAssetTypeId).HasColumnName("intAssetTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrAssetTypeName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strAssetTypeName");
            });

            modelBuilder.Entity<AssetsRegister>(entity =>
            {
                entity.HasKey(e => e.IntRegisterId);

                entity.ToTable("AssetsRegister", "saas");

                entity.Property(e => e.IntRegisterId).HasColumnName("intRegisterId");

                entity.Property(e => e.DteAcquisitionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAcquisitionDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.DteWarrantyDate)
                    .HasColumnType("date")
                    .HasColumnName("dteWarrantyDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAssetId).HasColumnName("intAssetId");

                entity.Property(e => e.IntAssetTypeId).HasColumnName("intAssetTypeId");

                entity.Property(e => e.IntAssignEmployeeId).HasColumnName("intAssignEmployeeId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUomId).HasColumnName("intUomId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrAssetName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strAssetName");

                entity.Property(e => e.StrAssetTypeName)
                    .HasMaxLength(250)
                    .HasColumnName("strAssetTypeName");

                entity.Property(e => e.StrRegisterCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strRegisterCode");

                entity.Property(e => e.StrSpecification)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strSpecification");

                entity.Property(e => e.StrUomName)
                    .HasMaxLength(250)
                    .HasColumnName("strUomName");
            });

            modelBuilder.Entity<AttendanceLateNotifyLog>(entity =>
            {
                entity.HasKey(e => e.IntAttendanceNotifyId)
                    .HasName("PK_AttendenceLateNotifyLog");

                entity.ToTable("AttendanceLateNotifyLog", "saas");

                entity.Property(e => e.IntAttendanceNotifyId).HasColumnName("intAttendanceNotifyId");

                entity.Property(e => e.DteAttendenceDate)
                    .HasColumnType("date")
                    .HasColumnName("dteAttendenceDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");
            });

            modelBuilder.Entity<BackgroundJobFailureHistory>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("BackgroundJobFailureHistory", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteLastExcute)
                    .HasColumnType("datetime")
                    .HasColumnName("dteLastExcute");

                entity.Property(e => e.StrModuleName)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("strModuleName");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(3000)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<BackgroundJobSuccessHistory>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("BackgroundJobSuccessHistory", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteLastExcute)
                    .HasColumnType("datetime")
                    .HasColumnName("dteLastExcute");

                entity.Property(e => e.StrModuleName)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("strModuleName");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(3000)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<BankBranch>(entity =>
            {
                entity.HasKey(e => e.IntBankBranchId);

                entity.ToTable("BankBranch", "core");

                entity.Property(e => e.IntBankBranchId).HasColumnName("intBankBranchId");

                entity.Property(e => e.DteInsertDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteInsertDateTime");

                entity.Property(e => e.IntBankId).HasColumnName("intBankId");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrBankBranchAddress)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("strBankBranchAddress");

                entity.Property(e => e.StrBankBranchCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strBankBranchCode");

                entity.Property(e => e.StrBankBranchName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strBankBranchName");

                entity.Property(e => e.StrBankCode)
                    .HasMaxLength(50)
                    .HasColumnName("strBankCode");

                entity.Property(e => e.StrBankName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strBankName");

                entity.Property(e => e.StrBankShortName)
                    .HasMaxLength(50)
                    .HasColumnName("strBankShortName");

                entity.Property(e => e.StrInsertUserId)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strInsertUserId");

                entity.Property(e => e.StrRoutingNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strRoutingNo");
            });

            modelBuilder.Entity<BankWallet>(entity =>
            {
                entity.HasKey(e => e.IntBankWalletId)
                    .HasName("PK__BankWall__619CAF3D0AFB83B5");

                entity.ToTable("BankWallet", "core");

                entity.Property(e => e.IntBankWalletId).HasColumnName("intBankWalletId");

                entity.Property(e => e.IntBankOrWalletType).HasColumnName("intBankOrWalletType");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrBankWalletName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBankWalletName");

                entity.Property(e => e.StrCode)
                    .HasMaxLength(50)
                    .HasColumnName("strCode");

                entity.Property(e => e.StrShortName)
                    .HasMaxLength(50)
                    .HasColumnName("strShortName");
            });

            modelBuilder.Entity<BloodGroup>(entity =>
            {
                entity.HasKey(e => e.IntBloodGroupId)
                    .HasName("PK__BloodGro__DF88351E1C1BA8E3");

                entity.ToTable("BloodGroup", "core");

                entity.HasIndex(e => e.StrBloodGroupCode, "UQ__BloodGro__06387C70575919AC")
                    .IsUnique();

                entity.Property(e => e.IntBloodGroupId)
                    .ValueGeneratedNever()
                    .HasColumnName("intBloodGroupId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrBloodGroup)
                    .HasMaxLength(250)
                    .HasColumnName("strBloodGroup");

                entity.Property(e => e.StrBloodGroupCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strBloodGroupCode");
            });

            modelBuilder.Entity<CafCafeteriaDetail>(entity =>
            {
                entity.HasKey(e => e.IntRow)
                    .IsClustered(false);

                entity.ToTable("cafCafeteriaDetails", "saas");

                entity.Property(e => e.IntRow).HasColumnName("intRow");

                entity.Property(e => e.DteAction)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAction");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteMeal)
                    .HasColumnType("date")
                    .HasColumnName("dteMeal");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntCountMeal).HasColumnName("intCountMeal");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEnroll).HasColumnName("intEnroll");

                entity.Property(e => e.IntMealFor).HasColumnName("intMealFor");

                entity.Property(e => e.IntSpendMeal).HasColumnName("intSpendMeal");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsOwnGuest).HasColumnName("isOwnGuest");

                entity.Property(e => e.IsPayable).HasColumnName("isPayable");

                entity.Property(e => e.StrNarration)
                    .HasMaxLength(250)
                    .HasColumnName("strNarration");

                entity.Property(e => e.YsnMenualEntry).HasColumnName("ysnMenualEntry");
            });

            modelBuilder.Entity<CafCafeterium>(entity =>
            {
                entity.HasKey(e => e.IntRow)
                    .HasName("PK_tblCafeteria");

                entity.ToTable("cafCafeteria", "saas");

                entity.Property(e => e.IntRow).HasColumnName("intRow");

                entity.Property(e => e.DteAction)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAction");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEnroll).HasColumnName("intEnroll");

                entity.Property(e => e.IntGroup).HasColumnName("intGroup");

                entity.Property(e => e.IntMealOption).HasColumnName("intMealOption");

                entity.Property(e => e.IntType).HasColumnName("intType");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.StrNarration)
                    .HasMaxLength(250)
                    .HasColumnName("strNarration");
            });

            modelBuilder.Entity<CafMenuListOfFoodCorner>(entity =>
            {
                entity.HasKey(e => e.IntAutoId);

                entity.ToTable("cafMenuListOfFoodCorner", "saas");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoID");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.StrDayName)
                    .HasMaxLength(100)
                    .HasColumnName("strDayName");

                entity.Property(e => e.StrMenu)
                    .HasMaxLength(500)
                    .HasColumnName("strMenu");

                entity.Property(e => e.StrStatus)
                    .HasMaxLength(100)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.IntCountryId)
                    .HasName("PK__Country__D0D4B4D95BF02882");

                entity.ToTable("Country", "core");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrCountry)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strCountry");

                entity.Property(e => e.StrCountryCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strCountryCode");

                entity.Property(e => e.StrDialingCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strDialingCode");

                entity.Property(e => e.StrNationality)
                    .HasMaxLength(250)
                    .HasColumnName("strNationality");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.IntCurrencyId)
                    .HasName("PK__Currency__E84D7A1359A6F045");

                entity.ToTable("Currency", "core");

                entity.Property(e => e.IntCurrencyId).HasColumnName("intCurrencyId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrCurrency)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strCurrency");

                entity.Property(e => e.StrCurrencyCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strCurrencyCode");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasKey(e => e.IntDistrictId);

                entity.ToTable("District", "core");

                entity.Property(e => e.IntDistrictId).HasColumnName("intDistrictId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IntDivisionId).HasColumnName("intDivisionId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrDistrict)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDistrict");

                entity.Property(e => e.StrDistrictBn)
                    .HasMaxLength(250)
                    .HasColumnName("strDistrictBN");

                entity.Property(e => e.StrDistrictCode)
                    .HasMaxLength(50)
                    .HasColumnName("strDistrictCode");

                entity.Property(e => e.StrLatitude)
                    .HasMaxLength(250)
                    .HasColumnName("strLatitude");

                entity.Property(e => e.StrLongitude)
                    .HasMaxLength(250)
                    .HasColumnName("strLongitude");
            });

            modelBuilder.Entity<Division>(entity =>
            {
                entity.HasKey(e => e.IntDivisionId);

                entity.ToTable("Division", "core");

                entity.Property(e => e.IntDivisionId).HasColumnName("intDivisionId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrDivision)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDivision");

                entity.Property(e => e.StrDivisionBn)
                    .HasMaxLength(250)
                    .HasColumnName("strDivisionBN");

                entity.Property(e => e.StrDivisionCode)
                    .HasMaxLength(50)
                    .HasColumnName("strDivisionCode");

                entity.Property(e => e.StrLatitude)
                    .HasMaxLength(250)
                    .HasColumnName("strLatitude");

                entity.Property(e => e.StrLongitude)
                    .HasMaxLength(250)
                    .HasColumnName("strLongitude");
            });

            modelBuilder.Entity<EducationDegree>(entity =>
            {
                entity.HasKey(e => e.IntEmployeeEducationId)
                    .HasName("PK__Educatio__2F51D39CE54775CE");

                entity.ToTable("EducationDegree", "core");

                entity.Property(e => e.IntEmployeeEducationId)
                    .ValueGeneratedNever()
                    .HasColumnName("intEmployeeEducationId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrEducationDegree)
                    .HasMaxLength(250)
                    .HasColumnName("strEducationDegree");
            });

            modelBuilder.Entity<EducationFieldOfStudy>(entity =>
            {
                entity.HasKey(e => e.IntEducationFieldOfStudyId)
                    .HasName("PK__Educatio__E8A5B1336003B461");

                entity.ToTable("EducationFieldOfStudy", "core");

                entity.Property(e => e.IntEducationFieldOfStudyId).HasColumnName("intEducationFieldOfStudyId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrEducationFieldOfStudy)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEducationFieldOfStudy");
            });

            modelBuilder.Entity<EmpEmployeeAddress>(entity =>
            {
                entity.HasKey(e => e.IntEmployeeAddressId)
                    .HasName("PK__empEmplo__67ED467059F6F67F");

                entity.ToTable("empEmployeeAddress", "saas");

                entity.Property(e => e.IntEmployeeAddressId).HasColumnName("intEmployeeAddressId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAddressTypeId).HasColumnName("intAddressTypeId");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDistrictOrStateId).HasColumnName("intDistrictOrStateId");

                entity.Property(e => e.IntDivisionId).HasColumnName("intDivisionId");

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.IntPostOfficeId).HasColumnName("intPostOfficeId");

                entity.Property(e => e.IntThanaId).HasColumnName("intThanaId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrAddressDetails)
                    .HasMaxLength(2000)
                    .HasColumnName("strAddressDetails");

                entity.Property(e => e.StrAddressType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strAddressType");

                entity.Property(e => e.StrCountry)
                    .HasMaxLength(250)
                    .HasColumnName("strCountry");

                entity.Property(e => e.StrDistrictOrState)
                    .HasMaxLength(250)
                    .HasColumnName("strDistrictOrState");

                entity.Property(e => e.StrDivision)
                    .HasMaxLength(250)
                    .HasColumnName("strDivision");

                entity.Property(e => e.StrPostOffice)
                    .HasMaxLength(250)
                    .HasColumnName("strPostOffice");

                entity.Property(e => e.StrThana)
                    .HasMaxLength(250)
                    .HasColumnName("strThana");

                entity.Property(e => e.StrZipOrPostCode)
                    .HasMaxLength(50)
                    .HasColumnName("strZipOrPostCode");
            });

            modelBuilder.Entity<EmpEmployeeBankDetail>(entity =>
            {
                entity.HasKey(e => e.IntEmployeeBankDetailsId)
                    .HasName("PK__empEmplo__F1DF0389323E2897");

                entity.ToTable("empEmployeeBankDetails", "saas");

                entity.Property(e => e.IntEmployeeBankDetailsId).HasColumnName("intEmployeeBankDetailsId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBankBranchId).HasColumnName("intBankBranchId");

                entity.Property(e => e.IntBankOrWalletType).HasColumnName("intBankOrWalletType");

                entity.Property(e => e.IntBankWalletId).HasColumnName("intBankWalletId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPrimarySalaryAccount).HasColumnName("isPrimarySalaryAccount");

                entity.Property(e => e.StrAccountName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strAccountName");

                entity.Property(e => e.StrAccountNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strAccountNo");

                entity.Property(e => e.StrBankWalletName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBankWalletName");

                entity.Property(e => e.StrBranchName)
                    .HasMaxLength(250)
                    .HasColumnName("strBranchName");

                entity.Property(e => e.StrDistrict)
                    .HasMaxLength(250)
                    .HasColumnName("strDistrict");

                entity.Property(e => e.StrRoutingNo)
                    .HasMaxLength(50)
                    .HasColumnName("strRoutingNo");

                entity.Property(e => e.StrSwiftCode)
                    .HasMaxLength(50)
                    .HasColumnName("strSwiftCode");
            });

            modelBuilder.Entity<EmpEmployeeBasicInfo>(entity =>
            {
                entity.HasKey(e => e.IntEmployeeBasicInfoId)
                    .HasName("PK__empEmplo__7722B4BB3ADC462D");

                entity.ToTable("empEmployeeBasicInfo", "saas");

                entity.HasIndex(e => new { e.IntAccountId, e.StrEmployeeCode }, "unq_employee_code")
                    .IsUnique();

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.DteConfirmationDate)
                    .HasColumnType("date")
                    .HasColumnName("dteConfirmationDate");

                entity.Property(e => e.DteContactFromDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteContactFromDate");

                entity.Property(e => e.DteContactToDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteContactToDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteDateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("dteDateOfBirth");

                entity.Property(e => e.DteInternCloseDate)
                    .HasColumnType("date")
                    .HasColumnName("dteInternCloseDate");

                entity.Property(e => e.DteJoiningDate)
                    .HasColumnType("date")
                    .HasColumnName("dteJoiningDate");

                entity.Property(e => e.DteLastWorkingDate)
                    .HasColumnType("date")
                    .HasColumnName("dteLastWorkingDate");

                entity.Property(e => e.DteProbationaryCloseDate)
                    .HasColumnType("date")
                    .HasColumnName("dteProbationaryCloseDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.IntDottedSupervisorId).HasColumnName("intDottedSupervisorId");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntGenderId).HasColumnName("intGenderId");

                entity.Property(e => e.IntLineManagerId).HasColumnName("intLineManagerId");

                entity.Property(e => e.IntReligionId).HasColumnName("intReligionId");

                entity.Property(e => e.IntSupervisorId).HasColumnName("intSupervisorId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsRemoteAttendance).HasColumnName("isRemoteAttendance");

                entity.Property(e => e.IsSalaryHold).HasColumnName("isSalaryHold");

                entity.Property(e => e.IsUserInactive).HasColumnName("isUserInactive");

                entity.Property(e => e.StrBloodGroup)
                    .HasMaxLength(50)
                    .HasColumnName("strBloodGroup");

                entity.Property(e => e.StrCardNumber)
                    .HasMaxLength(20)
                    .HasColumnName("strCardNumber");

                entity.Property(e => e.StrEmployeeCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("strEmployeeCode");

                entity.Property(e => e.StrEmployeeName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrEmploymentType)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmploymentType");

                entity.Property(e => e.StrGender)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strGender");

                entity.Property(e => e.StrMaritalStatus)
                    .HasMaxLength(50)
                    .HasColumnName("strMaritalStatus");

                entity.Property(e => e.StrReferenceId)
                    .HasMaxLength(250)
                    .HasColumnName("strReferenceId");

                entity.Property(e => e.StrReligion)
                    .HasMaxLength(250)
                    .HasColumnName("strReligion");
            });

            modelBuilder.Entity<EmpEmployeeBasicInfoDetail>(entity =>
            {
                entity.HasKey(e => e.IntDetailsId)
                    .HasName("PK_empEmployeeBasicDetails");

                entity.ToTable("empEmployeeBasicInfoDetails", "saas");

                entity.Property(e => e.IntDetailsId).HasColumnName("intDetailsId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt).HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAreaId).HasColumnName("intAreaId");

                entity.Property(e => e.IntCalenderId).HasColumnName("intCalenderId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmployeeStatusId).HasColumnName("intEmployeeStatusId");

                entity.Property(e => e.IntHrpositionId).HasColumnName("intHRPositionId");

                entity.Property(e => e.IntPayrollGroupId).HasColumnName("intPayrollGroupId");

                entity.Property(e => e.IntPayscaleGradeId).HasColumnName("intPayscaleGradeId");

                entity.Property(e => e.IntRegionId).HasColumnName("intRegionId");

                entity.Property(e => e.IntSoleDepo).HasColumnName("intSoleDepo");

                entity.Property(e => e.IntTerritoryId).HasColumnName("intTerritoryId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWingId).HasColumnName("intWingId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsTakeHomePay).HasColumnName("isTakeHomePay");

                entity.Property(e => e.StrCalenderName)
                    .HasMaxLength(250)
                    .HasColumnName("strCalenderName");

                entity.Property(e => e.StrDrivingLicenseNo)
                    .HasMaxLength(250)
                    .HasColumnName("strDrivingLicenseNo");

                entity.Property(e => e.StrEmployeeStatus)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeStatus");

                entity.Property(e => e.StrHrpostionName)
                    .HasMaxLength(250)
                    .HasColumnName("strHRPostionName");

                entity.Property(e => e.StrOfficeMail)
                    .HasMaxLength(250)
                    .HasColumnName("strOfficeMail");

                entity.Property(e => e.StrOfficeMobile)
                    .HasMaxLength(50)
                    .HasColumnName("strOfficeMobile");

                entity.Property(e => e.StrPayrollGroupName)
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollGroupName");

                entity.Property(e => e.StrPayscaleGradeName)
                    .HasMaxLength(250)
                    .HasColumnName("strPayscaleGradeName");

                entity.Property(e => e.StrPersonalMail)
                    .HasMaxLength(250)
                    .HasColumnName("strPersonalMail");

                entity.Property(e => e.StrPersonalMobile)
                    .HasMaxLength(50)
                    .HasColumnName("strPersonalMobile");

                entity.Property(e => e.StrPinNo)
                    .HasMaxLength(250)
                    .HasColumnName("strPinNo");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(3000)
                    .HasColumnName("strRemarks");

                entity.Property(e => e.StrVehicleNo)
                    .HasMaxLength(250)
                    .HasColumnName("strVehicleNo");
            });

            modelBuilder.Entity<EmpEmployeeDocumentManagement>(entity =>
            {
                entity.HasKey(e => e.IntDocumentManagementId);

                entity.ToTable("empEmployeeDocumentManagement", "saas");

                entity.Property(e => e.IntDocumentManagementId).HasColumnName("intDocumentManagementId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDocumentTypeId).HasColumnName("intDocumentTypeId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntFileUrlId).HasColumnName("intFileUrlId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrDocumentType)
                    .HasMaxLength(250)
                    .HasColumnName("strDocumentType");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");
            });

            modelBuilder.Entity<EmpEmployeeEducation>(entity =>
            {
                entity.HasKey(e => e.IntEmployeeEducationId)
                    .HasName("PK__empEmplo__2F51D39C73577E01");

                entity.ToTable("empEmployeeEducation", "saas");

                entity.Property(e => e.IntEmployeeEducationId).HasColumnName("intEmployeeEducationId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteEndDate)
                    .HasColumnType("date")
                    .HasColumnName("dteEndDate");

                entity.Property(e => e.DteStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dteStartDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCertificateFileUrlId).HasColumnName("intCertificateFileUrlId");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEducationDegreeId).HasColumnName("intEducationDegreeId");

                entity.Property(e => e.IntEducationFieldOfStudyId).HasColumnName("intEducationFieldOfStudyId");

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.IntInstituteId).HasColumnName("intInstituteId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsForeign).HasColumnName("isForeign");

                entity.Property(e => e.StrCgpa)
                    .HasMaxLength(50)
                    .HasColumnName("strCGPA");

                entity.Property(e => e.StrCountry)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strCountry");

                entity.Property(e => e.StrEducationDegree)
                    .HasMaxLength(250)
                    .HasColumnName("strEducationDegree");

                entity.Property(e => e.StrEducationFieldOfStudy)
                    .HasMaxLength(250)
                    .HasColumnName("strEducationFieldOfStudy");

                entity.Property(e => e.StrInstituteName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strInstituteName");

                entity.Property(e => e.StrOutOf)
                    .HasMaxLength(50)
                    .HasColumnName("strOutOf");
            });

            modelBuilder.Entity<EmpEmployeeFile>(entity =>
            {
                entity.HasKey(e => e.IntEmployeeFileId)
                    .HasName("PK__empEmplo__3876D516503E8372");

                entity.ToTable("empEmployeeFile", "saas");

                entity.Property(e => e.IntEmployeeFileId).HasColumnName("intEmployeeFileId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDocumentTypeId).HasColumnName("intDocumentTypeId");

                entity.Property(e => e.IntEmployeeFileUrlId).HasColumnName("intEmployeeFileUrlId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrFileTitle)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strFileTitle");

                entity.Property(e => e.StrTags).HasColumnName("strTags");
            });

            modelBuilder.Entity<EmpEmployeeIncrement>(entity =>
            {
                entity.HasKey(e => e.IntIncrementId)
                    .HasName("PK_empEmployeeIncremnt");

                entity.ToTable("empEmployeeIncrement", "saas");

                entity.Property(e => e.IntIncrementId).HasColumnName("intIncrementId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteEffectiveDate)
                    .HasColumnType("date")
                    .HasColumnName("dteEffectiveDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntNewSalaryElementAssignHeaderId).HasColumnName("intNewSalaryElementAssignHeaderId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntOldSalaryElementAssignHeaderId).HasColumnName("intOldSalaryElementAssignHeaderId");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntTransferNpromotionReferenceId).HasColumnName("intTransferNPromotionReferenceId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsProcess).HasColumnName("isProcess");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.NumIncrementAmount)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numIncrementAmount");

                entity.Property(e => e.NumIncrementDependOn)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numIncrementDependOn");

                entity.Property(e => e.NumIncrementPercentage)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numIncrementPercentage");

                entity.Property(e => e.NumOldGrossAmount)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numOldGrossAmount");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrIncrementDependOn)
                    .HasMaxLength(50)
                    .HasColumnName("strIncrementDependOn");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<EmpEmployeeJobHistory>(entity =>
            {
                entity.HasKey(e => e.IntJobExperienceId);

                entity.ToTable("empEmployeeJobHistory", "saas");

                entity.Property(e => e.IntJobExperienceId).HasColumnName("intJobExperienceId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteFromDate)
                    .HasColumnType("date")
                    .HasColumnName("dteFromDate");

                entity.Property(e => e.DteToDate)
                    .HasColumnType("date")
                    .HasColumnName("dteToDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.IntNocfileUrlId).HasColumnName("intNOCFileUrlId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrCompanyName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strCompanyName");

                entity.Property(e => e.StrJobTitle)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strJobTitle");

                entity.Property(e => e.StrLocation)
                    .HasMaxLength(250)
                    .HasColumnName("strLocation");

                entity.Property(e => e.StrRemarks).HasColumnName("strRemarks");
            });

            modelBuilder.Entity<EmpEmployeePhotoIdentity>(entity =>
            {
                entity.HasKey(e => e.IntEmployeePhotoIdentityId)
                    .HasName("PK__empEmplo__A7E2003180AC522F");

                entity.ToTable("empEmployeePhotoIdentity", "saas");

                entity.HasIndex(e => new { e.IntEmployeeBasicInfoId, e.IsActive }, "UQ_Employee")
                    .IsUnique();

                entity.Property(e => e.IntEmployeePhotoIdentityId).HasColumnName("intEmployeePhotoIdentityId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntBirthIdfileUrlId).HasColumnName("intBirthIDFileUrlId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.IntNidfileUrlId).HasColumnName("intNIDFileUrlId");

                entity.Property(e => e.IntPassportFileUrlId).HasColumnName("intPassportFileUrlId");

                entity.Property(e => e.IntProfilePicFileUrlId).HasColumnName("intProfilePicFileUrlId");

                entity.Property(e => e.IntProfilePicFormalFileUrlId).HasColumnName("intProfilePicFormalFileUrlId");

                entity.Property(e => e.IntSignatureFileUrlId).HasColumnName("intSignatureFileUrlId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrBiography).HasColumnName("strBiography");

                entity.Property(e => e.StrBirthId)
                    .HasMaxLength(50)
                    .HasColumnName("strBirthID");

                entity.Property(e => e.StrHobbies).HasColumnName("strHobbies");

                entity.Property(e => e.StrNationality)
                    .HasMaxLength(250)
                    .HasColumnName("strNationality");

                entity.Property(e => e.StrNid)
                    .HasMaxLength(50)
                    .HasColumnName("strNID");

                entity.Property(e => e.StrPassport)
                    .HasMaxLength(50)
                    .HasColumnName("strPassport");
            });

            modelBuilder.Entity<EmpEmployeeRelativesContact>(entity =>
            {
                entity.HasKey(e => e.IntEmployeeRelativesContactId)
                    .HasName("PK__empEmplo__9EB209AB6EC44029");

                entity.ToTable("empEmployeeRelativesContact", "saas");

                entity.Property(e => e.IntEmployeeRelativesContactId).HasColumnName("intEmployeeRelativesContactId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteDateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("dteDateOfBirth");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.IntGrantorNomineeTypeId).HasColumnName("intGrantorNomineeTypeId");

                entity.Property(e => e.IntPictureFileUrlId).HasColumnName("intPictureFileUrlId");

                entity.Property(e => e.IntRelationShipId).HasColumnName("intRelationShipId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsEmergencyContact).HasColumnName("isEmergencyContact");

                entity.Property(e => e.StrAddress)
                    .HasMaxLength(250)
                    .HasColumnName("strAddress");

                entity.Property(e => e.StrBirthId)
                    .HasMaxLength(50)
                    .HasColumnName("strBirthID");

                entity.Property(e => e.StrEmail)
                    .HasMaxLength(50)
                    .HasColumnName("strEmail");

                entity.Property(e => e.StrGrantorNomineeType)
                    .HasMaxLength(250)
                    .HasColumnName("strGrantorNomineeType");

                entity.Property(e => e.StrNid)
                    .HasMaxLength(50)
                    .HasColumnName("strNID");

                entity.Property(e => e.StrPhone)
                    .HasMaxLength(50)
                    .HasColumnName("strPhone");

                entity.Property(e => e.StrRelationship)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strRelationship");

                entity.Property(e => e.StrRelativesName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strRelativesName");

                entity.Property(e => e.StrRemarks).HasColumnName("strRemarks");
            });

            modelBuilder.Entity<EmpEmployeeSeparation>(entity =>
            {
                entity.HasKey(e => e.IntSeparationId)
                    .HasName("PK__empEmplo__9460756390328C30");

                entity.ToTable("empEmployeeSeparation", "saas");

                entity.Property(e => e.IntSeparationId).HasColumnName("intSeparationId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteLastWorkingDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteLastWorkingDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteSeparationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteSeparationDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntSeparationTypeId).HasColumnName("intSeparationTypeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.IsRejoin).HasColumnName("isRejoin");

                entity.Property(e => e.IsReleased).HasColumnName("isReleased");

                entity.Property(e => e.StrDocumentId)
                    .HasMaxLength(500)
                    .HasColumnName("strDocumentId");

                entity.Property(e => e.StrEmployeeCode)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeCode");

                entity.Property(e => e.StrEmployeeName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrReason).HasColumnName("strReason");

                entity.Property(e => e.StrSeparationTypeName)
                    .HasMaxLength(250)
                    .HasColumnName("strSeparationTypeName");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<EmpEmployeeTraining>(entity =>
            {
                entity.HasKey(e => e.IntTrainingId);

                entity.ToTable("empEmployeeTraining", "saas");

                entity.Property(e => e.IntTrainingId).HasColumnName("intTrainingId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteEndDate)
                    .HasColumnType("date")
                    .HasColumnName("dteEndDate");

                entity.Property(e => e.DteExpiryDate)
                    .HasColumnType("date")
                    .HasColumnName("dteExpiryDate");

                entity.Property(e => e.DteStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dteStartDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.IntInstituteId).HasColumnName("intInstituteId");

                entity.Property(e => e.IntTrainingFileUrlId).HasColumnName("intTrainingFileUrlId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsForeign).HasColumnName("isForeign");

                entity.Property(e => e.StrCountry)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strCountry");

                entity.Property(e => e.StrInstituteName)
                    .HasMaxLength(250)
                    .HasColumnName("strInstituteName");

                entity.Property(e => e.StrTrainingTitle)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTrainingTitle");
            });

            modelBuilder.Entity<EmpEmploymentTypeWiseLeaveBalance>(entity =>
            {
                entity.HasKey(e => e.IntId)
                    .HasName("PK_EmpEmploymentTypeWiseLeaveBalance");

                entity.ToTable("empEmploymentTypeWiseLeaveBalance", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAllocatedLeave).HasColumnName("intAllocatedLeave");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntGenderId).HasColumnName("intGenderId");

                entity.Property(e => e.IntLeaveTypeId).HasColumnName("intLeaveTypeId");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IntYearId).HasColumnName("intYearId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrGender)
                    .HasMaxLength(150)
                    .HasColumnName("strGender");
            });

            modelBuilder.Entity<EmpExpenseApplication>(entity =>
            {
                entity.HasKey(e => e.IntExpenseId);

                entity.ToTable("empExpenseApplication", "saas");

                entity.Property(e => e.IntExpenseId).HasColumnName("intExpenseId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteExpenseFromDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteExpenseFromDate");

                entity.Property(e => e.DteExpenseToDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteExpenseToDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntDocumentId).HasColumnName("intDocumentId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntExpenseTypeId).HasColumnName("intExpenseTypeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.NumExpenseAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numExpenseAmount");

                entity.Property(e => e.StrDiscription)
                    .HasMaxLength(1000)
                    .HasColumnName("strDiscription");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<EmpExpenseDocument>(entity =>
            {
                entity.HasKey(e => e.IntExpenseDocId);

                entity.ToTable("empExpenseDocument", "saas");

                entity.Property(e => e.IntExpenseDocId).HasColumnName("intExpenseDocId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDocUrlid).HasColumnName("intDocURLId");

                entity.Property(e => e.IntExpenseId).HasColumnName("intExpenseId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrDocFor)
                    .HasMaxLength(150)
                    .HasColumnName("strDocFor");
            });

            modelBuilder.Entity<EmpExpenseType>(entity =>
            {
                entity.HasKey(e => e.IntExpenseTypeId);

                entity.ToTable("empExpenseType", "saas");

                entity.Property(e => e.IntExpenseTypeId).HasColumnName("intExpenseTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrExpenseType)
                    .HasMaxLength(250)
                    .HasColumnName("strExpenseType");
            });

            modelBuilder.Entity<EmpJobExperience>(entity =>
            {
                entity.HasKey(e => e.IntJobExperienceId)
                    .HasName("PK__empJobEx__7FE445C05BD9C90D");

                entity.ToTable("empJobExperience", "saas");

                entity.Property(e => e.IntJobExperienceId).HasColumnName("intJobExperienceId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteFromDate)
                    .HasColumnType("date")
                    .HasColumnName("dteFromDate");

                entity.Property(e => e.DteToDate)
                    .HasColumnType("date")
                    .HasColumnName("dteToDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.IntNocUrlId).HasColumnName("intNocUrlId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrCompanyName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strCompanyName");

                entity.Property(e => e.StrDescription).HasColumnName("strDescription");

                entity.Property(e => e.StrJobTitle)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strJobTitle");

                entity.Property(e => e.StrLocation)
                    .HasMaxLength(250)
                    .HasColumnName("strLocation");
            });

            modelBuilder.Entity<EmpLoanApplication>(entity =>
            {
                entity.HasKey(e => e.IntLoanApplicationId)
                    .HasName("PK__empLoanA__531A92869683C907");

                entity.ToTable("empLoanApplication", "saas");

                entity.Property(e => e.IntLoanApplicationId).HasColumnName("intLoanApplicationId");

                entity.Property(e => e.DteApplicationDate)
                    .HasColumnType("date")
                    .HasColumnName("dteApplicationDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteEffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteEffectiveDate");

                entity.Property(e => e.DteReScheduleDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteReScheduleDateTime");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntApproveLoanAmount).HasColumnName("intApproveLoanAmount");

                entity.Property(e => e.IntApproveNumberOfInstallment).HasColumnName("intApproveNumberOfInstallment");

                entity.Property(e => e.IntApproveNumberOfInstallmentAmount).HasColumnName("intApproveNumberOfInstallmentAmount");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntFileUrlId).HasColumnName("intFileUrlId");

                entity.Property(e => e.IntLoanAmount).HasColumnName("intLoanAmount");

                entity.Property(e => e.IntLoanTypeId).HasColumnName("intLoanTypeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntNumberOfInstallment).HasColumnName("intNumberOfInstallment");

                entity.Property(e => e.IntNumberOfInstallmentAmount).HasColumnName("intNumberOfInstallmentAmount");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntReScheduleCount).HasColumnName("intReScheduleCount");

                entity.Property(e => e.IntReScheduleNumberOfInstallment).HasColumnName("intReScheduleNumberOfInstallment");

                entity.Property(e => e.IntReScheduleNumberOfInstallmentAmount).HasColumnName("intReScheduleNumberOfInstallmentAmount");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsHold).HasColumnName("isHold");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.NumRemainingBalance).HasColumnName("numRemainingBalance");

                entity.Property(e => e.StrDescription).HasColumnName("strDescription");

                entity.Property(e => e.StrReScheduleRemarks).HasColumnName("strReScheduleRemarks");

                entity.Property(e => e.StrReferenceNo)
                    .HasMaxLength(250)
                    .HasColumnName("strReferenceNo");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<EmpLoanSchedule>(entity =>
            {
                entity.HasKey(e => e.IntScheduleId)
                    .HasName("PK__empLoanS__8DCD5938600D68D0");

                entity.ToTable("empLoanSchedule", "saas");

                entity.Property(e => e.IntScheduleId).HasColumnName("intScheduleId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteInstallmentDate)
                    .HasMaxLength(250)
                    .HasColumnName("dteInstallmentDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntInstallmentAmount).HasColumnName("intInstallmentAmount");

                entity.Property(e => e.IntLoanApplicationId).HasColumnName("intLoanApplicationId");

                entity.Property(e => e.IntMonth)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("intMonth");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntYear).HasColumnName("intYear");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.YsnEnable)
                    .HasMaxLength(250)
                    .HasColumnName("ysnEnable");

                entity.Property(e => e.YsnInstallmentStatus)
                    .HasMaxLength(250)
                    .HasColumnName("ysnInstallmentStatus");
            });

            modelBuilder.Entity<EmpLoanType>(entity =>
            {
                entity.HasKey(e => e.IntLoanTypeId)
                    .HasName("PK__empLoanT__E249FB7E0A60B1FF");

                entity.ToTable("empLoanType", "saas");

                entity.Property(e => e.IntLoanTypeId).HasColumnName("intLoanTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrLoanType)
                    .HasMaxLength(250)
                    .HasColumnName("strLoanType");
            });

            modelBuilder.Entity<EmpManualAttendanceSummary>(entity =>
            {
                entity.HasKey(e => e.IntId)
                    .HasName("PK_tblManualAttendanceSummary");

                entity.ToTable("empManualAttendanceSummary", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteAttendanceDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAttendanceDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAttendanceSummaryId).HasColumnName("intAttendanceSummaryId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrCurrentStatus)
                    .HasMaxLength(50)
                    .HasColumnName("strCurrentStatus");

                entity.Property(e => e.StrRemarks).HasColumnName("strRemarks");

                entity.Property(e => e.StrRequestStatus)
                    .HasMaxLength(50)
                    .HasColumnName("strRequestStatus");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");

                entity.Property(e => e.TimeInTime).HasColumnName("timeInTime");

                entity.Property(e => e.TimeOutTime).HasColumnName("timeOutTime");
            });

            modelBuilder.Entity<EmpPfInvestmentHeader>(entity =>
            {
                entity.HasKey(e => e.IntInvenstmentHeaderId);

                entity.ToTable("empPfInvestmentHeader", "saas");

                entity.Property(e => e.IntInvenstmentHeaderId).HasColumnName("intInvenstmentHeaderId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteInvestmentDate)
                    .HasColumnType("date")
                    .HasColumnName("dteInvestmentDate");

                entity.Property(e => e.DteMatureDate)
                    .HasColumnType("date")
                    .HasColumnName("dteMatureDate");

                entity.Property(e => e.DtePfPeriodFromMonthYear)
                    .HasColumnType("date")
                    .HasColumnName("dtePfPeriodFromMonthYear");

                entity.Property(e => e.DtePfPeriodToMonthYear)
                    .HasColumnType("date")
                    .HasColumnName("dtePfPeriodToMonthYear");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBankBranchId).HasColumnName("intBankBranchId");

                entity.Property(e => e.IntBankId).HasColumnName("intBankId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.NumInterestRate)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numInterestRate");

                entity.Property(e => e.StrAccountName)
                    .HasMaxLength(10)
                    .HasColumnName("strAccountName")
                    .IsFixedLength();

                entity.Property(e => e.StrAccountNumber)
                    .HasMaxLength(250)
                    .HasColumnName("strAccountNumber");

                entity.Property(e => e.StrBankBranchName)
                    .HasMaxLength(250)
                    .HasColumnName("strBankBranchName");

                entity.Property(e => e.StrBankName)
                    .HasMaxLength(250)
                    .HasColumnName("strBankName");

                entity.Property(e => e.StrInvestmentCode)
                    .HasMaxLength(50)
                    .HasColumnName("strInvestmentCode");

                entity.Property(e => e.StrInvestmentReffNo)
                    .HasMaxLength(250)
                    .HasColumnName("strInvestmentReffNo");

                entity.Property(e => e.StrRoutingNo)
                    .HasMaxLength(250)
                    .HasColumnName("strRoutingNo");
            });

            modelBuilder.Entity<EmpPfInvestmentRow>(entity =>
            {
                entity.HasKey(e => e.IntRowId);

                entity.ToTable("empPfInvestmentRow", "saas");

                entity.Property(e => e.IntRowId).HasColumnName("intRowId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntInvenstmentHeaderId).HasColumnName("intInvenstmentHeaderId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.NumEmployeeContribution)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numEmployeeContribution");

                entity.Property(e => e.NumEmployerContribution)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numEmployerContribution");

                entity.Property(e => e.NumTotalAmount)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numTotalAmount");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrServiceLength)
                    .HasMaxLength(250)
                    .HasColumnName("strServiceLength");
            });

            modelBuilder.Entity<EmpPfngratuity>(entity =>
            {
                entity.HasKey(e => e.IntPfngratuityId);

                entity.ToTable("empPFNGratuity", "saas");

                entity.Property(e => e.IntPfngratuityId).HasColumnName("intPFNGratuityId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntNumOfEligibleMonthForPfinvestment).HasColumnName("intNumOfEligibleMonthForPFInvestment");

                entity.Property(e => e.IntNumOfEligibleYearForBenifit).HasColumnName("intNumOfEligibleYearForBenifit");

                entity.Property(e => e.IntNumOfEligibleYearForGratuity).HasColumnName("intNumOfEligibleYearForGratuity");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsEmployeeBased).HasColumnName("isEmployeeBased");

                entity.Property(e => e.IsHasGratuityPolicy).HasColumnName("isHasGratuityPolicy");

                entity.Property(e => e.IsHasPfpolicy).HasColumnName("isHasPFPolicy");

                entity.Property(e => e.NumEmployeeContributionOfBasic)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numEmployeeContributionOfBasic");

                entity.Property(e => e.NumEmployerContributionOfBasic)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numEmployerContributionOfBasic");
            });

            modelBuilder.Entity<EmpPfwithdraw>(entity =>
            {
                entity.HasKey(e => e.IntPfwithdrawId);

                entity.ToTable("empPFWithdraw", "saas");

                entity.Property(e => e.IntPfwithdrawId).HasColumnName("intPFWithdrawId");

                entity.Property(e => e.DteApplicationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteApplicationDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.NumWithdrawAmount)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("numWithdrawAmount");

                entity.Property(e => e.StrEmployee)
                    .HasMaxLength(500)
                    .HasColumnName("strEmployee");

                entity.Property(e => e.StrReason)
                    .HasMaxLength(1000)
                    .HasColumnName("strReason");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<EmpSalaryCertificateRequest>(entity =>
            {
                entity.HasKey(e => e.IntSalaryCertificateRequestId);

                entity.ToTable("EmpSalaryCertificateRequest", "saas");

                entity.Property(e => e.IntSalaryCertificateRequestId).HasColumnName("intSalaryCertificateRequestId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPayRollMonth).HasColumnName("intPayRollMonth");

                entity.Property(e => e.IntPayRollYear).HasColumnName("intPayRollYear");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrEmployeName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strEmployeName");

                entity.Property(e => e.StrStatus)
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<EmpSeparationType>(entity =>
            {
                entity.HasKey(e => e.IntSeparationTypeId)
                    .HasName("PK__empSepar__A1EBA34A52B64103");

                entity.ToTable("empSeparationType", "saas");

                entity.Property(e => e.IntSeparationTypeId).HasColumnName("intSeparationTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsEmployeeView).HasColumnName("isEmployeeView");

                entity.Property(e => e.StrSeparationType)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strSeparationType");
            });

            modelBuilder.Entity<EmpSocialMedium>(entity =>
            {
                entity.HasKey(e => e.IntSocialMediaId)
                    .HasName("PK__empSocia__80ACD49252291ED5");

                entity.ToTable("empSocialMedia", "saas");

                entity.Property(e => e.IntSocialMediaId).HasColumnName("intSocialMediaId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeBasicInfoId).HasColumnName("intEmployeeBasicInfoId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrSocialMedialLink)
                    .HasMaxLength(500)
                    .HasColumnName("strSocialMedialLink");
            });

            modelBuilder.Entity<EmpTax>(entity =>
            {
                entity.HasKey(e => e.IntTaxId);

                entity.ToTable("empTax", "saas");

                entity.Property(e => e.IntTaxId).HasColumnName("intTaxId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NumTaxAmount)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numTaxAmount");
            });

            modelBuilder.Entity<EmpTransferNpromotion>(entity =>
            {
                entity.HasKey(e => e.IntTransferNpromotionId);

                entity.ToTable("empTransferNPromotion", "saas");

                entity.Property(e => e.IntTransferNpromotionId).HasColumnName("intTransferNPromotionId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteEffectiveDate)
                    .HasColumnType("date")
                    .HasColumnName("dteEffectiveDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteReleaseDate)
                    .HasColumnType("date")
                    .HasColumnName("dteReleaseDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAreaId).HasColumnName("intAreaId");

                entity.Property(e => e.IntAreaIdFrom).HasColumnName("intAreaIdFrom");

                entity.Property(e => e.IntAttachementId).HasColumnName("intAttachementId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntBusinessUnitIdFrom).HasColumnName("intBusinessUnitIdFrom");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.IntDepartmentIdFrom).HasColumnName("intDepartmentIdFrom");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.IntDesignationIdFrom).HasColumnName("intDesignationIdFrom");

                entity.Property(e => e.IntDottedSupervisorId).HasColumnName("intDottedSupervisorId");

                entity.Property(e => e.IntDottedSupervisorIdFrom).HasColumnName("intDottedSupervisorIdFrom");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntLineManagerId).HasColumnName("intLineManagerId");

                entity.Property(e => e.IntLineManagerIdFrom).HasColumnName("intLineManagerIdFrom");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRegionId).HasColumnName("intRegionId");

                entity.Property(e => e.IntRegionIdFrom).HasColumnName("intRegionIdFrom");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntSoldDepoId).HasColumnName("intSoldDepoId");

                entity.Property(e => e.IntSoldDepoIdFrom).HasColumnName("intSoldDepoIdFrom");

                entity.Property(e => e.IntSubstitutionEmployeeId).HasColumnName("intSubstitutionEmployeeId");

                entity.Property(e => e.IntSupervisorId).HasColumnName("intSupervisorId");

                entity.Property(e => e.IntSupervisorIdFrom).HasColumnName("intSupervisorIdFrom");

                entity.Property(e => e.IntTerritoryId).HasColumnName("intTerritoryId");

                entity.Property(e => e.IntTerritoryIdFrom).HasColumnName("intTerritoryIdFrom");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWingId).HasColumnName("intWingId");

                entity.Property(e => e.IntWingIdFrom).HasColumnName("intWingIdFrom");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IntWorkplaceGroupIdFrom).HasColumnName("intWorkplaceGroupIdFrom");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IntWorkplaceIdFrom).HasColumnName("intWorkplaceIdFrom");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsJoined).HasColumnName("isJoined");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrEmployeeName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrRemarks).HasColumnName("strRemarks");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");

                entity.Property(e => e.StrTransferNpromotionType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strTransferNPromotionType");
            });

            modelBuilder.Entity<EmpTransferNpromotionRoleExtension>(entity =>
            {
                entity.HasKey(e => e.IntRoleExtensionRowId);

                entity.ToTable("empTransferNPromotionRoleExtension", "saas");

                entity.Property(e => e.IntRoleExtensionRowId).HasColumnName("intRoleExtensionRowId");

                entity.Property(e => e.DteCreatedDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedDateTime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntOrganizationReffId).HasColumnName("intOrganizationReffId");

                entity.Property(e => e.IntOrganizationTypeId).HasColumnName("intOrganizationTypeId");

                entity.Property(e => e.IntTransferNpromotionId).HasColumnName("intTransferNPromotionId");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrOrganizationReffName)
                    .HasMaxLength(250)
                    .HasColumnName("strOrganizationReffName");

                entity.Property(e => e.StrOrganizationTypeName)
                    .HasMaxLength(250)
                    .HasColumnName("strOrganizationTypeName");
            });

            modelBuilder.Entity<EmpTransferNpromotionUserRole>(entity =>
            {
                entity.HasKey(e => e.IntTransferNpromotionUserRoleId);

                entity.ToTable("empTransferNPromotionUserRole", "saas");

                entity.Property(e => e.IntTransferNpromotionUserRoleId).HasColumnName("intTransferNPromotionUserRoleId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntTransferNpromotionId).HasColumnName("intTransferNPromotionId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntUserRoleId).HasColumnName("intUserRoleId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrUserRoleName)
                    .HasMaxLength(250)
                    .HasColumnName("strUserRoleName");
            });

            modelBuilder.Entity<EmpWorklineConfig>(entity =>
            {
                entity.HasKey(e => e.IntWorklineId);

                entity.ToTable("empWorklineConfig", "saas");

                entity.Property(e => e.IntWorklineId).HasColumnName("intWorklineId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdateAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntNotifyInDays).HasColumnName("intNotifyInDays");

                entity.Property(e => e.IntServiceLengthInDays).HasColumnName("intServiceLengthInDays");

                entity.Property(e => e.IntUpdateBy).HasColumnName("intUpdateBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrEmploymentType)
                    .HasMaxLength(100)
                    .HasColumnName("strEmploymentType");
            });

            modelBuilder.Entity<EmployeeBulkUpload>(entity =>
            {
                entity.HasKey(e => e.IntEmpBulkUploadId);

                entity.ToTable("EmployeeBulkUpload");

                entity.Property(e => e.IntEmpBulkUploadId).HasColumnName("intEmpBulkUploadId");

                entity.Property(e => e.DteContactFromDate)
                    .HasColumnType("date")
                    .HasColumnName("dteContactFromDate");

                entity.Property(e => e.DteContactToDate)
                    .HasColumnType("date")
                    .HasColumnName("dteContactToDate");

                entity.Property(e => e.DteCreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateAt");

                entity.Property(e => e.DteDateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("dteDateOfBirth");

                entity.Property(e => e.DteInternCloseDate)
                    .HasColumnType("date")
                    .HasColumnName("dteInternCloseDate");

                entity.Property(e => e.DteJoiningDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteJoiningDate");

                entity.Property(e => e.DteProbationaryCloseDate)
                    .HasColumnType("date")
                    .HasColumnName("dteProbationaryCloseDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreateBy).HasColumnName("intCreateBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntSlid).HasColumnName("intSLId");

                entity.Property(e => e.IntUrlId).HasColumnName("intUrlId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsProcess).HasColumnName("isProcess");

                entity.Property(e => e.IsSalaryHold).HasColumnName("isSalaryHold");

                entity.Property(e => e.StrAreaName)
                    .HasMaxLength(250)
                    .HasColumnName("strAreaName");

                entity.Property(e => e.StrBusinessUnit)
                    .HasMaxLength(300)
                    .HasColumnName("strBusinessUnit");

                entity.Property(e => e.StrCardNumber)
                    .HasMaxLength(300)
                    .HasColumnName("strCardNumber");

                entity.Property(e => e.StrDepartment)
                    .HasMaxLength(300)
                    .HasColumnName("strDepartment");

                entity.Property(e => e.StrDesignation)
                    .HasMaxLength(300)
                    .HasColumnName("strDesignation");

                entity.Property(e => e.StrDisplayName)
                    .HasMaxLength(300)
                    .HasColumnName("strDisplayName");

                entity.Property(e => e.StrDottedSupervisorCode)
                    .HasMaxLength(300)
                    .HasColumnName("strDottedSupervisorCode");

                entity.Property(e => e.StrEmailAddress)
                    .HasMaxLength(300)
                    .HasColumnName("strEmailAddress");

                entity.Property(e => e.StrEmployeeCode)
                    .HasMaxLength(300)
                    .HasColumnName("strEmployeeCode");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(300)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrEmploymentType)
                    .HasMaxLength(300)
                    .HasColumnName("strEmploymentType");

                entity.Property(e => e.StrGender)
                    .HasMaxLength(50)
                    .HasColumnName("strGender");

                entity.Property(e => e.StrHrPosition)
                    .HasMaxLength(100)
                    .HasColumnName("strHrPosition");

                entity.Property(e => e.StrLineManagerCode)
                    .HasMaxLength(300)
                    .HasColumnName("strLineManagerCode");

                entity.Property(e => e.StrLoginId)
                    .HasMaxLength(300)
                    .HasColumnName("strLoginId");

                entity.Property(e => e.StrPassword)
                    .HasMaxLength(300)
                    .HasColumnName("strPassword");

                entity.Property(e => e.StrPhoneNumber)
                    .HasMaxLength(50)
                    .HasColumnName("strPhoneNumber");

                entity.Property(e => e.StrRegionName)
                    .HasMaxLength(250)
                    .HasColumnName("strRegionName");

                entity.Property(e => e.StrReligionName)
                    .HasMaxLength(300)
                    .HasColumnName("strReligionName");

                entity.Property(e => e.StrSoleDepoName)
                    .HasMaxLength(250)
                    .HasColumnName("strSoleDepoName");

                entity.Property(e => e.StrSupervisorCode)
                    .HasMaxLength(300)
                    .HasColumnName("strSupervisorCode");

                entity.Property(e => e.StrTerritoryName)
                    .HasMaxLength(250)
                    .HasColumnName("strTerritoryName");

                entity.Property(e => e.StrUserType)
                    .HasMaxLength(300)
                    .HasColumnName("strUserType");

                entity.Property(e => e.StrWingName)
                    .HasMaxLength(250)
                    .HasColumnName("strWingName");

                entity.Property(e => e.StrWorkplace)
                    .HasMaxLength(300)
                    .HasColumnName("strWorkplace");

                entity.Property(e => e.StrWorkplaceGroup)
                    .HasMaxLength(300)
                    .HasColumnName("strWorkplaceGroup");
            });

            modelBuilder.Entity<ExternalTraining>(entity =>
            {
                entity.HasKey(e => e.IntExternalTrainingId);

                entity.ToTable("ExternalTraining", "saas");

                entity.Property(e => e.IntExternalTrainingId).HasColumnName("intExternalTrainingId");

                entity.Property(e => e.DteActionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteActionDate");

                entity.Property(e => e.DteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteDate");

                entity.Property(e => e.DteUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntBatchSize).HasColumnName("intBatchSize");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.IntPresentParticipant).HasColumnName("intPresentParticipant");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrDepartmentName)
                    .HasMaxLength(100)
                    .HasColumnName("strDepartmentName");

                entity.Property(e => e.StrOrganizationCategory)
                    .HasMaxLength(100)
                    .HasColumnName("strOrganizationCategory");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(250)
                    .HasColumnName("strRemarks");

                entity.Property(e => e.StrResourcePersonName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strResourcePersonName");

                entity.Property(e => e.StrTrainingName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTrainingName");
            });

            modelBuilder.Entity<FiscalYear>(entity =>
            {
                entity.HasKey(e => e.IntAutoId);

                entity.ToTable("FiscalYear", "core");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteFiscalFromDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteFiscalFromDate");

                entity.Property(e => e.DteFiscalToDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteFiscalToDate");

                entity.Property(e => e.IntCreateBy).HasColumnName("intCreateBy");

                entity.Property(e => e.IntYearId).HasColumnName("intYearId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrFiscalYear)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strFiscalYear");
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.HasKey(e => e.IntGenderId)
                    .HasName("PK__Gender__41F5E8E96D90F0B3");

                entity.ToTable("Gender", "core");

                entity.Property(e => e.IntGenderId)
                    .ValueGeneratedNever()
                    .HasColumnName("intGenderId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrGender)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strGender");

                entity.Property(e => e.StrGenderCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strGenderCode");
            });

            modelBuilder.Entity<GeneratedHashCode>(entity =>
            {
                entity.HasKey(e => e.IntHashId)
                    .HasName("PK_HashCode");

                entity.ToTable("GeneratedHashCode", "core");

                entity.Property(e => e.IntHashId).HasColumnName("intHashId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsLocked).HasColumnName("isLocked");

                entity.Property(e => e.StrHashCode)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strHashCode");
            });

            modelBuilder.Entity<GlobalBankBranch>(entity =>
            {
                entity.HasKey(e => e.IntBankBranchId)
                    .HasName("PK_BankBranch");

                entity.ToTable("globalBankBranch", "saas");

                entity.Property(e => e.IntBankBranchId).HasColumnName("intBankBranchId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBankId).HasColumnName("intBankId");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDistrictId).HasColumnName("intDistrictId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrBankBranchAddress)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strBankBranchAddress");

                entity.Property(e => e.StrBankBranchCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strBankBranchCode");

                entity.Property(e => e.StrBankBranchName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBankBranchName");

                entity.Property(e => e.StrBankCode)
                    .HasMaxLength(50)
                    .HasColumnName("strBankCode");

                entity.Property(e => e.StrBankName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strBankName");

                entity.Property(e => e.StrBankShortName)
                    .HasMaxLength(50)
                    .HasColumnName("strBankShortName");

                entity.Property(e => e.StrRoutingNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strRoutingNo");
            });

            modelBuilder.Entity<GlobalCulture>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("globalCulture", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrKey)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strKey");

                entity.Property(e => e.StrLabel)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strLabel");

                entity.Property(e => e.StrLanguage)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strLanguage");
            });

            modelBuilder.Entity<GlobalDocumentType>(entity =>
            {
                entity.HasKey(e => e.IntDocumentTypeId)
                    .HasName("PK__globalDo__16E5B148B5C3732F");

                entity.ToTable("globalDocumentType", "saas");

                entity.Property(e => e.IntDocumentTypeId).HasColumnName("intDocumentTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntOwnerType).HasColumnName("intOwnerType");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrDocumentType)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDocumentType");
            });

            modelBuilder.Entity<GlobalFileUrl>(entity =>
            {
                entity.HasKey(e => e.IntDocumentId)
                    .HasName("PK__globalFi__E2485E9A715C4BDF");

                entity.ToTable("globalFileUrl", "saas");

                entity.Property(e => e.IntDocumentId).HasColumnName("intDocumentId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDocumentTypeId).HasColumnName("intDocumentTypeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsProcess)
                    .HasColumnName("isProcess")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumFileSize)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numFileSize");

                entity.Property(e => e.StrDocumentName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDocumentName");

                entity.Property(e => e.StrFileExtension)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strFileExtension");

                entity.Property(e => e.StrFileServerId)
                    .IsRequired()
                    .HasColumnName("strFileServerId");

                entity.Property(e => e.StrRefferenceDescription)
                    .HasMaxLength(250)
                    .HasColumnName("strRefferenceDescription");

                entity.Property(e => e.StrServerLocation)
                    .HasMaxLength(250)
                    .HasColumnName("strServerLocation");

                entity.Property(e => e.StrTableReferrence)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTableReferrence");
            });

            modelBuilder.Entity<GlobalInstitute>(entity =>
            {
                entity.HasKey(e => e.IntInstituteId)
                    .HasName("PK__globalIn__21DFCBDAADD9D87C");

                entity.ToTable("globalInstitute", "saas");

                entity.Property(e => e.IntInstituteId).HasColumnName("intInstituteId");

                entity.Property(e => e.IntInstituteTypeId).HasColumnName("intInstituteTypeId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrInstituteName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strInstituteName");
            });

            modelBuilder.Entity<GlobalOrganogramTree>(entity =>
            {
                entity.HasKey(e => e.IntAutoId);

                entity.ToTable("globalOrganogramTree", "saas");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntParentId).HasColumnName("intParentId");

                entity.Property(e => e.IntPositionId).HasColumnName("intPositionId");

                entity.Property(e => e.IntSequence).HasColumnName("intSequence");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrPositionName)
                    .HasMaxLength(500)
                    .HasColumnName("strPositionName");
            });

            modelBuilder.Entity<GlobalPipelineHeader>(entity =>
            {
                entity.HasKey(e => e.IntPipelineHeaderId);

                entity.ToTable("globalPipelineHeader", "saas");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAreaId).HasColumnName("intAreaId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntRegionId).HasColumnName("intRegionId");

                entity.Property(e => e.IntSoleDepoId).HasColumnName("intSoleDepoId");

                entity.Property(e => e.IntTerritoryId).HasColumnName("intTerritoryId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWingId).HasColumnName("intWingId");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrApplicationType)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strApplicationType");

                entity.Property(e => e.StrPipelineName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPipelineName");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(2000)
                    .HasColumnName("strRemarks");
            });

            modelBuilder.Entity<GlobalPipelineRow>(entity =>
            {
                entity.HasKey(e => e.IntPipelineRowId);

                entity.ToTable("globalPipelineRow", "saas");

                entity.Property(e => e.IntPipelineRowId).HasColumnName("intPipelineRowId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntShortOrder).HasColumnName("intShortOrder");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntUserGroupHeaderId).HasColumnName("intUserGroupHeaderId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsLineManager).HasColumnName("isLineManager");

                entity.Property(e => e.IsSupervisor).HasColumnName("isSupervisor");

                entity.Property(e => e.StrStatusTitle)
                    .HasMaxLength(250)
                    .HasColumnName("strStatusTitle");
            });

            modelBuilder.Entity<GlobalUserUrl>(entity =>
            {
                entity.HasKey(e => e.IntAutoId);

                entity.ToTable("GlobalUserUrl", "auth");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.IntUrlId).HasColumnName("intUrlId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrLoginId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strLoginId");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.IntItemId);

                entity.ToTable("Item", "saas");

                entity.Property(e => e.IntItemId).HasColumnName("intItemId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntItemCategoryId).HasColumnName("intItemCategoryId");

                entity.Property(e => e.IntItemUomId).HasColumnName("intItemUomId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsAutoCode).HasColumnName("isAutoCode");

                entity.Property(e => e.StrItemCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strItemCode");

                entity.Property(e => e.StrItemName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("strItemName");
            });

            modelBuilder.Entity<ItemCategory>(entity =>
            {
                entity.HasKey(e => e.IntItemCategoryId);

                entity.ToTable("ItemCategory", "saas");

                entity.Property(e => e.IntItemCategoryId).HasColumnName("intItemCategoryId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrItemCategory)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("strItemCategory");
            });

            modelBuilder.Entity<ItemUom>(entity =>
            {
                entity.HasKey(e => e.IntItemUomId);

                entity.ToTable("ItemUom", "saas");

                entity.Property(e => e.IntItemUomId).HasColumnName("intItemUomId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrItemUom)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("strItemUom");
            });

            modelBuilder.Entity<LogPyrEmployeeSalaryDefault>(entity =>
            {
                entity.HasKey(e => e.IntLogEmployeeSalaryDefaultId);

                entity.ToTable("logPyrEmployeeSalaryDefault", "saas");

                entity.Property(e => e.IntLogEmployeeSalaryDefaultId).HasColumnName("intLogEmployeeSalaryDefaultId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteLogInsertCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteLogInsertCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEffectiveMonth).HasColumnName("intEffectiveMonth");

                entity.Property(e => e.IntEffectiveYear).HasColumnName("intEffectiveYear");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntLogCreatedByUserId).HasColumnName("intLogCreatedByUserId");

                entity.Property(e => e.IntPayrollGroupId).HasColumnName("intPayrollGroupId");

                entity.Property(e => e.IntPayscaleGradeAdditionId).HasColumnName("intPayscaleGradeAdditionId");

                entity.Property(e => e.IntPayscaleGradeId).HasColumnName("intPayscaleGradeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumBasic)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numBasic");

                entity.Property(e => e.NumCbadeduction)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numCBADeduction");

                entity.Property(e => e.NumConveyanceAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numConveyanceAllowance");

                entity.Property(e => e.NumGrossSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numGrossSalary");

                entity.Property(e => e.NumHouseAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numHouseAllowance");

                entity.Property(e => e.NumMedicalAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numMedicalAllowance");

                entity.Property(e => e.NumSpecialAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numSpecialAllowance");

                entity.Property(e => e.NumTotalSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numTotalSalary");

                entity.Property(e => e.NumWashingAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numWashingAllowance");

                entity.Property(e => e.ReqNumBasic)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumBasic");

                entity.Property(e => e.ReqNumCbadeduction)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumCBADeduction");

                entity.Property(e => e.ReqNumConveyanceAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumConveyanceAllowance");

                entity.Property(e => e.ReqNumGrossSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumGrossSalary");

                entity.Property(e => e.ReqNumHouseAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumHouseAllowance");

                entity.Property(e => e.ReqNumMedicalAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumMedicalAllowance");

                entity.Property(e => e.ReqNumSpecialAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumSpecialAllowance");

                entity.Property(e => e.ReqNumTotalSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumTotalSalary");

                entity.Property(e => e.ReqNumWashingAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumWashingAllowance");
            });

            modelBuilder.Entity<LogPyrEmployeeSalaryOther>(entity =>
            {
                entity.HasKey(e => e.IntLogEmployeeSalaryOtherId);

                entity.ToTable("logPyrEmployeeSalaryOthers", "saas");

                entity.Property(e => e.IntLogEmployeeSalaryOtherId).HasColumnName("intLogEmployeeSalaryOtherId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.DtelogCreatedByDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dtelogCreatedByDateTime");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEffectiveMonth).HasColumnName("intEffectiveMonth");

                entity.Property(e => e.IntEffectiveYear).HasColumnName("intEffectiveYear");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntLogCreatedByUserId).HasColumnName("intLogCreatedByUserId");

                entity.Property(e => e.IntPayrollElementId).HasColumnName("intPayrollElementId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.NumAmount)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numAmount");

                entity.Property(e => e.ReqNumAmount)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumAmount");

                entity.Property(e => e.StrPayrollElementTypeCode)
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollElementTypeCode");
            });

            modelBuilder.Entity<LogTimeAttendanceHistory>(entity =>
            {
                entity.HasKey(e => e.IntAutoId)
                    .HasName("PK_timeAttendanceHistory");

                entity.ToTable("LogTimeAttendanceHistory", "saas");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.DteAttendanceDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAttendanceDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteEndTime).HasColumnName("dteEndTime");

                entity.Property(e => e.DteExtendedStartTime).HasColumnName("dteExtendedStartTime");

                entity.Property(e => e.DteLastStartTime).HasColumnName("dteLastStartTime");

                entity.Property(e => e.DteNextChangeDate)
                    .HasColumnType("date")
                    .HasColumnName("dteNextChangeDate");

                entity.Property(e => e.DteOfficeClosingTime)
                    .HasColumnType("time(0)")
                    .HasColumnName("dteOfficeClosingTime");

                entity.Property(e => e.DteOfficeOpeningTime)
                    .HasColumnType("time(0)")
                    .HasColumnName("dteOfficeOpeningTime");

                entity.Property(e => e.DteStartTime).HasColumnName("dteStartTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCalendarId).HasColumnName("intCalendarId");

                entity.Property(e => e.IntCalendarTypeId).HasColumnName("intCalendarTypeId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDayId).HasColumnName("intDayId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntMonthId).HasColumnName("intMonthId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntYear).HasColumnName("intYear");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsNightShift).HasColumnName("isNightShift");

                entity.Property(e => e.NumMinWorkHour)
                    .HasColumnType("numeric(4, 0)")
                    .HasColumnName("numMinWorkHour");

                entity.Property(e => e.StrCalendarName)
                    .HasMaxLength(250)
                    .HasColumnName("strCalendarName");

                entity.Property(e => e.StrCalendarType)
                    .HasMaxLength(250)
                    .HasColumnName("strCalendarType");
            });

            modelBuilder.Entity<LveLeaveApplication>(entity =>
            {
                entity.HasKey(e => e.IntApplicationId)
                    .HasName("PK__lveLeave__388D1EC3B5321904");

                entity.ToTable("lveLeaveApplication", "saas");

                entity.Property(e => e.IntApplicationId).HasColumnName("intApplicationId");

                entity.Property(e => e.DteApplicationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteApplicationDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteFromDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteFromDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteToDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteToDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntDocumentFileId).HasColumnName("intDocumentFileId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntLeaveTypeId).HasColumnName("intLeaveTypeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPaid).HasColumnName("isPaid");

                entity.Property(e => e.IsPayable).HasColumnName("isPayable");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrAddressDuetoLeave)
                    .HasMaxLength(500)
                    .HasColumnName("strAddressDuetoLeave");

                entity.Property(e => e.StrReason)
                    .HasMaxLength(3000)
                    .HasColumnName("strReason");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<LveLeaveBalance>(entity =>
            {
                entity.HasKey(e => e.IntLeaveBalanceId)
                    .HasName("PK__lveLeave__86ACFAB464CEED21");

                entity.ToTable("lveLeaveBalance", "saas");

                entity.Property(e => e.IntLeaveBalanceId).HasColumnName("intLeaveBalanceId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntBalanceDays).HasColumnName("intBalanceDays");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntLeaveTakenDays).HasColumnName("intLeaveTakenDays");

                entity.Property(e => e.IntLeaveTypeId).HasColumnName("intLeaveTypeId");

                entity.Property(e => e.IntRemainingDays).HasColumnName("intRemainingDays");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntYear).HasColumnName("intYear");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsAutoGenerate).HasColumnName("isAutoGenerate");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(2000)
                    .HasColumnName("strRemarks");
            });

            modelBuilder.Entity<LveLeaveType>(entity =>
            {
                entity.HasKey(e => e.IntLeaveTypeId);

                entity.ToTable("lveLeaveType", "saas");

                entity.Property(e => e.IntLeaveTypeId).HasColumnName("intLeaveTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntParentId).HasColumnName("intParentId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrLeaveType)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strLeaveType");

                entity.Property(e => e.StrLeaveTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strLeaveTypeCode");
            });

            modelBuilder.Entity<LveMovementApplication>(entity =>
            {
                entity.HasKey(e => e.IntApplicationId)
                    .HasName("PK__lveMovem__388D1EC3C6B5673E");

                entity.ToTable("lveMovementApplication", "saas");

                entity.Property(e => e.IntApplicationId).HasColumnName("intApplicationId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteFromDate)
                    .HasColumnType("date")
                    .HasColumnName("dteFromDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteToDate)
                    .HasColumnType("date")
                    .HasColumnName("dteToDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntMovementTypeId).HasColumnName("intMovementTypeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrLocation)
                    .HasMaxLength(250)
                    .HasColumnName("strLocation");

                entity.Property(e => e.StrReason)
                    .HasMaxLength(2000)
                    .HasColumnName("strReason");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");

                entity.Property(e => e.TmeFromTime)
                    .HasColumnType("time(0)")
                    .HasColumnName("tmeFromTime");

                entity.Property(e => e.TmeToTime)
                    .HasColumnType("time(0)")
                    .HasColumnName("tmeToTime");
            });

            modelBuilder.Entity<LveMovementType>(entity =>
            {
                entity.HasKey(e => e.IntMovementTypeId);

                entity.ToTable("lveMovementType", "saas");

                entity.Property(e => e.IntMovementTypeId).HasColumnName("intMovementTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntQuotaFrequency).HasColumnName("intQuotaFrequency");

                entity.Property(e => e.IntQuotaHour).HasColumnName("intQuotaHour");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrMovementType)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strMovementType");

                entity.Property(e => e.StrMovementTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strMovementTypeCode");
            });

            modelBuilder.Entity<ManagementDashboardPermission>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("ManagementDashboardPermission", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessId).HasColumnName("intBusinessId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");
            });

            modelBuilder.Entity<MasterAnnouncementType>(entity =>
            {
                entity.HasKey(e => e.IntAnnouncementTypeId);

                entity.ToTable("masterAnnouncementType", "saas");

                entity.Property(e => e.IntAnnouncementTypeId).HasColumnName("intAnnouncementTypeId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrAnnouncementTypeName)
                    .HasMaxLength(250)
                    .HasColumnName("strAnnouncementTypeName");
            });

            modelBuilder.Entity<MasterBusinessUnit>(entity =>
            {
                entity.HasKey(e => e.IntBusinessUnitId)
                    .HasName("PK__masterBu__9AC20E27847D85A3");

                entity.ToTable("masterBusinessUnit", "saas");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDistrictId).HasColumnName("intDistrictId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrAddress)
                    .HasMaxLength(250)
                    .HasColumnName("strAddress");

                entity.Property(e => e.StrBusinessUnit)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBusinessUnit");

                entity.Property(e => e.StrCurrency)
                    .HasMaxLength(50)
                    .HasColumnName("strCurrency");

                entity.Property(e => e.StrDistrict)
                    .HasMaxLength(250)
                    .HasColumnName("strDistrict");

                entity.Property(e => e.StrEmail)
                    .HasMaxLength(250)
                    .HasColumnName("strEmail");

                entity.Property(e => e.StrLogoUrlId).HasColumnName("strLogoUrlId");

                entity.Property(e => e.StrShortCode)
                    .HasMaxLength(50)
                    .HasColumnName("strShortCode");

                entity.Property(e => e.StrWebsiteUrl)
                    .HasMaxLength(500)
                    .HasColumnName("strWebsiteUrl");
            });

            modelBuilder.Entity<MasterDashboardComponent>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("masterDashboardComponent", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrDisplayName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDisplayName");

                entity.Property(e => e.StrHashCode)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strHashCode");

                entity.Property(e => e.StrName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strName");
            });

            modelBuilder.Entity<MasterDashboardComponentPermission>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("masterDashboardComponentPermission", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDashboardComponentId).HasColumnName("intDashboardComponentId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrAccountName)
                    .HasMaxLength(100)
                    .HasColumnName("strAccountName");

                entity.Property(e => e.StrHashCode)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strHashCode");
            });

            modelBuilder.Entity<MasterDepartment>(entity =>
            {
                entity.HasKey(e => e.IntDepartmentId)
                    .HasName("PK__empDepar__B39CCAA56E4B5E80");

                entity.ToTable("masterDepartment", "saas");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntParentDepId).HasColumnName("intParentDepId");

                entity.Property(e => e.IntRankingId).HasColumnName("intRankingId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.StrDepartment)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDepartment");

                entity.Property(e => e.StrDepartmentCode)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDepartmentCode");

                entity.Property(e => e.StrParentDepName)
                    .HasMaxLength(250)
                    .HasColumnName("strParentDepName");
            });

            modelBuilder.Entity<MasterDesignation>(entity =>
            {
                entity.HasKey(e => e.IntDesignationId)
                    .HasName("PK__empDesig__0431712FA4B57F56");

                entity.ToTable("masterDesignation", "saas");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntPayscaleGradeId).HasColumnName("intPayscaleGradeId");

                entity.Property(e => e.IntPositionId).HasColumnName("intPositionId");

                entity.Property(e => e.IntRankingId).HasColumnName("intRankingId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.StrDesignation)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDesignation");

                entity.Property(e => e.StrDesignationCode)
                    .HasMaxLength(50)
                    .HasColumnName("strDesignationCode");
            });

            modelBuilder.Entity<MasterEmploymentType>(entity =>
            {
                entity.HasKey(e => e.IntEmploymentTypeId)
                    .HasName("PK__empEmplo__EAD8479E54707BF5");

                entity.ToTable("masterEmploymentType", "saas");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntParentId).HasColumnName("intParentId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrEmploymentType)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmploymentType");
            });

            modelBuilder.Entity<MasterLocationRegister>(entity =>
            {
                entity.HasKey(e => e.IntMasterLocationId);

                entity.ToTable("masterLocationRegister", "saas");

                entity.Property(e => e.IntMasterLocationId).HasColumnName("intMasterLocationId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessId).HasColumnName("intBusinessId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrAddress)
                    .HasMaxLength(500)
                    .HasColumnName("strAddress");

                entity.Property(e => e.StrLatitude)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strLatitude");

                entity.Property(e => e.StrLocationCode)
                    .HasMaxLength(50)
                    .HasColumnName("strLocationCode");

                entity.Property(e => e.StrLongitude)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strLongitude");

                entity.Property(e => e.StrPlaceName)
                    .HasMaxLength(500)
                    .HasColumnName("strPlaceName");

                entity.Property(e => e.StrStatus)
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<MasterPosition>(entity =>
            {
                entity.HasKey(e => e.IntPositionId)
                    .HasName("PK__masterPo__902C6A2267AD1845");

                entity.ToTable("masterPosition", "saas");

                entity.Property(e => e.IntPositionId).HasColumnName("intPositionId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrPosition)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPosition");

                entity.Property(e => e.StrPositionCode)
                    .HasMaxLength(250)
                    .HasColumnName("strPositionCode");
            });

            modelBuilder.Entity<MasterTaxChallanConfig>(entity =>
            {
                entity.HasKey(e => e.IntTaxChallanConfigId);

                entity.ToTable("masterTaxChallanConfig", "saas");

                entity.Property(e => e.IntTaxChallanConfigId).HasColumnName("intTaxChallanConfigId");

                entity.Property(e => e.DteChallanDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteChallanDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteFiscalFromDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteFiscalFromDate");

                entity.Property(e => e.DteFiscalToDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteFiscalToDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntBankId).HasColumnName("intBankId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntFiscalYearId).HasColumnName("intFiscalYearId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntYear).HasColumnName("intYear");

                entity.Property(e => e.StrBankName)
                    .HasMaxLength(250)
                    .HasColumnName("strBankName");

                entity.Property(e => e.StrChallanNo)
                    .HasMaxLength(250)
                    .HasColumnName("strChallanNo");

                entity.Property(e => e.StrCircle)
                    .HasMaxLength(500)
                    .HasColumnName("strCircle");

                entity.Property(e => e.StrZone)
                    .HasMaxLength(500)
                    .HasColumnName("strZone");
            });

            modelBuilder.Entity<MasterTerritoryType>(entity =>
            {
                entity.HasKey(e => e.IntTerritoryTypeId);

                entity.ToTable("masterTerritoryType", "core");

                entity.Property(e => e.IntTerritoryTypeId).HasColumnName("intTerritoryTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntHrPositionId).HasColumnName("intHrPositionId");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrTerritoryType)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strTerritoryType");
            });

            modelBuilder.Entity<MasterWorkplace>(entity =>
            {
                entity.HasKey(e => e.IntWorkplaceId)
                    .HasName("PK__masterWo__5F36F7092B859448");

                entity.ToTable("masterWorkplace", "saas");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDistrictId).HasColumnName("intDistrictId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrAddress)
                    .HasMaxLength(250)
                    .HasColumnName("strAddress");

                entity.Property(e => e.StrDistrict)
                    .HasMaxLength(250)
                    .HasColumnName("strDistrict");

                entity.Property(e => e.StrWorkplace)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strWorkplace");

                entity.Property(e => e.StrWorkplaceCode)
                    .HasMaxLength(10)
                    .HasColumnName("strWorkplaceCode");

                entity.Property(e => e.StrWorkplaceGroup)
                    .HasMaxLength(250)
                    .HasColumnName("strWorkplaceGroup");
            });

            modelBuilder.Entity<MasterWorkplaceGroup>(entity =>
            {
                entity.HasKey(e => e.IntWorkplaceGroupId)
                    .HasName("PK__masterWo__171424479239ACD6");

                entity.ToTable("masterWorkplaceGroup", "saas");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrWorkplaceGroup)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strWorkplaceGroup");

                entity.Property(e => e.StrWorkplaceGroupCode)
                    .HasMaxLength(50)
                    .HasColumnName("strWorkplaceGroupCode");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.IntMenuId);

                entity.ToTable("Menus", "auth");

                entity.Property(e => e.IntMenuId).HasColumnName("intMenuId");

                entity.Property(e => e.IntMenuLabelId).HasColumnName("intMenuLabelId");

                entity.Property(e => e.IntMenuSerial).HasColumnName("intMenuSerial");

                entity.Property(e => e.IntMenuSerialForApps).HasColumnName("intMenuSerialForApps");

                entity.Property(e => e.IntParentMenuId).HasColumnName("intParentMenuId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsExpandable).HasColumnName("isExpandable");

                entity.Property(e => e.IsForApps).HasColumnName("isForApps");

                entity.Property(e => e.IsForCommon).HasColumnName("isForCommon");

                entity.Property(e => e.IsForWeb).HasColumnName("isForWeb");

                entity.Property(e => e.IsHasApproval).HasColumnName("isHasApproval");

                entity.Property(e => e.IsMenuForView).HasColumnName("isMenuForView");

                entity.Property(e => e.StrHashCode)
                    .HasMaxLength(500)
                    .HasColumnName("strHashCode");

                entity.Property(e => e.StrIcon)
                    .HasMaxLength(250)
                    .HasColumnName("strIcon");

                entity.Property(e => e.StrIconForApps)
                    .HasMaxLength(250)
                    .HasColumnName("strIconForApps");

                entity.Property(e => e.StrMenuName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strMenuName");

                entity.Property(e => e.StrMenuNameApps)
                    .HasMaxLength(250)
                    .HasColumnName("strMenuNameApps");

                entity.Property(e => e.StrMenuNameWeb)
                    .HasMaxLength(250)
                    .HasColumnName("strMenuNameWeb");

                entity.Property(e => e.StrTo)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTo")
                    .HasDefaultValueSql("('v')");

                entity.Property(e => e.StrToForApps)
                    .HasMaxLength(500)
                    .HasColumnName("strToForApps");
            });

            modelBuilder.Entity<MenuPermission>(entity =>
            {
                entity.HasKey(e => e.IntMenuPermissionId);

                entity.ToTable("MenuPermission", "auth");

                entity.Property(e => e.IntMenuPermissionId).HasColumnName("intMenuPermissionId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeOrRoleId).HasColumnName("intEmployeeOrRoleId");

                entity.Property(e => e.IntMenuId).HasColumnName("intMenuId");

                entity.Property(e => e.IntModuleId).HasColumnName("intModuleId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsCreate)
                    .HasColumnName("isCreate")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsEdit)
                    .HasColumnName("isEdit")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsForApps).HasColumnName("isForApps");

                entity.Property(e => e.IsForWeb).HasColumnName("isForWeb");

                entity.Property(e => e.IsView)
                    .HasColumnName("isView")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrIsFor)
                    .HasMaxLength(250)
                    .HasColumnName("strIsFor");

                entity.Property(e => e.StrMenuName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strMenuName");

                entity.Property(e => e.StrModuleName)
                    .HasMaxLength(250)
                    .HasColumnName("strModuleName");
            });

            modelBuilder.Entity<NotificationCategoriesType>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("NotificationCategoriesType", "signalR");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrTypeName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strTypeName");
            });

            modelBuilder.Entity<NotificationCategory>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("NotificationCategories", "signalR");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrCategoriesName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strCategoriesName");

                entity.Property(e => e.StrGenericMessageText).HasColumnName("strGenericMessageText");

                entity.Property(e => e.StrImageUrl).HasColumnName("strImageUrl");
            });

            modelBuilder.Entity<NotificationDetail>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("NotificationDetails", "signalR");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntMasterId).HasColumnName("intMasterId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrCreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("strCreatedBy");

                entity.Property(e => e.StrReceiver)
                    .HasMaxLength(150)
                    .HasColumnName("strReceiver");
            });

            modelBuilder.Entity<NotificationMaster>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("NotificationMaster", "signalR");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntFeatureTableAutoId).HasColumnName("intFeatureTableAutoId");

                entity.Property(e => e.IntModuleId).HasColumnName("intModuleId");

                entity.Property(e => e.IntOrgId).HasColumnName("intOrgId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsCommon).HasColumnName("isCommon");

                entity.Property(e => e.IsSeen).HasColumnName("isSeen");

                entity.Property(e => e.StrCreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("strCreatedBy");

                entity.Property(e => e.StrFeature)
                    .HasMaxLength(500)
                    .HasColumnName("strFeature");

                entity.Property(e => e.StrLoginId).HasColumnName("strLoginId");

                entity.Property(e => e.StrModule)
                    .HasMaxLength(500)
                    .HasColumnName("strModule");

                entity.Property(e => e.StrNotifyDetails).HasColumnName("strNotifyDetails");

                entity.Property(e => e.StrNotifyTitle)
                    .HasMaxLength(500)
                    .HasColumnName("strNotifyTitle");

                entity.Property(e => e.StrReceiver)
                    .HasMaxLength(150)
                    .HasColumnName("strReceiver");
            });

            modelBuilder.Entity<NotificationPermissionDetail>(entity =>
            {
                entity.HasKey(e => e.IntPermissionDetailsId);

                entity.ToTable("NotificationPermissionDetails", "signalR");

                entity.Property(e => e.IntPermissionDetailsId).HasColumnName("intPermissionDetailsID");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntNcategoryTypeId).HasColumnName("intNCategoryTypeId");

                entity.Property(e => e.IntPermissionId).HasColumnName("intPermissionId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.SteNcategoryTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("steNCategoryTypeName");
            });

            modelBuilder.Entity<NotificationPermissionMaster>(entity =>
            {
                entity.HasKey(e => e.IntPermissionId);

                entity.ToTable("NotificationPermissionMaster", "signalR");

                entity.Property(e => e.IntPermissionId).HasColumnName("intPermissionId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntNcategoryId).HasColumnName("intNCategoryId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrNcategoryName)
                    .HasMaxLength(100)
                    .HasColumnName("strNCategoryName");
            });

            modelBuilder.Entity<NotifySendFailedLog>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("NotifySendFailedLog", "signalR");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntFeatureTableAutoId).HasColumnName("intFeatureTableAutoId");

                entity.Property(e => e.StrFeature)
                    .HasMaxLength(50)
                    .HasColumnName("strFeature");
            });

            modelBuilder.Entity<OrganizationType>(entity =>
            {
                entity.HasKey(e => e.IntOrganizationTypeId);

                entity.ToTable("OrganizationType", "auth");

                entity.Property(e => e.IntOrganizationTypeId).HasColumnName("intOrganizationTypeId");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrOrganizationTypeName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strOrganizationTypeName");
            });

            modelBuilder.Entity<OverTimeConfiguration>(entity =>
            {
                entity.HasKey(e => e.IntOtconfigId);

                entity.ToTable("OverTimeConfiguration", "saas");

                entity.Property(e => e.IntOtconfigId).HasColumnName("intOTConfigId");

                entity.Property(e => e.DteCreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateAt");

                entity.Property(e => e.DteUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntMaxOverTimeDaily).HasColumnName("intMaxOverTimeDaily");

                entity.Property(e => e.IntMaxOverTimeMonthly).HasColumnName("intMaxOverTimeMonthly");

                entity.Property(e => e.IntOtAmountShouldBe).HasColumnName("intOtAmountShouldBe");

                entity.Property(e => e.IntOtbenefitsHour).HasColumnName("intOTBenefitsHour");

                entity.Property(e => e.IntOtcalculationShouldBe).HasColumnName("intOTCalculationShouldBe");

                entity.Property(e => e.IntOtdependOn).HasColumnName("intOTDependOn");

                entity.Property(e => e.IntOverTimeCountFrom).HasColumnName("intOverTimeCountFrom");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.NumFixedAmount)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numFixedAmount");
            });

            modelBuilder.Entity<OverTimeConfigurationDetail>(entity =>
            {
                entity.HasKey(e => e.IntOtautoId);

                entity.ToTable("OverTimeConfigurationDetails", "saas");

                entity.Property(e => e.IntOtautoId).HasColumnName("intOTAutoId");

                entity.Property(e => e.DteCreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateAt");

                entity.Property(e => e.DteUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedDate");

                entity.Property(e => e.IntAmountPercentage).HasColumnName("intAmountPercentage");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntMasterId).HasColumnName("intMasterId");

                entity.Property(e => e.IntUpdatedBy)
                    .HasMaxLength(10)
                    .HasColumnName("intUpdatedBy")
                    .IsFixedLength();

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.NumFromMinute)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numFromMinute");

                entity.Property(e => e.NumToMinute)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numToMinute");
            });

            modelBuilder.Entity<PipelineTypeDdl>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("PipelineTypeDDL", "core");

                entity.HasComment("PipelineTypeDDL");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.StrApplicationType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strApplicationType");

                entity.Property(e => e.StrDisplayName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strDisplayName");

                entity.Property(e => e.StrHashCode)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strHashCode");
            });

            modelBuilder.Entity<PolicyAcknowledge>(entity =>
            {
                entity.HasKey(e => e.IntAcknowledgeId);

                entity.ToTable("PolicyAcknowledge", "saas");

                entity.Property(e => e.IntAcknowledgeId).HasColumnName("intAcknowledgeId");

                entity.Property(e => e.DteAcknowledgeDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAcknowledgeDate");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntPolicyId).HasColumnName("intPolicyId");
            });

            modelBuilder.Entity<PolicyCategory>(entity =>
            {
                entity.HasKey(e => e.IntPolicyCategoryId);

                entity.ToTable("PolicyCategory", "saas");

                entity.Property(e => e.IntPolicyCategoryId).HasColumnName("intPolicyCategoryId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrPolicyCategoryName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPolicyCategoryName");
            });

            modelBuilder.Entity<PolicyHeader>(entity =>
            {
                entity.HasKey(e => e.IntPolicyId);

                entity.ToTable("PolicyHeader", "saas");

                entity.Property(e => e.IntPolicyId).HasColumnName("intPolicyId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntPolicyCategoryId).HasColumnName("intPolicyCategoryId");

                entity.Property(e => e.IntPolicyFileUrlId).HasColumnName("intPolicyFileUrlId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrPolicyCategoryName)
                    .HasMaxLength(250)
                    .HasColumnName("strPolicyCategoryName");

                entity.Property(e => e.StrPolicyFileName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPolicyFileName");

                entity.Property(e => e.StrPolicyTitle)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPolicyTitle");
            });

            modelBuilder.Entity<PolicyRow>(entity =>
            {
                entity.HasKey(e => e.IntRowId);

                entity.ToTable("PolicyRow", "saas");

                entity.Property(e => e.IntRowId).HasColumnName("intRowId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntAreaAutoId).HasColumnName("intAreaAutoId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntPolicyId).HasColumnName("intPolicyId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrAreaName)
                    .HasMaxLength(250)
                    .HasColumnName("strAreaName");

                entity.Property(e => e.StrAreaType)
                    .HasMaxLength(250)
                    .HasColumnName("strAreaType");
            });

            modelBuilder.Entity<PostOffice>(entity =>
            {
                entity.HasKey(e => e.IntPostOfficeId);

                entity.ToTable("PostOffice", "core");

                entity.Property(e => e.IntPostOfficeId).HasColumnName("intPostOfficeId");

                entity.Property(e => e.DteCreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IntDistrictId).HasColumnName("intDistrictId");

                entity.Property(e => e.IntDivisionId).HasColumnName("intDivisionId");

                entity.Property(e => e.IntThanaId).HasColumnName("intThanaId");

                entity.Property(e => e.StrPostCode)
                    .HasMaxLength(250)
                    .HasColumnName("strPostCode");

                entity.Property(e => e.StrPostOffice)
                    .HasMaxLength(250)
                    .HasColumnName("strPostOffice");

                entity.Property(e => e.StrPostOfficeBn)
                    .HasMaxLength(250)
                    .HasColumnName("strPostOfficeBN");

                entity.Property(e => e.StrThanaName)
                    .HasMaxLength(250)
                    .HasColumnName("strThanaName");
            });

            modelBuilder.Entity<PushNotifyDeviceRegistration>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("PushNotifyDeviceRegistration", "pushNotify");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrDeviceId)
                    .IsRequired()
                    .HasColumnName("strDeviceId");
            });

            modelBuilder.Entity<PyrArearSalaryGenerateHeader>(entity =>
            {
                entity.HasKey(e => e.IntArearSalaryGenerateHeaderId);

                entity.ToTable("pyrArearSalaryGenerateHeader", "saas");

                entity.Property(e => e.IntArearSalaryGenerateHeaderId).HasColumnName("intArearSalaryGenerateHeaderId");

                entity.Property(e => e.DteDateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("dteDateOfBirth");

                entity.Property(e => e.DteJoiningDate)
                    .HasColumnType("date")
                    .HasColumnName("dteJoiningDate");

                entity.Property(e => e.DteManualSalaryAdjustmentDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteManualSalaryAdjustmentDateTime");

                entity.Property(e => e.DtePayrollGenerateDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dtePayrollGenerateDateTime");

                entity.Property(e => e.DtePayrollGenerateFrom)
                    .HasColumnType("date")
                    .HasColumnName("dtePayrollGenerateFrom");

                entity.Property(e => e.DtePayrollGenerateTo)
                    .HasColumnType("date")
                    .HasColumnName("dtePayrollGenerateTo");

                entity.Property(e => e.DteSalaryApprovedDateTime)
                    .HasColumnType("date")
                    .HasColumnName("dteSalaryApprovedDateTime");

                entity.Property(e => e.DteSalaryGenerateFor)
                    .HasColumnType("date")
                    .HasColumnName("dteSalaryGenerateFor");

                entity.Property(e => e.DteSalaryRejectDateTime)
                    .HasColumnType("date")
                    .HasColumnName("dteSalaryRejectDateTime");

                entity.Property(e => e.IntAbsent).HasColumnName("intAbsent");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAnnualLeave).HasColumnName("intAnnualLeave");

                entity.Property(e => e.IntArearSalaryGenerateRequestId).HasColumnName("intArearSalaryGenerateRequestId");

                entity.Property(e => e.IntBankBranchId).HasColumnName("intBankBranchId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCasualLeave).HasColumnName("intCasualLeave");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.IntEarnLeave).HasColumnName("intEarnLeave");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmployeeStatusId).HasColumnName("intEmployeeStatusId");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntFinancialInstitutionId).HasColumnName("intFinancialInstitutionId");

                entity.Property(e => e.IntGradeId).HasColumnName("intGradeId");

                entity.Property(e => e.IntHoliday).HasColumnName("intHoliday");

                entity.Property(e => e.IntLate).HasColumnName("intLate");

                entity.Property(e => e.IntLwp).HasColumnName("intLWP");

                entity.Property(e => e.IntManualSalaryAdjustmentBy)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("intManualSalaryAdjustmentBy")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IntMaternityLeave).HasColumnName("intMaternityLeave");

                entity.Property(e => e.IntMonthId).HasColumnName("intMonthId");

                entity.Property(e => e.IntMovement).HasColumnName("intMovement");

                entity.Property(e => e.IntOffDay).HasColumnName("intOffDay");

                entity.Property(e => e.IntOthersLeave).HasColumnName("intOthersLeave");

                entity.Property(e => e.IntPayableDays).HasColumnName("intPayableDays");

                entity.Property(e => e.IntPayrollGroupId).HasColumnName("intPayrollGroupId");

                entity.Property(e => e.IntPayrollPeriodId).HasColumnName("intPayrollPeriodId");

                entity.Property(e => e.IntPresent).HasColumnName("intPresent");

                entity.Property(e => e.IntPrivilegeLeave).HasColumnName("intPrivilegeLeave");

                entity.Property(e => e.IntSalaryPolicyId).HasColumnName("intSalaryPolicyId");

                entity.Property(e => e.IntSickLeave).HasColumnName("intSickLeave");

                entity.Property(e => e.IntSlaveId).HasColumnName("intSlaveId");

                entity.Property(e => e.IntSpecialLeave).HasColumnName("intSpecialLeave");

                entity.Property(e => e.IntTotalWorkingDays).HasColumnName("intTotalWorkingDays");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IntYearId).HasColumnName("intYearId");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsApprove).HasColumnName("isApprove");

                entity.Property(e => e.IsPerday).HasColumnName("isPerday");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.IsTakeHomePay).HasColumnName("isTakeHomePay");

                entity.Property(e => e.NumGrossSalary)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numGrossSalary");

                entity.Property(e => e.NumLoanAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numLoanAmount");

                entity.Property(e => e.NumLowerLimit)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numLowerLimit");

                entity.Property(e => e.NumManualSalaryAddition)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numManualSalaryAddition");

                entity.Property(e => e.NumManualSalaryDeduction)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numManualSalaryDeduction");

                entity.Property(e => e.NumOverTimeAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numOverTimeAmount");

                entity.Property(e => e.NumOverTimeHour)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numOverTimeHour");

                entity.Property(e => e.NumPayableSalaryCal)
                    .HasColumnType("numeric(29, 4)")
                    .HasColumnName("numPayableSalaryCal")
                    .HasComputedColumnSql("([numPerDaySalary]*[intPayableDays])", false);

                entity.Property(e => e.NumPerDaySalary)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numPerDaySalary");

                entity.Property(e => e.NumPfamount)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numPFAmount");

                entity.Property(e => e.NumPfcompany)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numPFCompany");

                entity.Property(e => e.NumTaxAmount)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numTaxAmount");

                entity.Property(e => e.NumTotalAllowance)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("numTotalAllowance");

                entity.Property(e => e.NumTotalDeduction)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("numTotalDeduction");

                entity.Property(e => e.NumUpperLimit)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numUpperLimit");

                entity.Property(e => e.StrAccountName)
                    .HasMaxLength(300)
                    .HasColumnName("strAccountName");

                entity.Property(e => e.StrAccountNo)
                    .HasMaxLength(50)
                    .HasColumnName("strAccountNo");

                entity.Property(e => e.StrAge)
                    .HasMaxLength(300)
                    .HasColumnName("strAge");

                entity.Property(e => e.StrBankBranchName)
                    .HasMaxLength(300)
                    .HasColumnName("strBankBranchName");

                entity.Property(e => e.StrBusinessUnitName)
                    .HasMaxLength(300)
                    .HasColumnName("strBusinessUnitName");

                entity.Property(e => e.StrContactNumber)
                    .HasMaxLength(50)
                    .HasColumnName("strContactNumber");

                entity.Property(e => e.StrDepartment)
                    .HasMaxLength(300)
                    .HasColumnName("strDepartment");

                entity.Property(e => e.StrDesignation)
                    .HasMaxLength(300)
                    .HasColumnName("strDesignation");

                entity.Property(e => e.StrEmployeeCode)
                    .HasMaxLength(150)
                    .HasColumnName("strEmployeeCode");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(300)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrEmployeeStatus)
                    .HasMaxLength(150)
                    .HasColumnName("strEmployeeStatus");

                entity.Property(e => e.StrEmploymentType)
                    .HasMaxLength(150)
                    .HasColumnName("strEmploymentType");

                entity.Property(e => e.StrFinancialInstitution)
                    .HasMaxLength(300)
                    .HasColumnName("strFinancialInstitution");

                entity.Property(e => e.StrGrade)
                    .HasMaxLength(200)
                    .HasColumnName("strGrade");

                entity.Property(e => e.StrOfficialEmail)
                    .HasMaxLength(300)
                    .HasColumnName("strOfficialEmail");

                entity.Property(e => e.StrPaymentBankType)
                    .HasMaxLength(50)
                    .HasColumnName("strPaymentBankType");

                entity.Property(e => e.StrPayrollGroupName)
                    .HasMaxLength(300)
                    .HasColumnName("strPayrollGroupName");

                entity.Property(e => e.StrPayrollPeriod)
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollPeriod");

                entity.Property(e => e.StrRoutingNumber)
                    .HasMaxLength(20)
                    .HasColumnName("strRoutingNumber");

                entity.Property(e => e.StrSalaryApprovedByUser)
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryApprovedByUser");

                entity.Property(e => e.StrSalaryPolicyName)
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryPolicyName");

                entity.Property(e => e.StrSalaryRejectByUser)
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryRejectByUser");

                entity.Property(e => e.StrServiceLength)
                    .HasMaxLength(300)
                    .HasColumnName("strServiceLength");

                entity.Property(e => e.StrSlave)
                    .HasMaxLength(200)
                    .HasColumnName("strSlave");

                entity.Property(e => e.StrWorkplaceGroupName)
                    .HasMaxLength(300)
                    .HasColumnName("strWorkplaceGroupName");

                entity.Property(e => e.StrWorkplaceName)
                    .HasMaxLength(300)
                    .HasColumnName("strWorkplaceName");
            });

            modelBuilder.Entity<PyrArearSalaryGenerateRequest>(entity =>
            {
                entity.HasKey(e => e.IntArearSalaryGenerateRequestId)
                    .HasName("PK_ArearSalaryGenerateRequest");

                entity.ToTable("pyrArearSalaryGenerateRequest", "saas");

                entity.Property(e => e.IntArearSalaryGenerateRequestId).HasColumnName("intArearSalaryGenerateRequestId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteEffectiveFrom)
                    .HasColumnType("date")
                    .HasColumnName("dteEffectiveFrom");

                entity.Property(e => e.DteEffectiveTo)
                    .HasColumnType("date")
                    .HasColumnName("dteEffectiveTo");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntSalaryPolicyId).HasColumnName("intSalaryPolicyId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsGenerated).HasColumnName("isGenerated");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.NumNetPayableSalary)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numNetPayableSalary");

                entity.Property(e => e.NumPercentOfGross)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numPercentOfGross");

                entity.Property(e => e.StrArearSalaryCode)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strArearSalaryCode");

                entity.Property(e => e.StrBusinessUnit)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBusinessUnit");

                entity.Property(e => e.StrDescription)
                    .HasMaxLength(500)
                    .HasColumnName("strDescription");

                entity.Property(e => e.StrSalaryPolicyName)
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryPolicyName");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<PyrArearSalaryGenerateRequestRow>(entity =>
            {
                entity.HasKey(e => e.IntArearSalaryGenerateRequestRowId)
                    .HasName("PK_ArearSalaryGenerateRequestRow");

                entity.ToTable("pyrArearSalaryGenerateRequestRow", "saas");

                entity.Property(e => e.IntArearSalaryGenerateRequestRowId).HasColumnName("intArearSalaryGenerateRequestRowId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntArearSalaryGenerateRequestId).HasColumnName("intArearSalaryGenerateRequestId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");
            });

            modelBuilder.Entity<PyrArearSalaryGenerateRow>(entity =>
            {
                entity.HasKey(e => e.IntArearSalaryGenerateRow)
                    .HasName("PK_ArearSalaryGenerateRow");

                entity.ToTable("pyrArearSalaryGenerateRow", "saas");

                entity.Property(e => e.IntArearSalaryGenerateRow).HasColumnName("intArearSalaryGenerateRow");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntArearSalaryGenerateHeaderId).HasColumnName("intArearSalaryGenerateHeaderId");

                entity.Property(e => e.IntArearSalaryGenerateRequestId).HasColumnName("intArearSalaryGenerateRequestId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntMonthId).HasColumnName("intMonthId");

                entity.Property(e => e.IntPayrollElementId).HasColumnName("intPayrollElementId");

                entity.Property(e => e.IntPayrollElementTypeId).HasColumnName("intPayrollElementTypeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntYearId).HasColumnName("intYearId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NumAmount)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numAmount");

                entity.Property(e => e.StrPayrollElement)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollElement");

                entity.Property(e => e.StrPayrollElementCode)
                    .HasMaxLength(5)
                    .HasColumnName("strPayrollElementCode");
            });

            modelBuilder.Entity<PyrBonusGenerateHeader>(entity =>
            {
                entity.HasKey(e => e.IntBonusHeaderId);

                entity.ToTable("pyrBonusGenerateHeader", "saas");

                entity.Property(e => e.IntBonusHeaderId).HasColumnName("intBonusHeaderId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteEffectedDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteEffectedDateTime");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAreaId).HasColumnName("intAreaId");

                entity.Property(e => e.IntArrearBonusReferenceId).HasColumnName("intArrearBonusReferenceId");

                entity.Property(e => e.IntBonusId).HasColumnName("intBonusId");

                entity.Property(e => e.IntBonusMonthCal)
                    .HasColumnName("intBonusMonthCal")
                    .HasComputedColumnSql("(datepart(month,[dteEffectedDateTime]))", false);

                entity.Property(e => e.IntBonusYearCal)
                    .HasColumnName("intBonusYearCal")
                    .HasComputedColumnSql("(datepart(year,[dteEffectedDateTime]))", false);

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRegionId).HasColumnName("intRegionId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntSoleDepoId).HasColumnName("intSoleDepoId");

                entity.Property(e => e.IntTerritoryId).HasColumnName("intTerritoryId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWingId).HasColumnName("intWingId");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsArrearBonus).HasColumnName("isArrearBonus");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.IsSendForApproval).HasColumnName("isSendForApproval");

                entity.Property(e => e.NumBonusAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numBonusAmount");

                entity.Property(e => e.StrBonusName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strBonusName");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<PyrBonusGenerateRow>(entity =>
            {
                entity.HasKey(e => e.IntRowId);

                entity.ToTable("pyrBonusGenerateRow", "saas");

                entity.Property(e => e.IntRowId).HasColumnName("intRowId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteJoiningDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteJoiningDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntBankBranchId).HasColumnName("intBankBranchId");

                entity.Property(e => e.IntBankId).HasColumnName("intBankId");

                entity.Property(e => e.IntBonusHeaderId).HasColumnName("intBonusHeaderId");

                entity.Property(e => e.IntBonusId).HasColumnName("intBonusId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntPayrollGroupId).HasColumnName("intPayrollGroupId");

                entity.Property(e => e.IntReligionId).HasColumnName("intReligionId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkPlaceGroupId).HasColumnName("intWorkPlaceGroupId");

                entity.Property(e => e.IntWorkPlaceId).HasColumnName("intWorkPlaceId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NumBasic)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("numBasic");

                entity.Property(e => e.NumBonusAmount)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("numBonusAmount");

                entity.Property(e => e.NumSalary)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("numSalary");

                entity.Property(e => e.StrBankAccountNumber)
                    .HasMaxLength(20)
                    .HasColumnName("strBankAccountNumber");

                entity.Property(e => e.StrBankBranchName)
                    .HasMaxLength(200)
                    .HasColumnName("strBankBranchName");

                entity.Property(e => e.StrBankName)
                    .HasMaxLength(200)
                    .HasColumnName("strBankName");

                entity.Property(e => e.StrBonusName)
                    .HasMaxLength(200)
                    .HasColumnName("strBonusName");

                entity.Property(e => e.StrDepartmentName)
                    .HasMaxLength(250)
                    .HasColumnName("strDepartmentName");

                entity.Property(e => e.StrDesignationName)
                    .HasMaxLength(250)
                    .HasColumnName("strDesignationName");

                entity.Property(e => e.StrEmployeeCode)
                    .HasMaxLength(50)
                    .HasColumnName("strEmployeeCode");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrEmploymentTypeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmploymentTypeName");

                entity.Property(e => e.StrPayrollGroupName)
                    .HasMaxLength(100)
                    .HasColumnName("strPayrollGroupName");

                entity.Property(e => e.StrReligionName)
                    .HasMaxLength(250)
                    .HasColumnName("strReligionName");

                entity.Property(e => e.StrRoutingNumber)
                    .HasMaxLength(50)
                    .HasColumnName("strRoutingNumber");

                entity.Property(e => e.StrServiceLength)
                    .HasMaxLength(100)
                    .HasColumnName("strServiceLength");

                entity.Property(e => e.StrWorkPlaceGroupName)
                    .HasMaxLength(250)
                    .HasColumnName("strWorkPlaceGroupName");

                entity.Property(e => e.StrWorkPlaceName)
                    .HasMaxLength(250)
                    .HasColumnName("strWorkPlaceName");
            });

            modelBuilder.Entity<PyrBonusName>(entity =>
            {
                entity.HasKey(e => e.IntBonusId);

                entity.ToTable("pyrBonusName", "saas");

                entity.Property(e => e.IntBonusId).HasColumnName("intBonusId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrBonusDescription)
                    .HasMaxLength(3000)
                    .HasColumnName("strBonusDescription");

                entity.Property(e => e.StrBonusName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBonusName");
            });

            modelBuilder.Entity<PyrBonusSetup>(entity =>
            {
                entity.HasKey(e => e.IntBonusSetupId);

                entity.ToTable("pyrBonusSetup", "saas");

                entity.Property(e => e.IntBonusSetupId).HasColumnName("intBonusSetupId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteServerDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteServerDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBonusId).HasColumnName("intBonusId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntMaximumServiceLengthMonth).HasColumnName("intMaximumServiceLengthMonth");

                entity.Property(e => e.IntMinimumServiceLengthMonth).HasColumnName("intMinimumServiceLengthMonth");

                entity.Property(e => e.IntReligion).HasColumnName("intReligion");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.NumBonusPercentage)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numBonusPercentage");

                entity.Property(e => e.StrBonusDescription).HasColumnName("strBonusDescription");

                entity.Property(e => e.StrBonusName)
                    .HasMaxLength(250)
                    .HasColumnName("strBonusName");

                entity.Property(e => e.StrBonusPercentageOn)
                    .HasMaxLength(250)
                    .HasColumnName("strBonusPercentageOn");

                entity.Property(e => e.StrEmploymentType)
                    .HasMaxLength(250)
                    .HasColumnName("strEmploymentType");

                entity.Property(e => e.StrReligionName)
                    .HasMaxLength(50)
                    .HasColumnName("strReligionName");
            });

            modelBuilder.Entity<PyrEmpSalaryAdditionNdeduction>(entity =>
            {
                entity.HasKey(e => e.IntSalaryAdditionAndDeductionId);

                entity.ToTable("pyrEmpSalaryAdditionNDeduction", "saas");

                entity.Property(e => e.IntSalaryAdditionAndDeductionId).HasColumnName("intSalaryAdditionAndDeductionId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAdditionNdeductionTypeId).HasColumnName("intAdditionNDeductionTypeId");

                entity.Property(e => e.IntAmountWillBeId).HasColumnName("intAmountWillBeId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntMonth).HasColumnName("intMonth");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntToMonth).HasColumnName("intToMonth");

                entity.Property(e => e.IntToYear).HasColumnName("intToYear");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntYear).HasColumnName("intYear");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsAddition).HasColumnName("isAddition");

                entity.Property(e => e.IsAutoRenew).HasColumnName("isAutoRenew");

                entity.Property(e => e.IsOther).HasColumnName("isOther");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsProcessed).HasColumnName("isProcessed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.NumAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numAmount");

                entity.Property(e => e.StrAdditionNdeduction)
                    .HasMaxLength(50)
                    .HasColumnName("strAdditionNDeduction");

                entity.Property(e => e.StrAmountWillBe)
                    .HasMaxLength(250)
                    .HasColumnName("strAmountWillBe");

                entity.Property(e => e.StrCreatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("strCreatedBy");

                entity.Property(e => e.StrMonth)
                    .HasMaxLength(50)
                    .HasColumnName("strMonth");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");

                entity.Property(e => e.StrToMonth)
                    .HasMaxLength(50)
                    .HasColumnName("strToMonth");
            });

            modelBuilder.Entity<PyrEmployeeSalaryDefault>(entity =>
            {
                entity.HasKey(e => e.IntEmployeeSalaryDefaultId)
                    .HasName("PK__pyrEmplo__5D30061AB72E3BA9");

                entity.ToTable("pyrEmployeeSalaryDefault", "saas");

                entity.Property(e => e.IntEmployeeSalaryDefaultId).HasColumnName("intEmployeeSalaryDefaultId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEffectiveMonth).HasColumnName("intEffectiveMonth");

                entity.Property(e => e.IntEffectiveYear).HasColumnName("intEffectiveYear");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NumBasic)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numBasic");

                entity.Property(e => e.NumCbadeduction)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numCBADeduction");

                entity.Property(e => e.NumConveyanceAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numConveyanceAllowance");

                entity.Property(e => e.NumGrossSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numGrossSalary");

                entity.Property(e => e.NumHouseAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numHouseAllowance");

                entity.Property(e => e.NumMedicalAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numMedicalAllowance");

                entity.Property(e => e.NumSpecialAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numSpecialAllowance");

                entity.Property(e => e.NumTotalSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numTotalSalary");

                entity.Property(e => e.NumWashingAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numWashingAllowance");

                entity.Property(e => e.ReqNumBasic)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumBasic");

                entity.Property(e => e.ReqNumCbadeduction)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumCBADeduction");

                entity.Property(e => e.ReqNumConveyanceAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumConveyanceAllowance");

                entity.Property(e => e.ReqNumGrossSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumGrossSalary");

                entity.Property(e => e.ReqNumHouseAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumHouseAllowance");

                entity.Property(e => e.ReqNumMedicalAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumMedicalAllowance");

                entity.Property(e => e.ReqNumSpecialAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumSpecialAllowance");

                entity.Property(e => e.ReqNumTotalSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumTotalSalary");

                entity.Property(e => e.ReqNumWashingAllowance)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("reqNumWashingAllowance");
            });

            modelBuilder.Entity<PyrEmployeeSalaryElementAssignHeader>(entity =>
            {
                entity.HasKey(e => e.IntEmpSalaryElementAssignHeaderId);

                entity.ToTable("pyrEmployeeSalaryElementAssignHeader", "saas");

                entity.Property(e => e.IntEmpSalaryElementAssignHeaderId).HasColumnName("intEmpSalaryElementAssignHeaderId");

                entity.Property(e => e.DteCreateDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateDateTime");

                entity.Property(e => e.DteUpdateDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdateDateTime");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreateBy).HasColumnName("intCreateBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntSalaryBreakdownHeaderId).HasColumnName("intSalaryBreakdownHeaderId");

                entity.Property(e => e.IntUpdateBy).HasColumnName("intUpdateBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPerdaySalary).HasColumnName("isPerdaySalary");

                entity.Property(e => e.NumBasicOrgross)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numBasicORGross");

                entity.Property(e => e.NumBasicOrgrossEncrypted)
                    .HasColumnName("numBasicORGrossEncrypted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumGrossSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numGrossSalary");

                entity.Property(e => e.NumGrossSalaryEncrypted).HasColumnName("numGrossSalaryEncrypted");

                entity.Property(e => e.NumNetGrossSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numNetGrossSalary");

                entity.Property(e => e.NumNetGrossSalaryEncrypted)
                    .HasColumnName("numNetGrossSalaryEncrypted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StrSalaryBreakdownHeaderTitle)
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryBreakdownHeaderTitle");
            });

            modelBuilder.Entity<PyrEmployeeSalaryElementAssignRow>(entity =>
            {
                entity.HasKey(e => e.IntEmpSalaryElementAssignRowId)
                    .HasName("PK_pyrEmployeeSalaryElementAssignId");

                entity.ToTable("pyrEmployeeSalaryElementAssignRow", "saas");

                entity.Property(e => e.IntEmpSalaryElementAssignRowId).HasColumnName("intEmpSalaryElementAssignRowId");

                entity.Property(e => e.DteCreateDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateDateTime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCreateBy).HasColumnName("intCreateBy");

                entity.Property(e => e.IntEmpSalaryElementAssignHeaderId).HasColumnName("intEmpSalaryElementAssignHeaderId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntSalaryElementId).HasColumnName("intSalaryElementId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NumAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numAmount");

                entity.Property(e => e.NumNumberOfPercent)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numNumberOfPercent");

                entity.Property(e => e.StrDependOn)
                    .HasMaxLength(150)
                    .HasColumnName("strDependOn");

                entity.Property(e => e.StrSalaryElement)
                    .HasMaxLength(150)
                    .HasColumnName("strSalaryElement");
            });

            modelBuilder.Entity<PyrGrossWiseBasicConfig>(entity =>
            {
                entity.HasKey(e => e.IntGrossWiseBasicId);

                entity.ToTable("pyrGrossWiseBasicConfig", "saas");

                entity.Property(e => e.IntGrossWiseBasicId).HasColumnName("intGrossWiseBasicId");

                entity.Property(e => e.DteCreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreateBy).HasColumnName("intCreateBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.NumMaxGross)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numMaxGross");

                entity.Property(e => e.NumMinGross)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numMinGross");

                entity.Property(e => e.NumPercentageOfBasic)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numPercentageOfBasic");
            });

            modelBuilder.Entity<PyrIouadjustmentHistory>(entity =>
            {
                entity.HasKey(e => e.IntIouadjustmentId);

                entity.ToTable("pyrIOUAdjustmentHistory", "saas");

                entity.Property(e => e.IntIouadjustmentId).HasColumnName("intIOUAdjustmentId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntIouid).HasColumnName("intIOUId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsAcknowledgement).HasColumnName("isAcknowledgement");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.NumPayableAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numPayableAmount");

                entity.Property(e => e.NumReceivableAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numReceivableAmount");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<PyrIouapplication>(entity =>
            {
                entity.HasKey(e => e.IntIouid);

                entity.ToTable("pyrIOUApplication", "saas");

                entity.Property(e => e.IntIouid).HasColumnName("intIOUId");

                entity.Property(e => e.DteApplicationDate)
                    .HasColumnType("date")
                    .HasColumnName("dteApplicationDate");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteFromDate)
                    .HasColumnType("date")
                    .HasColumnName("dteFromDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteToDate)
                    .HasColumnType("date")
                    .HasColumnName("dteToDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.NumAdjustedAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numAdjustedAmount");

                entity.Property(e => e.NumIouamount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numIOUAmount");

                entity.Property(e => e.NumPayableAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numPayableAmount");

                entity.Property(e => e.NumPendingAdjAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numPendingAdjAmount");

                entity.Property(e => e.NumReceivableAmount)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numReceivableAmount");

                entity.Property(e => e.StrDiscription)
                    .HasMaxLength(1000)
                    .HasColumnName("strDiscription");

                entity.Property(e => e.StrIoucode)
                    .HasMaxLength(150)
                    .HasColumnName("strIOUCode");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<PyrIoudocument>(entity =>
            {
                entity.HasKey(e => e.IntIoudocId);

                entity.ToTable("pyrIOUDocument", "saas");

                entity.Property(e => e.IntIoudocId).HasColumnName("intIOUDocId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDocUrlid).HasColumnName("intDocURLId");

                entity.Property(e => e.IntIouid).HasColumnName("intIOUId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrDocFor)
                    .HasMaxLength(150)
                    .HasColumnName("strDocFor");
            });

            modelBuilder.Entity<PyrPayrollElementAndRulesTest>(entity =>
            {
                entity.HasKey(e => e.IntPayrollElementId);

                entity.ToTable("pyrPayrollElementAndRulesTest", "saas");

                entity.Property(e => e.IntPayrollElementId).HasColumnName("intPayrollElementId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntPayrollElementTypeId).HasColumnName("intPayrollElementTypeId");

                entity.Property(e => e.IntPercentageOfPayrollElementId).HasColumnName("intPercentageOfPayrollElementId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDefaultSalary).HasColumnName("isDefaultSalary");

                entity.Property(e => e.NumAmount)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numAmount");

                entity.Property(e => e.StrCalculationOn)
                    .HasMaxLength(250)
                    .HasColumnName("strCalculationOn");

                entity.Property(e => e.StrEmploymentTypeName)
                    .HasMaxLength(500)
                    .HasColumnName("strEmploymentTypeName");

                entity.Property(e => e.StrPayrollElementCode)
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollElementCode");

                entity.Property(e => e.StrPayrollElementName)
                    .HasMaxLength(500)
                    .HasColumnName("strPayrollElementName");

                entity.Property(e => e.StrPayrollElementTypeName)
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollElementTypeName");

                entity.Property(e => e.StrPercentageOfPayrollElement)
                    .HasMaxLength(250)
                    .HasColumnName("strPercentageOfPayrollElement");
            });

            modelBuilder.Entity<PyrPayrollElementType>(entity =>
            {
                entity.HasKey(e => e.IntPayrollElementTypeId)
                    .HasName("PK_pyrPayrollElementType_1");

                entity.ToTable("pyrPayrollElementType", "saas");

                entity.Property(e => e.IntPayrollElementTypeId).HasColumnName("intPayrollElementTypeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsAddition).HasColumnName("isAddition");

                entity.Property(e => e.IsBasicSalary).HasColumnName("isBasicSalary");

                entity.Property(e => e.IsDeduction).HasColumnName("isDeduction");

                entity.Property(e => e.IsPrimarySalary).HasColumnName("isPrimarySalary");

                entity.Property(e => e.IsTaxable).HasColumnName("isTaxable");

                entity.Property(e => e.StrCode)
                    .HasMaxLength(50)
                    .HasColumnName("strCode");

                entity.Property(e => e.StrPayrollElementName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollElementName");
            });

            modelBuilder.Entity<PyrPayrollGroup>(entity =>
            {
                entity.HasKey(e => e.IntPayrollGroupId);

                entity.ToTable("pyrPayrollGroup", "saas");

                entity.Property(e => e.IntPayrollGroupId).HasColumnName("intPayrollGroupId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntPayFrequencyId).HasColumnName("intPayFrequencyId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrEndDateOfMonth)
                    .HasMaxLength(50)
                    .HasColumnName("strEndDateOfMonth");

                entity.Property(e => e.StrPayFrequencyName)
                    .HasMaxLength(250)
                    .HasColumnName("strPayFrequencyName");

                entity.Property(e => e.StrPayrollGroupCode)
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollGroupCode");

                entity.Property(e => e.StrPayrollGroupName)
                    .HasMaxLength(500)
                    .HasColumnName("strPayrollGroupName");

                entity.Property(e => e.StrStartDateOfMonth)
                    .HasMaxLength(50)
                    .HasColumnName("strStartDateOfMonth");
            });

            modelBuilder.Entity<PyrPayrollSalaryGenerateRequest>(entity =>
            {
                entity.HasKey(e => e.IntSalaryGenerateRequestId);

                entity.ToTable("pyrPayrollSalaryGenerateRequest", "saas");

                entity.Property(e => e.IntSalaryGenerateRequestId).HasColumnName("intSalaryGenerateRequestId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteSalaryGenerateFrom)
                    .HasColumnType("date")
                    .HasColumnName("dteSalaryGenerateFrom");

                entity.Property(e => e.DteSalaryGenerateTo)
                    .HasColumnType("date")
                    .HasColumnName("dteSalaryGenerateTo");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntAreaId).HasColumnName("intAreaId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntMonth).HasColumnName("intMonth");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRegionId).HasColumnName("intRegionId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntSoleDepoId).HasColumnName("intSoleDepoId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWingId).HasColumnName("intWingId");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IntYear).HasColumnName("intYear");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsApprovedByHeadOffice)
                    .HasColumnName("isApprovedByHeadOffice")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsApprovedBySoleDepo)
                    .HasColumnName("isApprovedBySoleDepo")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsGenerated).HasColumnName("isGenerated");

                entity.Property(e => e.IsHasRankingOrder).HasColumnName("isHasRankingOrder");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsProcessing)
                    .HasColumnName("isProcessing")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.IsRejectedByHeadOffice).HasColumnName("isRejectedByHeadOffice");

                entity.Property(e => e.IsRejectedBySoleDepo).HasColumnName("isRejectedBySoleDepo");

                entity.Property(e => e.NumNetPayableSalary)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numNetPayableSalary");

                entity.Property(e => e.StrBusinessUnit)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBusinessUnit");

                entity.Property(e => e.StrDescription)
                    .HasMaxLength(500)
                    .HasColumnName("strDescription");

                entity.Property(e => e.StrSalaryCode)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryCode");

                entity.Property(e => e.StrSalaryType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strSalaryType");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");

                entity.Property(e => e.StrTerritoryIdList).HasColumnName("strTerritoryIdList");

                entity.Property(e => e.StrTerritoryNameList).HasColumnName("strTerritoryNameList");

                entity.Property(e => e.StrWorkplaceGroupName)
                    .HasMaxLength(300)
                    .HasColumnName("strWorkplaceGroupName");
            });

            modelBuilder.Entity<PyrPayscaleGrade>(entity =>
            {
                entity.HasKey(e => e.IntPayscaleGradeId);

                entity.ToTable("pyrPayscaleGrade", "saas");

                entity.Property(e => e.IntPayscaleGradeId).HasColumnName("intPayscaleGradeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntShortOrder).HasColumnName("intShortOrder");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NumMaxSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numMaxSalary");

                entity.Property(e => e.NumMinSalary)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numMinSalary");

                entity.Property(e => e.StrDepentOn)
                    .HasMaxLength(100)
                    .HasColumnName("strDepentOn");

                entity.Property(e => e.StrPayscaleGradeCode)
                    .HasMaxLength(250)
                    .HasColumnName("strPayscaleGradeCode");

                entity.Property(e => e.StrPayscaleGradeName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strPayscaleGradeName");
            });

            modelBuilder.Entity<PyrSalaryBreakdownHeader>(entity =>
            {
                entity.HasKey(e => e.IntSalaryBreakdownHeaderId)
                    .HasName("PK_pyrSalaryBreakdow");

                entity.ToTable("pyrSalaryBreakdownHeader", "saas");

                entity.Property(e => e.IntSalaryBreakdownHeaderId).HasColumnName("intSalaryBreakdownHeaderId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntHrPositionId).HasColumnName("intHrPositionId");

                entity.Property(e => e.IntSalaryPolicyId).HasColumnName("intSalaryPolicyId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDefault).HasColumnName("isDefault");

                entity.Property(e => e.IsPerday).HasColumnName("isPerday");

                entity.Property(e => e.StrDependOn)
                    .HasMaxLength(50)
                    .HasColumnName("strDependOn");

                entity.Property(e => e.StrSalaryBreakdownTitle)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryBreakdownTitle");
            });

            modelBuilder.Entity<PyrSalaryBreakdownRow>(entity =>
            {
                entity.HasKey(e => e.IntSalaryBreakdownRowId)
                    .HasName("PK_pyrSalaryBreakdownAssign");

                entity.ToTable("pyrSalaryBreakdownRow", "saas");

                entity.Property(e => e.IntSalaryBreakdownRowId).HasColumnName("intSalaryBreakdownRowId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntPayrollElementTypeId).HasColumnName("intPayrollElementTypeId");

                entity.Property(e => e.IntSalaryBreakdownHeaderId).HasColumnName("intSalaryBreakdownHeaderId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.NumAmount)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numAmount");

                entity.Property(e => e.NumNumberOfPercent)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numNumberOfPercent");

                entity.Property(e => e.StrBasedOn)
                    .HasMaxLength(50)
                    .HasColumnName("strBasedOn");

                entity.Property(e => e.StrDependOn)
                    .HasMaxLength(50)
                    .HasColumnName("strDependOn");

                entity.Property(e => e.StrPayrollElementName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollElementName");
            });

            modelBuilder.Entity<PyrSalaryGenerateHeader>(entity =>
            {
                entity.HasKey(e => e.IntSalaryGenerateHeaderId);

                entity.ToTable("pyrSalaryGenerateHeader", "saas");

                entity.Property(e => e.IntSalaryGenerateHeaderId).HasColumnName("intSalaryGenerateHeaderId");

                entity.Property(e => e.DteDateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("dteDateOfBirth");

                entity.Property(e => e.DteJoiningDate)
                    .HasColumnType("date")
                    .HasColumnName("dteJoiningDate");

                entity.Property(e => e.DteManualSalaryAdjustmentDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteManualSalaryAdjustmentDateTime");

                entity.Property(e => e.DtePayrollGenerateDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dtePayrollGenerateDateTime");

                entity.Property(e => e.DtePayrollGenerateFrom)
                    .HasColumnType("date")
                    .HasColumnName("dtePayrollGenerateFrom");

                entity.Property(e => e.DtePayrollGenerateTo)
                    .HasColumnType("date")
                    .HasColumnName("dtePayrollGenerateTo");

                entity.Property(e => e.DteSalaryApprovedDateTime)
                    .HasColumnType("date")
                    .HasColumnName("dteSalaryApprovedDateTime");

                entity.Property(e => e.DteSalaryGenerateFor)
                    .HasColumnType("date")
                    .HasColumnName("dteSalaryGenerateFor");

                entity.Property(e => e.DteSalaryRejectDateTime)
                    .HasColumnType("date")
                    .HasColumnName("dteSalaryRejectDateTime");

                entity.Property(e => e.IntAbsent).HasColumnName("intAbsent");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntActualPayableDays).HasColumnName("intActualPayableDays");

                entity.Property(e => e.IntAnnualLeave).HasColumnName("intAnnualLeave");

                entity.Property(e => e.IntAttendancePunishmentDays)
                    .HasColumnName("intAttendancePunishmentDays")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IntBankBranchId).HasColumnName("intBankBranchId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCasualLeave).HasColumnName("intCasualLeave");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.IntEarnLeave).HasColumnName("intEarnLeave");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmployeeStatusId).HasColumnName("intEmployeeStatusId");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntFinancialInstitutionId).HasColumnName("intFinancialInstitutionId");

                entity.Property(e => e.IntGradeId).HasColumnName("intGradeId");

                entity.Property(e => e.IntHoliday).HasColumnName("intHoliday");

                entity.Property(e => e.IntJoiningAbsentDays)
                    .HasColumnName("intJoiningAbsentDays")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IntLate).HasColumnName("intLate");

                entity.Property(e => e.IntLwp).HasColumnName("intLWP");

                entity.Property(e => e.IntManualSalaryAdjustmentBy)
                    .HasMaxLength(250)
                    .HasColumnName("intManualSalaryAdjustmentBy");

                entity.Property(e => e.IntMaternityLeave).HasColumnName("intMaternityLeave");

                entity.Property(e => e.IntMonthId).HasColumnName("intMonthId");

                entity.Property(e => e.IntMovement).HasColumnName("intMovement");

                entity.Property(e => e.IntOffDay).HasColumnName("intOffDay");

                entity.Property(e => e.IntOthersLeave).HasColumnName("intOthersLeave");

                entity.Property(e => e.IntPayableDays)
                    .HasColumnName("intPayableDays")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IntPayrollGroupId).HasColumnName("intPayrollGroupId");

                entity.Property(e => e.IntPayrollPeriodId).HasColumnName("intPayrollPeriodId");

                entity.Property(e => e.IntPresent).HasColumnName("intPresent");

                entity.Property(e => e.IntPrivilegeLeave).HasColumnName("intPrivilegeLeave");

                entity.Property(e => e.IntSalaryGenerateRequestId).HasColumnName("intSalaryGenerateRequestId");

                entity.Property(e => e.IntSalaryPolicyId).HasColumnName("intSalaryPolicyId");

                entity.Property(e => e.IntSickLeave).HasColumnName("intSickLeave");

                entity.Property(e => e.IntSlaveId).HasColumnName("intSlaveId");

                entity.Property(e => e.IntSpecialLeave).HasColumnName("intSpecialLeave");

                entity.Property(e => e.IntTotalWorkingDays).HasColumnName("intTotalWorkingDays");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IntYearId).HasColumnName("intYearId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsApprove).HasColumnName("isApprove");

                entity.Property(e => e.IsPerday).HasColumnName("isPerday");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.IsTakeHomePay)
                    .HasColumnName("isTakeHomePay")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumArearSalary)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("numArearSalary");

                entity.Property(e => e.NumGrossSalary)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numGrossSalary");

                entity.Property(e => e.NumLoanAmount)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("numLoanAmount")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumLowerLimit)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numLowerLimit")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumManualSalaryAddition)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numManualSalaryAddition")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumManualSalaryDeduction)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numManualSalaryDeduction")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumMissingOfficeHourPunishmentAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numMissingOfficeHourPunishmentAmount");

                entity.Property(e => e.NumMissingOfficeHourPunishmentMinute).HasColumnName("numMissingOfficeHourPunishmentMinute");

                entity.Property(e => e.NumNetPayableSalary)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numNetPayableSalary")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumOverTimeAmount)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("numOverTimeAmount");

                entity.Property(e => e.NumOverTimeHour)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("numOverTimeHour");

                entity.Property(e => e.NumPayableSalaryCal)
                    .HasColumnType("numeric(29, 12)")
                    .HasColumnName("numPayableSalaryCal")
                    .HasComputedColumnSql("([numPerDaySalary]*[intPayableDays])", false);

                entity.Property(e => e.NumPerDaySalary)
                    .HasColumnType("numeric(18, 12)")
                    .HasColumnName("numPerDaySalary")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumPfamount)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("numPFAmount")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumPfcompany)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("numPFCompany");

                entity.Property(e => e.NumTaxAmount)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("numTaxAmount")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumTotalAllowance)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("numTotalAllowance");

                entity.Property(e => e.NumTotalDeduction)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("numTotalDeduction");

                entity.Property(e => e.NumUpperLimit)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numUpperLimit")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StrAccountName)
                    .HasMaxLength(300)
                    .HasColumnName("strAccountName");

                entity.Property(e => e.StrAccountNo)
                    .HasMaxLength(50)
                    .HasColumnName("strAccountNo");

                entity.Property(e => e.StrAge)
                    .HasMaxLength(300)
                    .HasColumnName("strAge");

                entity.Property(e => e.StrBankBranchName)
                    .HasMaxLength(300)
                    .HasColumnName("strBankBranchName");

                entity.Property(e => e.StrBusinessUnitName)
                    .HasMaxLength(300)
                    .HasColumnName("strBusinessUnitName");

                entity.Property(e => e.StrContactNumber)
                    .HasMaxLength(50)
                    .HasColumnName("strContactNumber");

                entity.Property(e => e.StrDepartment)
                    .HasMaxLength(300)
                    .HasColumnName("strDepartment");

                entity.Property(e => e.StrDesignation)
                    .HasMaxLength(300)
                    .HasColumnName("strDesignation");

                entity.Property(e => e.StrEmployeeCode)
                    .HasMaxLength(150)
                    .HasColumnName("strEmployeeCode");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(300)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrEmployeeStatus)
                    .HasMaxLength(150)
                    .HasColumnName("strEmployeeStatus");

                entity.Property(e => e.StrEmploymentType)
                    .HasMaxLength(150)
                    .HasColumnName("strEmploymentType");

                entity.Property(e => e.StrFinancialInstitution)
                    .HasMaxLength(300)
                    .HasColumnName("strFinancialInstitution");

                entity.Property(e => e.StrGrade)
                    .HasMaxLength(200)
                    .HasColumnName("strGrade");

                entity.Property(e => e.StrOfficialEmail)
                    .HasMaxLength(300)
                    .HasColumnName("strOfficialEmail");

                entity.Property(e => e.StrPaymentBankType)
                    .HasMaxLength(50)
                    .HasColumnName("strPaymentBankType");

                entity.Property(e => e.StrPayrollGroupName)
                    .HasMaxLength(300)
                    .HasColumnName("strPayrollGroupName");

                entity.Property(e => e.StrPayrollPeriod)
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollPeriod");

                entity.Property(e => e.StrRoutingNumber)
                    .HasMaxLength(20)
                    .HasColumnName("strRoutingNumber");

                entity.Property(e => e.StrSalaryApprovedByUser)
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryApprovedByUser");

                entity.Property(e => e.StrSalaryPolicyName)
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryPolicyName");

                entity.Property(e => e.StrSalaryRejectByUser)
                    .HasMaxLength(250)
                    .HasColumnName("strSalaryRejectByUser");

                entity.Property(e => e.StrServiceLength)
                    .HasMaxLength(300)
                    .HasColumnName("strServiceLength");

                entity.Property(e => e.StrSlave)
                    .HasMaxLength(200)
                    .HasColumnName("strSlave");

                entity.Property(e => e.StrWorkplaceGroupName)
                    .HasMaxLength(300)
                    .HasColumnName("strWorkplaceGroupName");

                entity.Property(e => e.StrWorkplaceName)
                    .HasMaxLength(300)
                    .HasColumnName("strWorkplaceName");
            });

            modelBuilder.Entity<PyrSalaryGenerateRow>(entity =>
            {
                entity.HasKey(e => e.IntSalaryGenerateRow);

                entity.ToTable("pyrSalaryGenerateRow", "saas");

                entity.Property(e => e.IntSalaryGenerateRow).HasColumnName("intSalaryGenerateRow");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntMonthId).HasColumnName("intMonthId");

                entity.Property(e => e.IntPayrollElementId).HasColumnName("intPayrollElementId");

                entity.Property(e => e.IntPayrollElementTypeId).HasColumnName("intPayrollElementTypeId");

                entity.Property(e => e.IntSalaryGenerateHeaderId).HasColumnName("intSalaryGenerateHeaderId");

                entity.Property(e => e.IntSalaryGenerateRequestId).HasColumnName("intSalaryGenerateRequestId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntYearId).HasColumnName("intYearId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.NumAmount)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("numAmount");

                entity.Property(e => e.StrPayrollElement)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPayrollElement");

                entity.Property(e => e.StrPayrollElementCode)
                    .HasMaxLength(5)
                    .HasColumnName("strPayrollElementCode");
            });

            modelBuilder.Entity<PyrSalaryPolicy>(entity =>
            {
                entity.HasKey(e => e.IntPolicyId);

                entity.ToTable("pyrSalaryPolicy", "saas");

                entity.Property(e => e.IntPolicyId).HasColumnName("intPolicyId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntGrossSalaryDevidedByDays).HasColumnName("intGrossSalaryDevidedByDays");

                entity.Property(e => e.IntGrossSalaryRoundDigits).HasColumnName("intGrossSalaryRoundDigits");

                entity.Property(e => e.IntNetPayableSalaryRoundDigits).HasColumnName("intNetPayableSalaryRoundDigits");

                entity.Property(e => e.IntNextMonthEndDay).HasColumnName("intNextMonthEndDay");

                entity.Property(e => e.IntPreviousMonthStartDay).HasColumnName("intPreviousMonthStartDay");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsGrossSalaryRoundDown).HasColumnName("isGrossSalaryRoundDown");

                entity.Property(e => e.IsGrossSalaryRoundUp).HasColumnName("isGrossSalaryRoundUp");

                entity.Property(e => e.IsNetPayableSalaryRoundDown).HasColumnName("isNetPayableSalaryRoundDown");

                entity.Property(e => e.IsNetPayableSalaryRoundUp).HasColumnName("isNetPayableSalaryRoundUp");

                entity.Property(e => e.IsSalaryCalculationShouldBeActual).HasColumnName("isSalaryCalculationShouldBeActual");

                entity.Property(e => e.IsSalaryShouldBeFullMonth).HasColumnName("isSalaryShouldBeFullMonth");

                entity.Property(e => e.StrPolicyName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPolicyName");
            });

            modelBuilder.Entity<RefreshTokenHistory>(entity =>
            {
                entity.HasKey(e => e.IntAutoId);

                entity.ToTable("RefreshTokenHistory", "auth");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrLogInId)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strLogInId");

                entity.Property(e => e.StrRefreshToken)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strRefreshToken");
            });

            modelBuilder.Entity<Religion>(entity =>
            {
                entity.HasKey(e => e.IntReligionId)
                    .HasName("PK__Religion__1262ED991DE4B5C8");

                entity.ToTable("Religion", "core");

                entity.Property(e => e.IntReligionId).HasColumnName("intReligionId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrReligion)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strReligion");

                entity.Property(e => e.StrReligionCode)
                    .HasMaxLength(50)
                    .HasColumnName("strReligionCode");
            });

            modelBuilder.Entity<RoleBridgeWithDesignation>(entity =>
            {
                entity.HasKey(e => e.IntId);

                entity.ToTable("RoleBridgeWithDesignation", "auth");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedDateTime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdateDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdateDateTime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDesignationOrEmployeeId).HasColumnName("intDesignationOrEmployeeId");

                entity.Property(e => e.IntRoleId).HasColumnName("intRoleId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrIsFor)
                    .HasMaxLength(250)
                    .HasColumnName("strIsFor");
            });

            modelBuilder.Entity<RoleExtensionHeader>(entity =>
            {
                entity.HasKey(e => e.IntRoleExtensionHeaderId);

                entity.ToTable("RoleExtensionHeader", "auth");

                entity.Property(e => e.IntRoleExtensionHeaderId).HasColumnName("intRoleExtensionHeaderId");

                entity.Property(e => e.DteCreatedDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedDateTime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<RoleExtensionRow>(entity =>
            {
                entity.HasKey(e => e.IntRoleExtensionRowId);

                entity.ToTable("RoleExtensionRow", "auth");

                entity.Property(e => e.IntRoleExtensionRowId).HasColumnName("intRoleExtensionRowId");

                entity.Property(e => e.DteCreatedDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedDateTime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntOrganizationReffId).HasColumnName("intOrganizationReffId");

                entity.Property(e => e.IntOrganizationTypeId).HasColumnName("intOrganizationTypeId");

                entity.Property(e => e.IntRoleExtensionHeaderId).HasColumnName("intRoleExtensionHeaderId");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrOrganizationReffName)
                    .HasMaxLength(250)
                    .HasColumnName("strOrganizationReffName");

                entity.Property(e => e.StrOrganizationTypeName)
                    .HasMaxLength(250)
                    .HasColumnName("strOrganizationTypeName");
            });

            modelBuilder.Entity<RoleGroupHeader>(entity =>
            {
                entity.HasKey(e => e.IntRoleGroupId);

                entity.ToTable("RoleGroupHeader", "auth");

                entity.Property(e => e.IntRoleGroupId).HasColumnName("intRoleGroupId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrRoleGroupCode)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strRoleGroupCode");

                entity.Property(e => e.StrRoleGroupName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strRoleGroupName");
            });

            modelBuilder.Entity<RoleGroupRow>(entity =>
            {
                entity.HasKey(e => e.IntRowId);

                entity.ToTable("RoleGroupRow", "auth");

                entity.Property(e => e.IntRowId).HasColumnName("intRowId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntRoleGroupId).HasColumnName("intRoleGroupId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntUserId).HasColumnName("intUserId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrUserName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strUserName");
            });

            modelBuilder.Entity<TerritorySetup>(entity =>
            {
                entity.HasKey(e => e.IntTerritoryId);

                entity.ToTable("TerritorySetup", "core");

                entity.Property(e => e.IntTerritoryId).HasColumnName("intTerritoryId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdateBy)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdateBy");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntHrPositionId).HasColumnName("intHrPositionId");

                entity.Property(e => e.IntParentTerritoryId).HasColumnName("intParentTerritoryId");

                entity.Property(e => e.IntRankingId).HasColumnName("intRankingId");

                entity.Property(e => e.IntTerritoryTypeId).HasColumnName("intTerritoryTypeId");

                entity.Property(e => e.IntUpdateBy).HasColumnName("intUpdateBy");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(500)
                    .HasColumnName("strRemarks");

                entity.Property(e => e.StrTerritoryAddress)
                    .HasMaxLength(250)
                    .HasColumnName("strTerritoryAddress");

                entity.Property(e => e.StrTerritoryCode)
                    .HasMaxLength(50)
                    .HasColumnName("strTerritoryCode");

                entity.Property(e => e.StrTerritoryName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTerritoryName");
            });

            modelBuilder.Entity<Thana>(entity =>
            {
                entity.HasKey(e => e.IntThanaId);

                entity.ToTable("Thana", "core");

                entity.Property(e => e.IntThanaId).HasColumnName("intThanaId");

                entity.Property(e => e.DteInsertDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteInsertDateTime");

                entity.Property(e => e.IntCountryId).HasColumnName("intCountryId");

                entity.Property(e => e.IntDistrictId).HasColumnName("intDistrictId");

                entity.Property(e => e.IntDivisionId).HasColumnName("intDivisionId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrInsertUserId)
                    .HasMaxLength(500)
                    .HasColumnName("strInsertUserId");

                entity.Property(e => e.StrThanaBn)
                    .HasMaxLength(250)
                    .HasColumnName("strThanaBN");

                entity.Property(e => e.StrThanaName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strThanaName");
            });

            modelBuilder.Entity<TimeAttendanceDailySummary>(entity =>
            {
                entity.HasKey(e => e.IntAutoId);

                entity.ToTable("timeAttendanceDailySummary", "saas");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.DteAttendanceDate)
                    .HasColumnType("date")
                    .HasColumnName("dteAttendanceDate");

                entity.Property(e => e.DteBreakEndTime).HasColumnName("dteBreakEndTime");

                entity.Property(e => e.DteBreakStartTime).HasColumnName("dteBreakStartTime");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteEndTime).HasColumnName("dteEndTime");

                entity.Property(e => e.DteExtendedStartTime).HasColumnName("dteExtendedStartTime");

                entity.Property(e => e.DteGenerateDate)
                    .HasColumnType("date")
                    .HasColumnName("dteGenerateDate");

                entity.Property(e => e.DteLastStartTime).HasColumnName("dteLastStartTime");

                entity.Property(e => e.DteManualAttendanceDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteManualAttendanceDate");

                entity.Property(e => e.DteNextChangeDate)
                    .HasColumnType("date")
                    .HasColumnName("dteNextChangeDate");

                entity.Property(e => e.DteOfficeClosingTime)
                    .HasColumnType("time(0)")
                    .HasColumnName("dteOfficeClosingTime");

                entity.Property(e => e.DteOfficeOpeningTime)
                    .HasColumnType("time(0)")
                    .HasColumnName("dteOfficeOpeningTime");

                entity.Property(e => e.DteProcessDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteProcessDateTime");

                entity.Property(e => e.DteStartTime).HasColumnName("dteStartTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCalendarId).HasColumnName("intCalendarId");

                entity.Property(e => e.IntCalendarTypeId).HasColumnName("intCalendarTypeId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDayId).HasColumnName("intDayId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntManualAttendanceBy).HasColumnName("intManualAttendanceBy");

                entity.Property(e => e.IntMissingWorkingHoursInMinutes).HasColumnName("intMissingWorkingHoursInMinutes");

                entity.Property(e => e.IntMonthId).HasColumnName("intMonthId");

                entity.Property(e => e.IntPunchCount).HasColumnName("intPunchCount");

                entity.Property(e => e.IntRosterGroupId).HasColumnName("intRosterGroupId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkingHourInMinute).HasColumnName("intWorkingHourInMinute");

                entity.Property(e => e.IntYear).HasColumnName("intYear");

                entity.Property(e => e.IsAbsent)
                    .HasColumnName("isAbsent")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsAutoGenerate).HasColumnName("isAutoGenerate");

                entity.Property(e => e.IsEarlyLeave)
                    .HasColumnName("isEarlyLeave")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsHoliday)
                    .HasColumnName("isHoliday")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsLate)
                    .HasColumnName("isLate")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsLeave)
                    .HasColumnName("isLeave")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsLeaveWithPay)
                    .HasColumnName("isLeaveWithPay")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsManual).HasColumnName("isManual");

                entity.Property(e => e.IsManualAbsent).HasColumnName("isManualAbsent");

                entity.Property(e => e.IsManualLate).HasColumnName("isManualLate");

                entity.Property(e => e.IsManualLeave).HasColumnName("isManualLeave");

                entity.Property(e => e.IsManualPresent).HasColumnName("isManualPresent");

                entity.Property(e => e.IsMovement)
                    .HasColumnName("isMovement")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsNightShift)
                    .HasColumnName("isNightShift")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsOffday)
                    .HasColumnName("isOffday")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsPresent)
                    .HasColumnName("isPresent")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsProcess)
                    .HasColumnName("isProcess")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsWorkingDayCal)
                    .HasColumnName("isWorkingDayCal")
                    .HasComputedColumnSql("(case when isnull([isHoliday],(0))=(0) AND isnull([isOffday],(0))=(0) then (1) else (0) end)", false);

                entity.Property(e => e.NumMinWorkHour)
                    .HasColumnType("numeric(4, 0)")
                    .HasColumnName("numMinWorkHour");

                entity.Property(e => e.NumModifiedOverTime)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numModifiedOverTime");

                entity.Property(e => e.NumOverTime)
                    .HasColumnType("decimal(18, 6)")
                    .HasColumnName("numOverTime");

                entity.Property(e => e.StrCalendarName)
                    .HasMaxLength(250)
                    .HasColumnName("strCalendarName");

                entity.Property(e => e.StrCalendarType)
                    .HasMaxLength(250)
                    .HasColumnName("strCalendarType");

                entity.Property(e => e.StrRosterGroupName)
                    .HasMaxLength(500)
                    .HasColumnName("strRosterGroupName");

                entity.Property(e => e.StrWorkingHours)
                    .HasMaxLength(50)
                    .HasColumnName("strWorkingHours");

                entity.Property(e => e.TmeAttendanceHour).HasColumnName("tmeAttendanceHour");

                entity.Property(e => e.TmeEarlyLeaveHour).HasColumnName("tmeEarlyLeaveHour");

                entity.Property(e => e.TmeExtraHour).HasColumnName("tmeExtraHour");

                entity.Property(e => e.TmeInTime).HasColumnName("tmeInTime");

                entity.Property(e => e.TmeLastOutTime).HasColumnName("tmeLastOutTime");

                entity.Property(e => e.TmeLateHour).HasColumnName("tmeLateHour");

                entity.Property(e => e.TmeOtOutTime).HasColumnName("tmeOtOutTime");

                entity.Property(e => e.TmeOtStartTime).HasColumnName("tmeOtStartTime");

                entity.Property(e => e.TmeShiftOverTime).HasColumnName("tmeShiftOverTime");
            });

            modelBuilder.Entity<TimeAttendanceProcessRequestHeader>(entity =>
            {
                entity.HasKey(e => e.IntHeaderRequestId);

                entity.ToTable("timeAttendanceProcessRequestHeader", "saas");

                entity.Property(e => e.IntHeaderRequestId).HasColumnName("intHeaderRequestId");

                entity.Property(e => e.DteCreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateDate");

                entity.Property(e => e.DteFromDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteFromDate");

                entity.Property(e => e.DteProcessDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteProcessDate");

                entity.Property(e => e.DteToDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteToDate");

                entity.Property(e => e.DteUpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdateDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreateBy).HasColumnName("intCreateBy");

                entity.Property(e => e.IntTotalEmployee).HasColumnName("intTotalEmployee");

                entity.Property(e => e.IntUpdateBy).HasColumnName("intUpdateBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsAll).HasColumnName("isAll");
            });

            modelBuilder.Entity<TimeAttendanceProcessRequestRow>(entity =>
            {
                entity.HasKey(e => e.IntRowRequestId);

                entity.ToTable("timeAttendanceProcessRequestRow", "saas");

                entity.Property(e => e.IntRowRequestId).HasColumnName("intRowRequestId");

                entity.Property(e => e.DteCreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateDate");

                entity.Property(e => e.DteUpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdateDate");

                entity.Property(e => e.IntCreateBy).HasColumnName("intCreateBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntHeaderRequestId).HasColumnName("intHeaderRequestId");

                entity.Property(e => e.IntUpdateBy).HasColumnName("intUpdateBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");
            });

            modelBuilder.Entity<TimeCalender>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("timeCalender", "saas");

                entity.Property(e => e.DteBreakEndTime).HasColumnName("dteBreakEndTime");

                entity.Property(e => e.DteBreakStartTime).HasColumnName("dteBreakStartTime");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteEndTime).HasColumnName("dteEndTime");

                entity.Property(e => e.DteExtendedStartTime).HasColumnName("dteExtendedStartTime");

                entity.Property(e => e.DteLastStartTime).HasColumnName("dteLastStartTime");

                entity.Property(e => e.DteOfficeCloseTime)
                    .HasColumnName("dteOfficeCloseTime")
                    .HasDefaultValueSql("('00:00:00.0000000')");

                entity.Property(e => e.DteOfficeStartTime)
                    .HasColumnName("dteOfficeStartTime")
                    .HasDefaultValueSql("('00:00:00.0000000')");

                entity.Property(e => e.DteStartTime).HasColumnName("dteStartTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCalenderId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("intCalenderId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsNightShift)
                    .HasColumnName("isNightShift")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NumMinWorkHour)
                    .HasColumnType("numeric(4, 0)")
                    .HasColumnName("numMinWorkHour");

                entity.Property(e => e.StrCalenderCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strCalenderCode");

                entity.Property(e => e.StrCalenderName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strCalenderName");
            });

            modelBuilder.Entity<TimeEmpOverTime>(entity =>
            {
                entity.HasKey(e => e.IntOverTimeId);

                entity.ToTable("timeEmpOverTime", "saas");

                entity.Property(e => e.IntOverTimeId).HasColumnName("intOverTimeId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteOverTimeDate)
                    .HasColumnType("date")
                    .HasColumnName("dteOverTimeDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntMonth).HasColumnName("intMonth");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IntWorkplaceId).HasColumnName("intWorkplaceId");

                entity.Property(e => e.IntYear).HasColumnName("intYear");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.NumOverTimeHour)
                    .HasColumnType("numeric(18, 6)")
                    .HasColumnName("numOverTimeHour");

                entity.Property(e => e.StrDailyOrMonthly)
                    .HasMaxLength(50)
                    .HasColumnName("strDailyOrMonthly");

                entity.Property(e => e.StrReason).HasColumnName("strReason");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");

                entity.Property(e => e.TmeEndTime)
                    .HasColumnType("time(0)")
                    .HasColumnName("tmeEndTime");

                entity.Property(e => e.TmeStartTime)
                    .HasColumnType("time(0)")
                    .HasColumnName("tmeStartTime");
            });

            modelBuilder.Entity<TimeEmpOverTimeUpload>(entity =>
            {
                entity.HasKey(e => e.IntAutoId);

                entity.ToTable("timeEmpOverTimeUpload", "saas");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteOvertimeDate)
                    .HasColumnType("date")
                    .HasColumnName("dteOvertimeDate");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntDay).HasColumnName("intDay");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntFromHour).HasColumnName("intFromHour");

                entity.Property(e => e.IntFromMinute).HasColumnName("intFromMinute");

                entity.Property(e => e.IntMonth).HasColumnName("intMonth");

                entity.Property(e => e.IntToHour).HasColumnName("intToHour");

                entity.Property(e => e.IntToMinute).HasColumnName("intToMinute");

                entity.Property(e => e.IntTotalHoursInMonth).HasColumnName("intTotalHoursInMonth");

                entity.Property(e => e.IntYear).HasColumnName("intYear");

                entity.Property(e => e.IsMonthly).HasColumnName("isMonthly");

                entity.Property(e => e.IsSubmitted).HasColumnName("isSubmitted");

                entity.Property(e => e.IsValid).HasColumnName("isValid");

                entity.Property(e => e.StrDesignationName)
                    .HasMaxLength(250)
                    .HasColumnName("strDesignationName");

                entity.Property(e => e.StrEmployeeCode)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeCode");

                entity.Property(e => e.StrEmployeeName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrFromAmPm)
                    .HasMaxLength(50)
                    .HasColumnName("strFromAmPm");

                entity.Property(e => e.StrFromTime)
                    .HasMaxLength(50)
                    .HasColumnName("strFromTime");

                entity.Property(e => e.StrInsertBy)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strInsertBy");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(500)
                    .HasColumnName("strRemarks");

                entity.Property(e => e.StrToAmPm)
                    .HasMaxLength(50)
                    .HasColumnName("strToAmPm");

                entity.Property(e => e.StrToTime)
                    .HasMaxLength(50)
                    .HasColumnName("strToTime");
            });

            modelBuilder.Entity<TimeEmployeeAttendance>(entity =>
            {
                entity.HasKey(e => e.IntAutoIncrement);

                entity.ToTable("timeEmployeeAttendance", "saas");

                entity.Property(e => e.IntAutoIncrement).HasColumnName("intAutoIncrement");

                entity.Property(e => e.DteAttendanceDate)
                    .HasColumnType("date")
                    .HasColumnName("dteAttendanceDate");

                entity.Property(e => e.DteAttendanceTime).HasColumnName("dteAttendanceTime");

                entity.Property(e => e.DtePunchDate)
                    .HasColumnType("date")
                    .HasColumnName("dtePunchDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteSyncDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteSyncDatetime");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntRemoteAttendanceId).HasColumnName("intRemoteAttendanceId");

                entity.Property(e => e.IsMarket).HasColumnName("isMarket");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsProcess)
                    .HasColumnName("isProcess")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrAttendanceType)
                    .HasMaxLength(50)
                    .HasColumnName("strAttendanceType");

                entity.Property(e => e.StrCloudId)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("strCloudId");

                entity.Property(e => e.StrInOutStatus)
                    .HasMaxLength(250)
                    .HasColumnName("strInOutStatus");

                entity.Property(e => e.StrLatitude)
                    .HasMaxLength(250)
                    .HasColumnName("strLatitude");

                entity.Property(e => e.StrLongitude)
                    .HasMaxLength(250)
                    .HasColumnName("strLongitude");

                entity.Property(e => e.StrRemark)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("strRemark");

                entity.Property(e => e.StrStatus)
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<TimeEmployeeExcOffday>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("timeEmployeeExcOffday", "saas");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteEffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteEffectiveDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmployeeOffdayAssignId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("intEmployeeOffdayAssignId");

                entity.Property(e => e.IntExceptionOffdayGroupId).HasColumnName("intExceptionOffdayGroupId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrExceptionOffdayGroupName)
                    .HasMaxLength(250)
                    .HasColumnName("strExceptionOffdayGroupName");
            });

            modelBuilder.Entity<TimeEmployeeHoliday>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("timeEmployeeHoliday", "saas");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteEffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteEffectiveDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeHolidayAssignId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("intEmployeeHolidayAssignId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntHolidayGroupId).HasColumnName("intHolidayGroupId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrHolidayGroupName)
                    .HasMaxLength(250)
                    .HasColumnName("strHolidayGroupName");
            });

            modelBuilder.Entity<TimeEmployeeOffday>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("timeEmployeeOffday", "saas");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteEffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteEffectiveDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmployeeOffdayAssignId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("intEmployeeOffdayAssignId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWorkplaceGroupId).HasColumnName("intWorkplaceGroupId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsFriday).HasColumnName("isFriday");

                entity.Property(e => e.IsMonday).HasColumnName("isMonday");

                entity.Property(e => e.IsSaturday).HasColumnName("isSaturday");

                entity.Property(e => e.IsSunday).HasColumnName("isSunday");

                entity.Property(e => e.IsThursday).HasColumnName("isThursday");

                entity.Property(e => e.IsTuesday).HasColumnName("isTuesday");

                entity.Property(e => e.IsWednesday).HasColumnName("isWednesday");
            });

            modelBuilder.Entity<TimeEmployeeOffdayReassign>(entity =>
            {
                entity.HasKey(e => e.IntAutoId);

                entity.ToTable("timeEmployeeOffdayReassign", "saas");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteDate)
                    .HasColumnType("date")
                    .HasColumnName("dteDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsOffday).HasColumnName("isOffday");
            });

            modelBuilder.Entity<TimeExceptionOffdayGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("timeExceptionOffdayGroup", "saas");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntExceptionOffdayGroupId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("intExceptionOffdayGroupId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsAlternativeDay).HasColumnName("isAlternativeDay");

                entity.Property(e => e.StrExceptionOffdayName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strExceptionOffdayName");
            });

            modelBuilder.Entity<TimeFixedRoasterSetupDetail>(entity =>
            {
                entity.HasKey(e => e.IntId)
                    .HasName("PK__timeFixe__11B678D2F75832B7");

                entity.ToTable("timeFixedRoasterSetupDetails", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCalendarId).HasColumnName("intCalendarId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntDay).HasColumnName("intDay");

                entity.Property(e => e.IntFixedRoasterMasterId).HasColumnName("intFixedRoasterMasterId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsHoliday).HasColumnName("isHoliday");

                entity.Property(e => e.IsOffDay).HasColumnName("isOffDay");

                entity.Property(e => e.StrCalendarName)
                    .HasMaxLength(250)
                    .HasColumnName("strCalendarName");
            });

            modelBuilder.Entity<TimeFixedRoasterSetupMaster>(entity =>
            {
                entity.HasKey(e => e.IntId)
                    .HasName("PK__timeFixe__11B678D2696C6351");

                entity.ToTable("timeFixedRoasterSetupMaster", "saas");

                entity.Property(e => e.IntId).HasColumnName("intId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnit).HasColumnName("intBusinessUnit");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrRoasterName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strRoasterName");
            });

            modelBuilder.Entity<TimeOffday>(entity =>
            {
                entity.HasKey(e => e.IntOffdayId);

                entity.ToTable("timeOffday", "saas");

                entity.Property(e => e.IntOffdayId).HasColumnName("intOffdayId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntNextWeekdayId).HasColumnName("intNextWeekdayId");

                entity.Property(e => e.IntNoOfDaysChange).HasColumnName("intNoOfDaysChange");

                entity.Property(e => e.IntOffdayGroupId).HasColumnName("intOffdayGroupId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntWeekdayId).HasColumnName("intWeekdayId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrNextWeekdayName)
                    .HasMaxLength(250)
                    .HasColumnName("strNextWeekdayName");

                entity.Property(e => e.StrWeekdayName)
                    .HasMaxLength(250)
                    .HasColumnName("strWeekdayName");
            });

            modelBuilder.Entity<TimeOffdayGroup>(entity =>
            {
                entity.HasKey(e => e.IntOffdayGroupId);

                entity.ToTable("timeOffdayGroup", "saas");

                entity.Property(e => e.IntOffdayGroupId).HasColumnName("intOffdayGroupId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrOffdayGroupName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strOffdayGroupName");
            });

            modelBuilder.Entity<TimeRemoteAttendance>(entity =>
            {
                entity.HasKey(e => e.IntRemoteAttendanceId);

                entity.ToTable("timeRemoteAttendance", "saas");

                entity.Property(e => e.IntRemoteAttendanceId).HasColumnName("intRemoteAttendanceId");

                entity.Property(e => e.DteAttendanceDate)
                    .HasColumnType("date")
                    .HasColumnName("dteAttendanceDate");

                entity.Property(e => e.IntAttendanceRegId).HasColumnName("intAttendanceRegId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntRealTimeImage).HasColumnName("intRealTimeImage");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsInvalidCalendar)
                    .HasColumnName("isInvalidCalendar")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsMarket).HasColumnName("isMarket");

                entity.Property(e => e.StrAddress)
                    .HasMaxLength(500)
                    .HasColumnName("strAddress");

                entity.Property(e => e.StrDeviceId)
                    .HasMaxLength(250)
                    .HasColumnName("strDeviceId");

                entity.Property(e => e.StrDeviceName)
                    .HasMaxLength(250)
                    .HasColumnName("strDeviceName");

                entity.Property(e => e.StrInOutStatus)
                    .HasMaxLength(50)
                    .HasColumnName("strInOutStatus");

                entity.Property(e => e.StrLatitude)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strLatitude");

                entity.Property(e => e.StrLongitude)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strLongitude");

                entity.Property(e => e.StrPlaceName)
                    .HasMaxLength(500)
                    .HasColumnName("strPlaceName");

                entity.Property(e => e.StrRemarks).HasColumnName("strRemarks");

                entity.Property(e => e.StrVisitingCompany)
                    .HasMaxLength(150)
                    .HasColumnName("strVisitingCompany");

                entity.Property(e => e.StrVisitingLocation)
                    .HasMaxLength(500)
                    .HasColumnName("strVisitingLocation");

                entity.Property(e => e.TmAttendanceTime).HasColumnName("tmAttendanceTime");
            });

            modelBuilder.Entity<TimeRemoteAttendanceRegistration>(entity =>
            {
                entity.HasKey(e => e.IntAttendanceRegId);

                entity.ToTable("timeRemoteAttendanceRegistration", "saas");

                entity.Property(e => e.IntAttendanceRegId).HasColumnName("intAttendanceRegId");

                entity.Property(e => e.DteInsertDate)
                    .HasColumnType("date")
                    .HasColumnName("dteInsertDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntInsertBy).HasColumnName("intInsertBy");

                entity.Property(e => e.IntMasterLocationId).HasColumnName("intMasterLocationId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsHomeOffice)
                    .HasColumnName("isHomeOffice")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsLocationRegister).HasColumnName("isLocationRegister");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrAddress)
                    .HasMaxLength(500)
                    .HasColumnName("strAddress");

                entity.Property(e => e.StrDeviceId)
                    .HasMaxLength(250)
                    .HasColumnName("strDeviceId");

                entity.Property(e => e.StrDeviceName)
                    .HasMaxLength(250)
                    .HasColumnName("strDeviceName");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrLatitude)
                    .HasMaxLength(250)
                    .HasColumnName("strLatitude");

                entity.Property(e => e.StrLongitude)
                    .HasMaxLength(250)
                    .HasColumnName("strLongitude");

                entity.Property(e => e.StrPlaceName)
                    .HasMaxLength(500)
                    .HasColumnName("strPlaceName");

                entity.Property(e => e.StrStatus)
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<TimeRemoteAttendanceSetup>(entity =>
            {
                entity.HasKey(e => e.IntAutoId);

                entity.ToTable("timeRemoteAttendanceSetup", "saas");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.DteCreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreateAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy)
                    .HasMaxLength(10)
                    .HasColumnName("intUpdatedBy")
                    .IsFixedLength();

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsCheckInApprovalNeed).HasColumnName("isCheckInApprovalNeed");

                entity.Property(e => e.IsDeviceRegNeed).HasColumnName("isDeviceRegNeed");

                entity.Property(e => e.IsFlexible)
                    .HasColumnName("isFlexible")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsInSideApprovalNeed)
                    .HasColumnName("isInSideApprovalNeed")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsLocationRegNeed).HasColumnName("isLocationRegNeed");

                entity.Property(e => e.IsOutSideApprovalNeed)
                    .HasColumnName("isOutSideApprovalNeed")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsRealTimeImageNeed).HasColumnName("isRealTimeImageNeed");

                entity.Property(e => e.NumMinimumValidDistance)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("numMinimumValidDistance");
            });

            modelBuilder.Entity<TrainingAssesmentAnsware>(entity =>
            {
                entity.HasKey(e => e.IntAnswerId);

                entity.ToTable("TrainingAssesmentAnsware", "saas");

                entity.Property(e => e.IntAnswerId).HasColumnName("intAnswerId");

                entity.Property(e => e.DteLastAction)
                    .HasColumnType("datetime")
                    .HasColumnName("dteLastAction");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntOptionId).HasColumnName("intOptionId");

                entity.Property(e => e.IntQuestionId).HasColumnName("intQuestionId");

                entity.Property(e => e.IntRequisitionId).HasColumnName("intRequisitionId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.NumMarks).HasColumnName("numMarks");

                entity.Property(e => e.StrOption)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strOption");
            });

            modelBuilder.Entity<TrainingAssesmentQuestion>(entity =>
            {
                entity.HasKey(e => e.IntQuestionId);

                entity.ToTable("TrainingAssesmentQuestion", "saas");

                entity.Property(e => e.IntQuestionId).HasColumnName("intQuestionId");

                entity.Property(e => e.DteLastActionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteLastActionDate");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntOrder).HasColumnName("intOrder");

                entity.Property(e => e.IntScheduleId).HasColumnName("intScheduleId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPreAssesment).HasColumnName("isPreAssesment");

                entity.Property(e => e.IsRequired).HasColumnName("isRequired");

                entity.Property(e => e.StrInputType)
                    .HasMaxLength(50)
                    .HasColumnName("strInputType");

                entity.Property(e => e.StrQuestion)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("strQuestion");
            });

            modelBuilder.Entity<TrainingAssesmentQuestionOption>(entity =>
            {
                entity.HasKey(e => e.IntOptionId);

                entity.ToTable("TrainingAssesmentQuestionOption", "saas");

                entity.Property(e => e.IntOptionId).HasColumnName("intOptionId");

                entity.Property(e => e.DteLastAction)
                    .HasColumnType("datetime")
                    .HasColumnName("dteLastAction");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntOrder).HasColumnName("intOrder");

                entity.Property(e => e.IntQuestionId).HasColumnName("intQuestionId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.NumPoints).HasColumnName("numPoints");

                entity.Property(e => e.StrOption)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strOption");
            });

            modelBuilder.Entity<TrainingAttendance>(entity =>
            {
                entity.HasKey(e => e.IntAttendanceId);

                entity.ToTable("TrainingAttendance", "saas");

                entity.Property(e => e.IntAttendanceId).HasColumnName("intAttendanceId");

                entity.Property(e => e.DteActionDate)
                    .HasColumnType("date")
                    .HasColumnName("dteActionDate");

                entity.Property(e => e.DteAttendanceDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteAttendanceDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntRequisitionId).HasColumnName("intRequisitionId");

                entity.Property(e => e.StrAttendanceStatus)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strAttendanceStatus");
            });

            modelBuilder.Entity<TrainingRequisition>(entity =>
            {
                entity.HasKey(e => e.IntRequisitionId);

                entity.ToTable("TrainingRequisition", "saas");

                entity.Property(e => e.IntRequisitionId).HasColumnName("intRequisitionId");

                entity.Property(e => e.DteActionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteActionDate");

                entity.Property(e => e.DteApprovedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteApprovedDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntApprovedBy).HasColumnName("intApprovedBy");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntDepartmentId).HasColumnName("intDepartmentId");

                entity.Property(e => e.IntDesignationId).HasColumnName("intDesignationId");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntEmploymentTypeId).HasColumnName("intEmploymentTypeId");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntScheduleId).HasColumnName("intScheduleId");

                entity.Property(e => e.IntSupervisorId).HasColumnName("intSupervisorId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsFromRequisition).HasColumnName("isFromRequisition");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.StrApprovalStatus)
                    .HasMaxLength(50)
                    .HasColumnName("strApprovalStatus");

                entity.Property(e => e.StrDesignationName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDesignationName");

                entity.Property(e => e.StrEmail)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmail");

                entity.Property(e => e.StrEmployeeName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");

                entity.Property(e => e.StrEmploymentType)
                    .HasMaxLength(100)
                    .HasColumnName("strEmploymentType");

                entity.Property(e => e.StrGender)
                    .HasMaxLength(100)
                    .HasColumnName("strGender");

                entity.Property(e => e.StrPhoneNo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strPhoneNo");

                entity.Property(e => e.StrRejectionComments)
                    .HasMaxLength(250)
                    .HasColumnName("strRejectionComments");

                entity.Property(e => e.StrStatus)
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");

                entity.Property(e => e.StrTrainingName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTrainingName");
            });

            modelBuilder.Entity<TrainingSchedule>(entity =>
            {
                entity.HasKey(e => e.IntScheduleId);

                entity.ToTable("TrainingSchedule", "saas");

                entity.Property(e => e.IntScheduleId).HasColumnName("intScheduleId");

                entity.Property(e => e.DteActionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteActionDate");

                entity.Property(e => e.DteCourseCompletionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCourseCompletionDate");

                entity.Property(e => e.DteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteDate");

                entity.Property(e => e.DteExtentedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteExtentedDate");

                entity.Property(e => e.DteFromDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteFromDate");

                entity.Property(e => e.DteLastAssesmentSubmissionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteLastAssesmentSubmissionDate");

                entity.Property(e => e.DteRejectDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dteRejectDateTime");

                entity.Property(e => e.DteToDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteToDate");

                entity.Property(e => e.DteUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntBatchSize).HasColumnName("intBatchSize");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IntCurrentStage).HasColumnName("intCurrentStage");

                entity.Property(e => e.IntNextStage).HasColumnName("intNextStage");

                entity.Property(e => e.IntPipelineHeaderId).HasColumnName("intPipelineHeaderId");

                entity.Property(e => e.IntRejectedBy).HasColumnName("intRejectedBy");

                entity.Property(e => e.IntRequestedByEmp).HasColumnName("intRequestedByEmp");

                entity.Property(e => e.IntTrainingId).HasColumnName("intTrainingId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPipelineClosed).HasColumnName("isPipelineClosed");

                entity.Property(e => e.IsReject).HasColumnName("isReject");

                entity.Property(e => e.IsRequestedSchedule).HasColumnName("isRequestedSchedule");

                entity.Property(e => e.NumTotalDuration)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("numTotalDuration");

                entity.Property(e => e.StrApprovealStatus)
                    .HasMaxLength(100)
                    .HasColumnName("strApprovealStatus");

                entity.Property(e => e.StrBatchNo)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBatchNo");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(250)
                    .HasColumnName("strRemarks");

                entity.Property(e => e.StrResourcePersonName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strResourcePersonName");

                entity.Property(e => e.StrStatus)
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");

                entity.Property(e => e.StrTrainingName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTrainingName");

                entity.Property(e => e.StrVenue)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strVenue");
            });

            modelBuilder.Entity<TskBoard>(entity =>
            {
                entity.HasKey(e => e.IntBoardId);

                entity.ToTable("tskBoard", "saas");

                entity.Property(e => e.IntBoardId).HasColumnName("intBoardId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteEndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteEndDate");

                entity.Property(e => e.DteStartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteStartDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntFileUrlId).HasColumnName("intFileUrlId");

                entity.Property(e => e.IntProjectId).HasColumnName("intProjectId");

                entity.Property(e => e.IntReporterId).HasColumnName("intReporterId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrBackgroundColor)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strBackgroundColor");

                entity.Property(e => e.StrBoardName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strBoardName");

                entity.Property(e => e.StrDescription)
                    .IsRequired()
                    .HasColumnName("strDescription");

                entity.Property(e => e.StrHtmlColorCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strHtmlColorCode");

                entity.Property(e => e.StrPriority)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strPriority");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<TskGroupMember>(entity =>
            {
                entity.HasKey(e => e.IntGroupMemberId);

                entity.ToTable("tskGroupMember", "saas");

                entity.Property(e => e.IntGroupMemberId).HasColumnName("intGroupMemberId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAutoId).HasColumnName("intAutoId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntGroupMemberTypeId).HasColumnName("intGroupMemberTypeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.StrEmployeeName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");
            });

            modelBuilder.Entity<TskProject>(entity =>
            {
                entity.HasKey(e => e.IntProjectId);

                entity.ToTable("tskProject", "saas");

                entity.Property(e => e.IntProjectId).HasColumnName("intProjectId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteEndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteEndDate");

                entity.Property(e => e.DteStartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteStartDate");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntFileUrlId).HasColumnName("intFileUrlId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActivate).HasColumnName("isActivate");

                entity.Property(e => e.StrDescription).HasColumnName("strDescription");

                entity.Property(e => e.StrProjectName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strProjectName");

                entity.Property(e => e.StrStatus)
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");
            });

            modelBuilder.Entity<TskTaskDetail>(entity =>
            {
                entity.HasKey(e => e.IntTaskDetailsId);

                entity.ToTable("tskTaskDetails", "saas");

                entity.Property(e => e.IntTaskDetailsId).HasColumnName("intTaskDetailsId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntBoardId).HasColumnName("intBoardId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntProjectId).HasColumnName("intProjectId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrStatus)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strStatus");

                entity.Property(e => e.StrTaskDescription)
                    .IsRequired()
                    .HasColumnName("strTaskDescription");

                entity.Property(e => e.StrTaskTitle)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTaskTitle");
            });

            modelBuilder.Entity<Uom>(entity =>
            {
                entity.HasKey(e => e.IntUomId);

                entity.ToTable("Uom", "core");

                entity.Property(e => e.IntUomId).HasColumnName("intUomId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrUomName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("strUomName");
            });

            modelBuilder.Entity<Url>(entity =>
            {
                entity.HasKey(e => e.IntUrlId)
                    .HasName("PK__URLs__60EAF4A9086654AF");

                entity.ToTable("URLs", "core");

                entity.Property(e => e.IntUrlId)
                    .ValueGeneratedNever()
                    .HasColumnName("intUrlId");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrDomainName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDomainName");

                entity.Property(e => e.StrIpaddress)
                    .HasMaxLength(250)
                    .HasColumnName("strIPAddress");

                entity.Property(e => e.StrUrl)
                    .IsRequired()
                    .HasColumnName("strURL");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IntUserId)
                    .HasName("PK__Users__AE995DCE45940DF3");

                entity.ToTable("Users", "auth");

                entity.HasIndex(e => new { e.StrLoginId, e.IntAccountId, e.IntUrlId }, "unq_user")
                    .IsUnique();

                entity.Property(e => e.IntUserId).HasColumnName("intUserId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteLastLogin)
                    .HasColumnType("datetime")
                    .HasColumnName("dteLastLogin");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntRefferenceId).HasColumnName("intRefferenceId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntUrlId).HasColumnName("intUrlId");

                entity.Property(e => e.IntUserTypeId).HasColumnName("intUserTypeId");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsOfficeAdmin)
                    .HasColumnName("isOfficeAdmin")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsOwner).HasColumnName("isOwner");

                entity.Property(e => e.IsSuperuser)
                    .HasColumnName("isSuperuser")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StrDisplayName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strDisplayName");

                entity.Property(e => e.StrLoginId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strLoginId");

                entity.Property(e => e.StrOldPassword)
                    .HasMaxLength(250)
                    .HasColumnName("strOldPassword");

                entity.Property(e => e.StrPassword)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strPassword");
            });

            modelBuilder.Entity<UserGroupHeader>(entity =>
            {
                entity.HasKey(e => e.IntUserGroupHeaderId)
                    .HasName("PK_UserGroup");

                entity.ToTable("UserGroupHeader", "auth");

                entity.Property(e => e.IntUserGroupHeaderId).HasColumnName("intUserGroupHeaderId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("strCode");

                entity.Property(e => e.StrRemarks)
                    .HasMaxLength(2000)
                    .HasColumnName("strRemarks");

                entity.Property(e => e.StrUserGroup)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strUserGroup");
            });

            modelBuilder.Entity<UserGroupRow>(entity =>
            {
                entity.HasKey(e => e.IntUserGroupRowId);

                entity.ToTable("UserGroupRow", "auth");

                entity.Property(e => e.IntUserGroupRowId).HasColumnName("intUserGroupRowId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntEmployeeId).HasColumnName("intEmployeeId");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IntUserGroupHeaderId).HasColumnName("intUserGroupHeaderId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrEmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("strEmployeeName");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.IntRoleId);

                entity.ToTable("UserRole", "auth");

                entity.Property(e => e.IntRoleId).HasColumnName("intRoleId");

                entity.Property(e => e.DteCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteCreatedAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DteUpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("dteUpdatedAt");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntCreatedBy).HasColumnName("intCreatedBy");

                entity.Property(e => e.IntUpdatedBy).HasColumnName("intUpdatedBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsDefault).HasColumnName("isDefault");

                entity.Property(e => e.StrRoleName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strRoleName");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.HasKey(e => e.IntUserTypeId)
                    .HasName("PK__UserType__C074F9B715AB488D");

                entity.ToTable("UserType", "auth");

                entity.HasIndex(e => e.StrUserType, "UQ__UserType__A327F2893B858DDC")
                    .IsUnique();

                entity.Property(e => e.IntUserTypeId).HasColumnName("intUserTypeId");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StrUserType)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strUserType");
            });

            modelBuilder.Entity<training>(entity =>
            {
                entity.HasKey(e => e.IntTrainingId);

                entity.ToTable("Training", "saas");

                entity.Property(e => e.IntTrainingId).HasColumnName("intTrainingId");

                entity.Property(e => e.DteActionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dteActionDate");

                entity.Property(e => e.IntAccountId).HasColumnName("intAccountId");

                entity.Property(e => e.IntActionBy).HasColumnName("intActionBy");

                entity.Property(e => e.IntBusinessUnitId).HasColumnName("intBusinessUnitId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.StrTrainingCode)
                    .HasMaxLength(100)
                    .HasColumnName("strTrainingCode");

                entity.Property(e => e.StrTrainingName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("strTrainingName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
