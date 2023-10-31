using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;

namespace PeopleDesk.Services.Jwt
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, User user);
        Task<Token> GenerateJSONWebToken(string key, string issuer, User user, long? empId, bool? isOfficeAdmin, bool? isOwner, int? isSupNLMORManagement, string? businessUnitList, string? workplaceGroupList, string? workplaceList,
            string? wings, string? soleDepos, String? regions, string? areas, string? territories, long? businessUnitId, long? workplaceGroupId);
        bool IsTokenValid(string key, string issuer, string token);
        Task<Token> GenerateNewToken(Token token, string key, string issuer);
        Task<bool> UpdateRefreshToken(string strLoginId, string strRefreshToken);
    }
}
