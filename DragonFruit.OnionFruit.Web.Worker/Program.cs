// OnionFruit™ Web Copyright DragonFruit Network <inbox@dragonfruit.network>
// Licensed under Apache-2. Refer to the LICENSE file for more info

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DnsClient;
using DragonFruit.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Modeling;
using StackExchange.Redis;

namespace DragonFruit.OnionFruit.Web.Worker;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(ConfigureHost)
            .ConfigureLogging((host, logging) => ConfigureLogging(logging, host.Configuration, "Worker"))
            .ConfigureServices(ConfigureServices)
            .Build();

        using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            await ValidateRedisStructures(scope.ServiceProvider).ConfigureAwait(false);
        }

        await host.RunAsync().ConfigureAwait(false);
    }

    private static void ConfigureHost(IConfigurationBuilder host)
    {
        // windows containers don't allow file mounts, so we allow setting an environment var to the config file path
        var configBase = Environment.GetEnvironmentVariable("CONFIG_FOLDER_PATH");
        if (string.IsNullOrEmpty(configBase))
        {
            return;
        }

        var fullConfigBasePath = Path.GetFullPath(configBase);
        if (Directory.Exists(fullConfigBasePath))
        {
            host.SetBasePath(fullConfigBasePath);
        }
    }

    public static void ConfigureLogging(ILoggingBuilder logging, IConfiguration config, string dsnType)
    {
        logging.ClearProviders();
        logging.AddSimpleConsole(o =>
        {
            o.SingleLine = true;
            o.IncludeScopes = false;
            o.TimestampFormat = $"[dd/MM/yyyy hh:mm:ss] ({dsnType}) ";
        });

#if WINDOWS
        logging.AddEventLog(o =>
        {
            o.Filter = (_, level) => level >= LogLevel.Information;
            o.SourceName = $"OnionFruit-Web-{dsnType}/v{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}";
        });
#endif

        logging.AddSentry(o =>
        {
            o.Dsn = config[$"{dsnType}:Dsn"] ?? config["Dsn"];
            o.Release = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);

            o.MaxBreadcrumbs = 50;
            o.MinimumEventLevel = LogLevel.Error;
            o.MinimumBreadcrumbLevel = LogLevel.Debug;
        });
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // redis + redis.om
        services.AddSingleton(RedisClientConfigurator.CreateConnectionMultiplexer(context.Configuration, true));
        services.AddSingleton<IRedisConnectionProvider>(s => new RedisConnectionProvider(s.GetRequiredService<IConnectionMultiplexer>()));

        // api
        services.AddSingleton<ILookupClient, LookupClient>();
        services.AddSingleton<ApiClient, WorkerApiClient>();

        // timed service
        services.AddHostedService<Worker>();
    }

    private static async Task ValidateRedisStructures(IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        var redis = services.GetRequiredService<IRedisConnectionProvider>();
        var logger = services.GetRequiredService<ILogger<IRedisConnectionProvider>>();

        var createNewIndexes = config["REDIS_CREATE_INDEXES"]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? true;
        var regenExistingIndexes = config["REDIS_REGEN_INDEXES"]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

        var documentSourceAssemblies = new[]
        {
            Assembly.GetExecutingAssembly()
        };

        foreach (var type in documentSourceAssemblies.SelectMany(x => x.ExportedTypes).Where(x => x.GetCustomAttribute<DocumentAttribute>() != null))
        {
            // drop index if regenerating
            if (regenExistingIndexes)
            {
                logger.LogInformation("Dropping index for type {t}", type.Name);
                await redis.Connection.DropIndexAsync(type).ConfigureAwait(false);
            }

            // create if enabled OR the index is being regenerated
            if (regenExistingIndexes || createNewIndexes)
            {
                logger.LogInformation("Initialising index for type {t} (if not already present)", type.Name);
                await redis.Connection.CreateIndexAsync(type).ConfigureAwait(false);
            }
        }
    }
}