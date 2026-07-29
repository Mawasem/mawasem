namespace Mawasem.API.Configuration;

public sealed class ApiSecurityOptions
{
    public const string SectionName =
        "Security";

    public const int DefaultGeneralRequestsPerMinute = 120;

    public const int DefaultAuthenticationRequestsPerMinute = 10;

    public const int DefaultSensitiveRequestsPerMinute = 30;

    public const long DefaultMaximumRequestBodySizeBytes =
        10 * 1024 * 1024;

    public int GeneralRequestsPerMinute { get; set; } =
        DefaultGeneralRequestsPerMinute;

    public int AuthenticationRequestsPerMinute { get; set; } =
        DefaultAuthenticationRequestsPerMinute;

    public int SensitiveRequestsPerMinute { get; set; } =
        DefaultSensitiveRequestsPerMinute;

    public long MaximumRequestBodySizeBytes { get; set; } =
        DefaultMaximumRequestBodySizeBytes;
}