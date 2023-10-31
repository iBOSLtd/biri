using Microsoft.Data.SqlClient;
using PeopleDesk.Helper;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.Global.Interface;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace PeopleDesk.Services.Global
{
    public class EmailService : iEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> SendEmail(string strProfile_name, string strEmailAdd, string strCCEmailAdd, string strBCEmailAdd,
            string strSubject, string strBody, string strHTML)
        {
            try
            {
                var connection = new SqlConnection(Connection.iPEOPLE_HCM);
                string sql = "saas.sprSendEmail";
                SqlCommand sqlCmd = new SqlCommand(sql, connection);
                sqlCmd.CommandType = CommandType.StoredProcedure;

                sqlCmd.Parameters.AddWithValue("@strProfile_name", strProfile_name);
                sqlCmd.Parameters.AddWithValue("@strEmailAdd", strEmailAdd);
                sqlCmd.Parameters.AddWithValue("@strCCEmailAdd", strCCEmailAdd);
                sqlCmd.Parameters.AddWithValue("@strBCEmailAdd", strBCEmailAdd);
                sqlCmd.Parameters.AddWithValue("@strSubject", strSubject);
                sqlCmd.Parameters.AddWithValue("@strBody", strBody);
                sqlCmd.Parameters.AddWithValue("@strHTML", strHTML);
                connection.Open();
                sqlCmd.ExecuteNonQuery();
                connection.Close();

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public async Task<bool> SendMail(SendMail obj)
        {
            try
            {
                // Mail
                string userName = _configuration["EmailConfiguration:Username"];
                string password = _configuration["EmailConfiguration:Password"];
                string host = _configuration["EmailConfiguration:SmtpServer"];
                int port = int.Parse(_configuration["EmailConfiguration:Port"]);
                string mailFrom = _configuration["EmailConfiguration:From"];

                obj.SendBy = mailFrom;
                using (var client = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = userName,
                        Password = password
                    };

                    client.Credentials = credential;
                    client.Host = host;
                    client.Port = port;
                    client.EnableSsl = true;

                    using (var emailMessage = new MailMessage())
                    {
                        emailMessage.To.Add(new MailAddress(obj.SendTo));
                        emailMessage.From = new MailAddress(obj.SendBy);
                        emailMessage.Subject = obj.MailSubject;
                        emailMessage.Body = obj.MailBody;
                        emailMessage.IsBodyHtml = (bool)obj.IsHtmlFormat;
                        client.Send(emailMessage);
                    }
                }
                await Task.CompletedTask;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
