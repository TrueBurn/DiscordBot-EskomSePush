namespace TrueBurn.DiscordBot.EskomSePush.Options;

public class EskomSePushOptions
{

    public const string SectionName = "Services:EskomSePush";
    public string BaseUrl { get; set; } = "https://developer.sepush.co.za";
    public EskomSePushEndpoints Endpoints { get; set; } = new();
    public string? Token { get; set; }

}

public class EskomSePushEndpoints
{
    public string Status { get; set; } = "business/2.0/status";
    public string ApiAllowance { get; set; } = "business/2.0/api_allowance";
}
