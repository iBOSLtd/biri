using PeopleDesk.Models;
using PeopleDesk.Models.MasterData;

namespace PeopleDesk.Services.MasterData.Interfaces
{
    public interface IOrganogramService
    {
        Task<OrganogramTreeViewModel> GetOrganogramTree(long businessUnitId);
        Task<MessageHelper> OrganogramReConstruct(List<OrganogramReConstructViewModel> objList);
    }
}
