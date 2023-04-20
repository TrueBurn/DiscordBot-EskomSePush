using System.Text.Json.Serialization;

namespace TrueBurn.DiscordBot.EskomSePush.Models.ESP;

public class LoadSheddingStatus
{
    [JsonPropertyName("status")]
    public Status Status { get; set; } = null!;

}

public class Status
{
    [JsonPropertyName("capetown")]
    public Supplier CapeTown { get; set; } = null!;
    [JsonPropertyName("eskom")] 
    public Supplier Eskom { get; set; } = null!;
}

public class NextStage
{
    [JsonPropertyName("stage")]
    public string Stage { get; set; } = null!;
    [JsonPropertyName("stage_start_timestamp")]
    public DateTime StageStartTimestamp { get; set; }
}

public class Supplier
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("next_stages")]
    public List<NextStage> NextStages { get; set; } = new();
    [JsonPropertyName("stage")]
    public string Stage { get; set; } = null!;
    [JsonPropertyName("stage_updated")]
    public DateTime StageUpdated { get; set; }
}
