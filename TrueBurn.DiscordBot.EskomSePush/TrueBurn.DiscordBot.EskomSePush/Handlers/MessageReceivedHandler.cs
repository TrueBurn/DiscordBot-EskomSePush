using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TrueBurn.DiscordBot.EskomSePush.Options;

namespace TrueBurn.DiscordBot.EskomSePush.Handlers;

internal class MessageReceivedHandler : IMessageReceivedHandler
{

    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly BotOptions _botOptions;

    private readonly IServiceProvider _services;

    public MessageReceivedHandler(IServiceProvider services, IOptions<BotOptions> botOptions, DiscordSocketClient client, CommandService commandService)
    {
        _services = services;
        _client = client;
        _commands = commandService;
        _botOptions = botOptions.Value;
    }

    public async Task ClientMessageReceivedHandler(SocketMessage rawMessage)
    {
        if (rawMessage.Author.IsBot || rawMessage is not SocketUserMessage message || message.Channel is IDMChannel)
        {
            return;
        }

        SocketCommandContext context = new(_client, message);

        int argPos = 0;

        if (message.HasStringPrefix(_botOptions.Prefix, ref argPos) ||
            message.HasMentionPrefix(_client.CurrentUser, ref argPos))
        {
            // Execute the command.
            IResult? result = await _commands.ExecuteAsync(context, argPos, _services);

            // If the command failed, notify the user.
            if (!result.IsSuccess && result.Error.HasValue)
            {
                await context.Channel.SendMessageAsync($":x: {result.ErrorReason}");
            }
        }

    }
}

internal interface IMessageReceivedHandler
{
    Task ClientMessageReceivedHandler(SocketMessage message);
}