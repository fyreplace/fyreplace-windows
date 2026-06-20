using System;

namespace Fyreplace.Extensions
{
    public static class Utils
    {
        public static T? Also<T>(this T? self, Action action)
        {
            action();
            return self;
        }
    }
}
