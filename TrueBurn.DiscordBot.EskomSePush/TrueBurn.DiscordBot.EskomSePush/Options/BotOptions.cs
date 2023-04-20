namespace TrueBurn.DiscordBot.EskomSePush.Options;

public class BotOptions
{

    public const string SectionName = "Discord:Bot";

    public string? Token { get; set; }
    public string Prefix { get; set; } = "$$";
}
