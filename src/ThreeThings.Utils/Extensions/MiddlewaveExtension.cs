using Microsoft.AspNetCore.Builder;

using ThreeThings.Utils.Middleware;

namespace ThreeThings.Utils.Extensions;

public static class MiddlewaveExtension
{
    public static IApplicationBuilder UseBasicException(this IApplicationBuilder host)
    {
        host.UseMiddleware<BasicExceptionMiddleware>();

        return host;
    }
}
