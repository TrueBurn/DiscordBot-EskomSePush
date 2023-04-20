using Discord;
using Discord.WebSocket;

namespace TrueBurn.DiscordBot.EskomSePush.Handlers;
internal class JoinedGuildHandler : IJoinedGuildHandler
{
    private readonly DiscordSocketClient _client;
    private readonly ILogHandler _logHandler;

    public JoinedGuildHandler(DiscordSocketClient client, ILogHandler logHandler)
    {
        _client = client;
        _logHandler = logHandler;
    }

    public async Task ClientJoinedGuildHandler(SocketGuild guild)
    {

        foreach (SocketTextChannel? channel in guild.TextChannels.OrderBy(x => x.Position))
        {

            if (channel is null)
            {
                continue;
            }

            OverwritePermissions botPerms = channel.GetPermissionOverwrite(_client.CurrentUser).GetValueOrDefault();

            if (botPerms.SendMessages == PermValue.Deny)
            {
                continue;
            }

            try
            {
                await channel.SendMessageAsync("Hello, I am EskomSePush. I will notify you when Eskom has a power outage in your area. To get started, type $$help");
                return;
            }
            catch (Exception Ex)
            { 
                await _logHandler.ClientLogHandler(new LogMessage(LogSeverity.Error, "JoinedGuildHandler", Ex.Message, Ex));
            }
        }

    }
}

internal interface IJoinedGuildHandler
{
    Task ClientJoinedGuildHandler(SocketGuild guild);
}
