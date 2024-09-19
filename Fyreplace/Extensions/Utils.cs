using System;

namespace Fyreplace.Extensions
{
    public static class Utils
    {
        public static T? Also<T>(this T? obj, Action action)
        {
            action();
            return obj;
        }
    }
}
