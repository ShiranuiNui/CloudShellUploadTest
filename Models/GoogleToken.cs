using System.Text.Json.Serialization;

namespace CloudShell_Test.Models
{
    public class GoogleToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string Scope { get; set; }
        public string TokenType { get; set; }
        public string IdToken { get; set; }


    }
}