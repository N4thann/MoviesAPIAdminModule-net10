namespace MoviesAPIAdminModule.JwtConfigurationsOptions
{
    public class JwtConfigurationsOptions
    {
        public const string JWT = "JWT";

        public string ValidAudience { get; set; } = string.Empty;
        public string ValidIssuer { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInMinutes { get; set; }
    }
}
