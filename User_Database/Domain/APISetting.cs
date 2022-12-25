namespace User_Database.Domain
{
    public class APISetting
    {
        public static string? UserDBConnection { get; set; }
        public static string? HangFireDBConnection { get; set; }
        public static string? LogDBConnection { get; set; }
        public static string? CORSAllowOrigin { get; set; }

        public static Jwt? Jwt { get; set; }
        public static CacheSetting? CacheConfiguration { get; set; }
        public static string? WWWRoot { get; set; }
    }

    public class Jwt
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
    }

    public class CacheSetting
    {
        public string? EnableCache { get; set; }
        public string? CacheURL { get; set; }
        public double AbsoluteExpirationInHours { get; set; }
        public double SlidingExpirationInMinutes { get; set; }
    }
}