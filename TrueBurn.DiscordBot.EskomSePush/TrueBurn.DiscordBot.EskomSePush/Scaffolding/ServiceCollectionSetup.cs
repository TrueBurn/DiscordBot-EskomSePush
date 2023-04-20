using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueBurn.DiscordBot.EskomSePush.Handlers;
using TrueBurn.DiscordBot.EskomSePush.Options;
using TrueBurn.DiscordBot.EskomSePush.Services;

namespace TrueBurn.DiscordBot.EskomSePush.Scaffolding;

public static class ServiceCollectionSetup
{

    public static ServiceProvider ConfigureServices()
    {
        ServiceCollection services = new();

        services.ConfigureBotOptions();

        services.AddMemoryCache();

        services
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
                MessageCacheSize = 500,
                LogLevel = LogSeverity.Info
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            }));

        services.AddHandlers();
        services.AddEskomSePushService();

        return services.BuildServiceProvider();
    }

    private static IServiceCollection ConfigureBotOptions(this IServiceCollection services)
    {
        IConfigurationBuilder configBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        IConfigurationRoot config = configBuilder.Build();

        services
            .AddOptions()
            .Configure<EskomSePushOptions>(options =>
            {
                options.Token = config[$"{EskomSePushOptions.SectionName}:Token"] ?? throw new InvalidOperationException("ESP API token missing");
            })
            .Configure<BotOptions>(options =>
            {
                options.Token = config[$"{BotOptions.SectionName}:Token"] ?? throw new InvalidOperationException("Discord Bot token missing");
            });

        return services;
    }

    private static IServiceCollection AddHandlers(this IServiceCollection services)
    {
         services
            .AddSingleton<IJoinedGuildHandler, JoinedGuildHandler>()
            .AddSingleton<ILogHandler, LogHandler>()
            .AddSingleton<IMessageReceivedHandler, MessageReceivedHandler>()
            .AddSingleton<IReadyHandler, ReadyHandler>();

        return services;
    }

    private static IServiceCollection AddEskomSePushService(this IServiceCollection services)
    {
        services
            .AddSingleton<IEskomSePushService, EskomSePushService>()
            .AddHttpClient(nameof(EskomSePushService));

        return services;
    }

}
