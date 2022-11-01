using System;
using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetBrowser_UWP.ViewModels
{
    public class NewsPageViewModel : ObservableObject
    {
        private ObservableCollection<ContentModel> _news = new();
        private bool _isProgressRingActive = true;

        public bool IsProgressRingActive
        {
            get => _isProgressRingActive;
            set => SetProperty(ref _isProgressRingActive, value);
        }

        public ObservableCollection<ContentModel> News
        {
            get => _news;
            set => SetProperty(ref _news, value);
        }

        public NewsPageViewModel()
        {
            Initialize();
        }

        public async void Initialize()
        {
            await GetNews();
            IsProgressRingActive = false;
        }
        private async Task GetNews()
        {
            const string API_KEY = "pub_12885f320dc5b090376f3097f23792824e93d";
            //const string API_KEY = "fdbee308fbd541d79be07e2283764da3";
            const string URL = $"https://newsdata.io/api/1/news?apikey={API_KEY}&language=ru";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(URL);
            var responseBody = await response.Content.ReadAsStringAsync();

            var des = JsonSerializer.Deserialize<NewsModel>(responseBody);
            var news = new ObservableCollection<ContentModel>();

            if (des is {Content: { }})
            {
                foreach (var content in des.Content)
                {
                    if (content.ImageUrl != null && content.Link != null)
                    {
                        news.Add(content);
                    }
                    
                }
            }

            News = news;

        }
    }
}
