using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Data;
using PeopleDesk.Models.Employee;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface IPayrollService
    {
        Task<BankAdviceLanding> BankAdvices(BankAdviceHeader obj, long AccountId);

        Task<dynamic> GetSoledepoDdlByWorkplaceGroup(long AccountId, long BusinessUnitId, long WorkplaceGroup,BaseVM tokenData);
        Task<dynamic> GetAreaBySoleDepoId(long AccountId, long BusinessUnitId, long WorkplaceGroup, long soleDepo, BaseVM tokenData);

    }
}
