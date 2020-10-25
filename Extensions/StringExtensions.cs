using SharpGen.Runtime.Win32;

namespace NeBrowser.Extensions
{
    public static class StringExtensions
    {
        public static string EmptyIfNull(this string s)
        {
            return s ?? string.Empty;
        }
    }
}