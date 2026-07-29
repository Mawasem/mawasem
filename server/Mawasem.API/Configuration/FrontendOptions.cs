namespace Mawasem.API.Configuration;

public sealed class FrontendOptions
{
    public const string SectionName =
        "Frontend";

    public string[] AllowedOrigins { get; set; } =
        Array.Empty<string>();
}