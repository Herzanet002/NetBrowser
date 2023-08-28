using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using NetBrowser_UWP.Attributes;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.Services.PageService;

public class PageService : IPageService
{
    public PageService()
        => InitializePagesList();


    public PageInfo GetPageInfo(string path)
        => _pages.Find(x => x.Path == path);

    private readonly List<PageInfo> _pages = new();

    private void InitializePagesList()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var pageTypes = assembly
            .GetTypes()
            .Where(type => typeof(Page).IsAssignableFrom(type) && type.IsSealed);

        foreach (var pageType in pageTypes)
        {
            var pageAttributes = pageType.GetCustomAttributes();
            var page = new PageInfo();

            foreach (var attribute in pageAttributes)
            {
                switch (attribute)
                {
                    case PageAddressAttribute addressAttribute:
                        page.Path = addressAttribute.Address;
                        break;
                    case ParentPageTypeAttribute parentTypeAttribute:
                        page.ParentType = parentTypeAttribute.ParentPageType;
                        break;
                }
            }

            page.Type = pageType;

            _pages.Add(page);
        }
    }
}