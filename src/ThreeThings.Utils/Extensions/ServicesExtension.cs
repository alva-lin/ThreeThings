using System.Reflection;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerGen;

using ThreeThings.Utils.Attributes;
using ThreeThings.Utils.Common;
using ThreeThings.Utils.Enums;
using ThreeThings.Utils.Options;

namespace ThreeThings.Utils.Extensions;

public static class ServicesExtension
{
    /// <summary>
    /// 所有加载的程序集
    /// </summary>
    private static Assembly[] AllAssemblies => AppDomain.CurrentDomain.GetAssemblies().OrderBy(assembly => assembly.FullName).ToArray();

    private static Type[] AllTypes => AllAssemblies.SelectMany(assembly => assembly.GetTypes()).ToArray();

    public static Type[] AllNormalTypes => AllTypes.Where(type => type.IsClass && !type.IsGenericType && !type.IsAbstract).ToArray();

    public static void IncludeAllXmlComments(this SwaggerGenOptions options)
    {
        var basePath = Directory.GetParent(Environment.CurrentDirectory);
        if (basePath is { Exists: true })
        {
            var currentAssembly = Assembly.GetCallingAssembly();
            var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union(new[] { currentAssembly.GetName() })
                .Select(a => Path.Combine(basePath.FullName, a.Name!, $"{a.Name}.xml"))
                .Where(File.Exists)
                .ToArray();
            Array.ForEach(xmlDocs, s => options.IncludeXmlComments(s));
        }
    }

    /// <summary>
    ///     根据 <see cref="LifeScopeAttribute" /> 来注册服务
    /// </summary>
    public static IServiceCollection AddBasicServiceByLifeScope(this IServiceCollection services)
    {
        var types = AllNormalTypes
            .Where(type => type.GetInterface(nameof(IBasicService)) != null)
            .ToList();

        foreach (var type in types)
        {
            var lifeScope = type.GetCustomAttribute<LifeScopeAttribute>()?.Scope ?? LifeScope.Transient;


            var implements = type.GetInterfaces()
                .Where(iType => !iType.IsGenericType && iType.Name.EndsWith("Service"))
                .Union(new[] { type });
            foreach (var implement in implements)
            {
                var iScope = implement.GetCustomAttribute<LifeScopeAttribute>()?.Scope ?? lifeScope;

                switch (iScope)
                {
                    case LifeScope.Transient:
                        services.AddTransient(implement, type);
                        break;
                    case LifeScope.Scope:
                        services.AddScoped(implement, type);
                        break;
                    case LifeScope.Singleton:
                        services.AddSingleton(implement, type);
                        break;
                }
            }
        }

        return services;
    }

    public static IServiceCollection AddJwtBearer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.Configure<JwtOption>(nameof(JwtOption), configuration);

        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        return services;
    }

    public static IServiceCollection AddCorsSetting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorsOption>(nameof(CorsOption), configuration);

        services.AddCors(options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var corsOption      = serviceProvider!.GetRequiredService<IOptions<CorsOption>>().Value;

            options.AddPolicy("Develop", builder =>
            {
                builder.WithOrigins(corsOption.AllowOrigins);
            });
        });

        return services;
    }
}
