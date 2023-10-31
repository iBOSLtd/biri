namespace PeopleDesk.Models
{
    public class TokenViewModel
    {
        public string? access_token { get; set; }
        public int? expires_in { get; set; }
        public string? token_type { get; set; }
        public long? orgId { get; set; }
        public string? username { get; set; }
        public string? useid { get; set; }

    }
    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
