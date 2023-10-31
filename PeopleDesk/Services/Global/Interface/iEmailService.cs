using PeopleDesk.Models.Global;

namespace PeopleDesk.Services.Global.Interface
{
    public interface iEmailService
    {
        Task<string> SendEmail(string strProfile_name, string strEmailAdd, string strCCEmailAdd, string strBCEmailAdd,
            string strSubject, string strBody, string strHTML);

        Task<bool> SendMail(SendMail obj);
    }
}
