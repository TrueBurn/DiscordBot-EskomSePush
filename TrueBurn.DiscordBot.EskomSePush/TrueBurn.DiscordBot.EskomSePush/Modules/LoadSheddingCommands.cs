using Discord;
using Discord.Commands;
using Humanizer;
using TrueBurn.DiscordBot.EskomSePush.Models.ESP;
using TrueBurn.DiscordBot.EskomSePush.Services;

namespace TrueBurn.DiscordBot.EskomSePush.Modules;

public class LoadSheddingCommands : ModuleBase<SocketCommandContext>
{

    private readonly IEskomSePushService _eskomSePushService;

    public LoadSheddingCommands(IEskomSePushService eskomSePushService)
    {
        _eskomSePushService = eskomSePushService;
    }

    [Command("status")]
    [Summary("Get the current load shedding status")]
    public async Task GetLoadSheddingStatus([Remainder] bool bypassCache = false)
    {

        LoadSheddingStatus lsStatus = await _eskomSePushService.GetLoadSheddingStatus(bypassCache);

        EmbedBuilder builder = new()
        {
            Title = "Stage Information",
            Color = Color.Blue,
            Description = "This is the current stage and next stages for each city as a timeline"
        };


        builder.AddField("Cape Town", ConstructStageTimeline(lsStatus.Status.CapeTown));
        builder.AddField("Eskom", ConstructStageTimeline(lsStatus.Status.Eskom));

        await ReplyAsync("", false, builder.Build());
    }

    private static string ConstructStageTimeline(Supplier supplier)
    {
        string currentStage = supplier.Stage;
        string nextStages = string.Join("\n", supplier.NextStages.Select(ns => $"• Stage {ns.Stage} ({ns.StageStartTimestamp.Humanize()})"));
        string timelineMessage = $"**Current Stage**: {currentStage}\n**Next Stages**:\n{nextStages}";
        return timelineMessage;
    }
       

}
