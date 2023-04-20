using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrueBurn.DiscordBot.EskomSePush.Handlers;
using TrueBurn.DiscordBot.EskomSePush.Options;
using TrueBurn.DiscordBot.EskomSePush.Scaffolding;

// Configure your services.
await using ServiceProvider serviceProvider = ServiceCollectionSetup.ConfigureServices();

// Retrieve client and Log handler instances via the provider.
DiscordSocketClient client = serviceProvider.GetRequiredService<DiscordSocketClient>();

#region Log Handler

ILogHandler logHandler = serviceProvider.GetRequiredService<ILogHandler>();

client.Log += logHandler.ClientLogHandler;
serviceProvider.GetRequiredService<CommandService>().Log += logHandler.ClientLogHandler;

#endregion Log Handler

BotOptions botOptions = serviceProvider.GetRequiredService<IOptions<BotOptions>>().Value;

#region Login and Connect

await client.LoginAsync(TokenType.Bot, botOptions.Token);
await client.StartAsync();

#endregion Login and Connect

#region Commands

CommandService commandService = serviceProvider.GetRequiredService<CommandService>();
await commandService.AddModulesAsync(typeof(Program).Assembly, serviceProvider);

#endregion Commands

#region Event Handlers

IReadyHandler readyHandler = serviceProvider.GetRequiredService<IReadyHandler>();
client.Ready += readyHandler.ClientReadyHandler;

IJoinedGuildHandler joinedGuildHandler = serviceProvider.GetRequiredService<IJoinedGuildHandler>();
client.JoinedGuild += joinedGuildHandler.ClientJoinedGuildHandler;

IMessageReceivedHandler messageReceivedHandler = serviceProvider.GetRequiredService<IMessageReceivedHandler>();
client.MessageReceived += messageReceivedHandler.ClientMessageReceivedHandler;

#endregion Event Handlers

// Block this task until the program is closed.
await Task.Delay(-1);