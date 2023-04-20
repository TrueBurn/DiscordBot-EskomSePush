using System.Text.Json;
using FluentDate;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TrueBurn.DiscordBot.EskomSePush.Models.ESP;
using TrueBurn.DiscordBot.EskomSePush.Options;

namespace TrueBurn.DiscordBot.EskomSePush.Services;

public class EskomSePushService : IEskomSePushService
{

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly EskomSePushOptions _options;

    private const string CACHE_KEY_GET_LOAD_SHEDDING_STATUS = "EskomSePushService.GetLoadSheddingStatus";

    public EskomSePushService(IHttpClientFactory httpClientFactory, IOptions<EskomSePushOptions> eskomSePushOptions, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _options = eskomSePushOptions.Value;
    }

    public async Task<LoadSheddingStatus> GetLoadSheddingStatus(bool bypassCache = false)
    {

        if (bypassCache)
        {
            _memoryCache.Remove(CACHE_KEY_GET_LOAD_SHEDDING_STATUS);
        }

        LoadSheddingStatus? loadSheddingStatus = await _memoryCache.GetOrCreateAsync(
            CACHE_KEY_GET_LOAD_SHEDDING_STATUS,
            async cacheEntry =>
            {

                LoadSheddingStatus loadSheddingStatus = await GetCurrentLoadSheddingStatus();

                // Cache for 2 hours rounded up to the nearest hour
                cacheEntry.AbsoluteExpiration = DateTimeOffset.UtcNow + 2.Hours();
                

                return loadSheddingStatus;

            }
        );

        return loadSheddingStatus ?? throw new Exception("Unable to get load shedding status");
    }

    private async Task<LoadSheddingStatus> GetCurrentLoadSheddingStatus()
    {
        using HttpClient client = _httpClientFactory.CreateClient(nameof(EskomSePushService));
        using HttpRequestMessage request = CreateRequestMessage(HttpMethod.Get, _options.Endpoints.Status);
        using HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        LoadSheddingStatus? loadSheddingStatus = JsonSerializer.Deserialize<LoadSheddingStatus>(content);
        return loadSheddingStatus ?? throw new Exception("Failed to deserialize response");
    }

    private HttpRequestMessage CreateRequestMessage(HttpMethod method, string endpointPath)
    {
        HttpRequestMessage request = new(method, new Uri($"{_options.BaseUrl}/{endpointPath}"));
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Token", _options.Token);
        return request;
    }

}

public interface IEskomSePushService
{
    Task<LoadSheddingStatus> GetLoadSheddingStatus(bool bypassCache = false);
}
