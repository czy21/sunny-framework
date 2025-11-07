using System.Text.Encodings.Web;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nacos.V2.DependencyInjection;
using NLog.Web;
using Refit;
using Sunny.Framework.Core.Json;
using Sunny.Framework.NLog;

namespace Sunny.Framework.Web;

public static class WebConfigure
{
    public static IHostBuilder UseHostConfigure(this IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)((_, cfb) =>
        {
            var configurationRoot = cfb.Build();
            var nacosConfigEnabled = configurationRoot.GetValue("Nacos:Config:Enabled", false);
            if (nacosConfigEnabled)
            {
                cfb.AddNacosV2Configuration(configurationRoot.GetSection("Nacos:Config"), parser: null, logAction: null);
            }
        }));

        builder.ConfigureLogging(c => c.ClearProviders()).UseNLog();

        builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        return builder;
    }

    public static IServiceCollection AddHostConfigure(this IServiceCollection services, IConfiguration config)
    {
        var nacosConfigEnabled = config.GetValue("Nacos:Config:Enabled", false);
        if (nacosConfigEnabled)
        {
            services.AddNacosV2Config(config, null, "Nacos:Config");
        }
        
        services.AddHostedService<NLogConfigListener>();
        return services;
    } 
    
    public static IServiceCollection AddWebConfigure(this IServiceCollection services, IConfiguration config)
    {
        AddHostConfigure(services, config);
        
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.CombineLogs = true;
        });
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            options.JsonSerializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParse("yyyy-MM-dd HH:mm:ss"));
        });

        services.AddSingleton(provider =>
        {
            var jsonOptions = provider.GetService<IOptions<JsonOptions>>();
            return new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(jsonOptions.Value.JsonSerializerOptions)
            };
        });

        services.AddHealthChecks();

        return services;
    }

    public static IEndpointConventionBuilder UseWebHealthCheck(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapHealthChecks("/actuator/health");
    }
}