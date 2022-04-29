using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Security
{
    public class TokenResponse
    {
        public const string DefaultTokenType = "Bearer";

        /// <summary>
        /// The access token string as issued by the authorization server.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// The type of token this is, typically just the string "Bearer".
        /// </summary>
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        /// <summary>
        /// If the access token expires, the server should reply with the duration of time the access token is granted for.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int? ExpiresInSeconds { get; set; }

        /// <summary>
        /// If the access token expires, the server should reply with the duration of time the access token is granted for.
        /// </summary>
        [JsonIgnore]
        public TimeSpan? ExpiresIn
        {
            get => ExpiresInSeconds is null ? null : TimeSpan.FromSeconds((int)ExpiresInSeconds);
            set => ExpiresInSeconds = value is null ? null : Convert.ToInt32(((TimeSpan)value).TotalSeconds);
        }

        /// <summary>
        /// If the access token will expire, then it is useful to return a refresh token which applications can use to obtain another access token. 
        /// However, tokens issued with the implicit grant cannot be issued a refresh token.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// If the scope the user granted is identical to the scope the app requested, this parameter is optional. 
        /// If the granted scope is different from the requested scope, such as if the user modified the scope, then this parameter is required.
        /// </summary>
        public string? Scope { get; set; }
    }
}
