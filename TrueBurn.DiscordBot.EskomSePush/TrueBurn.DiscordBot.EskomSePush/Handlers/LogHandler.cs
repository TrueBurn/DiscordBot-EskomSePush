using Discord;

namespace TrueBurn.DiscordBot.EskomSePush.Handlers;

internal class LogHandler : ILogHandler
{
    public Task ClientLogHandler(LogMessage logMessage)
    {
        Console.WriteLine(logMessage.Message);
        return Task.CompletedTask;
    }
}

internal interface ILogHandler
{
    Task ClientLogHandler(LogMessage logMessage);
}