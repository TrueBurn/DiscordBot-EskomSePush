using Discord;
using Discord.Commands;

namespace TrueBurn.DiscordBot.EskomSePush.Modules;

public class HelpCommand : ModuleBase<SocketCommandContext>
{

    [Command("help")]
    [Summary("Provide help on the bot commands")]
    public async Task ProvideHelp()
    {

        EmbedBuilder builder = new()
        {
            Title = "Help",
            Color = Color.Orange,
            Description = "This is the help page for the Eskom Se Push Bot"
        };

        builder.AddField("$$status", "Get the current load shedding status. Pass in 'true' to ensure you get the most recent status");
        builder.AddField("$$help", "Get help on the bot commands");

        await ReplyAsync("", false, builder.Build());

    }

}
