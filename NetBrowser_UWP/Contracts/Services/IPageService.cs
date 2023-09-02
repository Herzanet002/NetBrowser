using System;
using NetBrowser_UWP.Services.PageService;

namespace NetBrowser_UWP.Contracts.Services;

public interface IPageService
{
    PageInfo? GetPageInfo(string path);

    PageInfo? GetPageInfoByPageType(Type pageType);
}