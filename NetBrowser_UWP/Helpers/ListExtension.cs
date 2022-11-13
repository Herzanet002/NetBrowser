using System;
using System.Collections.Generic;

namespace NetBrowser_UWP.Helpers;

public static class ListExtension
{
    private static readonly Random Rand = new((int) DateTime.Now.Ticks & 0x0000FFFF);


    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Rand.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}