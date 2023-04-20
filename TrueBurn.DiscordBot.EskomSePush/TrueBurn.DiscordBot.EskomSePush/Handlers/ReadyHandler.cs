using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TrueBurn.DiscordBot.EskomSePush.Options;

namespace TrueBurn.DiscordBot.EskomSePush.Handlers;

internal class ReadyHandler : IReadyHandler
{

    private readonly DiscordSocketClient _client;
    private readonly BotOptions _botOptions;

    public ReadyHandler(DiscordSocketClient client, IOptions<BotOptions> botOptions)
    {
        _client = client;
        _botOptions = botOptions.Value;
    }

    public async Task ClientReadyHandler()
    {

        UserStatus userStatus = UserStatus.Online;

        await _client.SetStatusAsync(userStatus);
        Console.WriteLine($"{DateTime.UtcNow} | Status set | {userStatus}");

        ActivityType activity = ActivityType.Playing;
        string statusText = "with South Africa's future";

        await _client.SetGameAsync(statusText,  type: activity);
        Console.WriteLine($"{DateTime.UtcNow} | Game set | {activity}: {statusText}");

    }

}

internal interface IReadyHandler
{
    Task ClientReadyHandler();
}
