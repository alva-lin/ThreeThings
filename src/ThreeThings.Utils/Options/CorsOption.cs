namespace ThreeThings.Utils.Options;

/// <summary>
/// Cors 配置
/// </summary>
public class CorsOption
{
    public string[] AllowOrigins { get; set; } = Array.Empty<string>();
}
