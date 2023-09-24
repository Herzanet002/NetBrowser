using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Exceptions;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Services;

public class NewsApiClientService : INewsApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly AppConfigService _appConfigService;

    public NewsApiClientService(IHttpClientFactory httpClientFactory, AppConfigService appConfigService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _appConfigService = appConfigService;
    }

    public async Task<IEnumerable<ContentModel>> GetNewsAsync(int pageSize, int pageIndex)
    {
        try
        {
            var newsApiSetting = _appConfigService.GetSection<NewsApiSetting>(nameof(NewsApiSetting));
            var connection = $"{newsApiSetting.Connection}/news?pageSize={pageSize}&pageIndex={pageIndex}";
            var response = await _httpClient.GetAsync(connection);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var body = await response.Content.ReadAsByteArrayAsync();
            var content = JsonSerializer.Deserialize<IEnumerable<ContentModel>>(body);
            return content;
        }
        catch (Exception ex)
        {
            throw new NewsApiException(ex.Message, ex);
        }
    }

    public async Task<IEnumerable<ContentModel>> GetNewsByProvidersAsync(IEnumerable<NewsProvider> providers,
        int pageSize, int pageIndex)
    {
        try
        {
            var newsApiSetting = _appConfigService.GetSection<NewsApiSetting>(nameof(NewsApiSetting));
            var connection = $"{newsApiSetting.Connection}/news?pageSize={pageSize}&pageIndex={pageIndex}";
            var response = await _httpClient.PostAsJsonAsync(connection, new
                { Ids = providers.Select(x => x.Id) });
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var body = await response.Content.ReadAsByteArrayAsync();
            var content = JsonSerializer.Deserialize<IEnumerable<ContentModel>>(body);
            return content;
        }
        catch (Exception ex)
        {
            throw new NewsApiException(ex.Message, ex);
        }
    }

    public async Task<IEnumerable<NewsProvider>> GetNewsProvidersAsync()
    {
        try
        {
            var newsApiSetting = _appConfigService.GetSection<NewsApiSetting>(nameof(NewsApiSetting));
            var connection = $"{newsApiSetting.Connection}/news-providers";
            var response = await _httpClient.GetAsync(connection);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var body = await response.Content.ReadAsByteArrayAsync();
            var content = JsonSerializer.Deserialize<IEnumerable<NewsProvider>>(body);
            return content;
        }
        catch (Exception ex)
        {
            throw new NewsApiException(ex.Message, ex);
        }
    }
}