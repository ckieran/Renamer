using System;

namespace Renamer.Core.Utils
{
    public static class DateUtils
    {
        public static string FormatDateRange(DateTime? min, DateTime? max)
        {
            if (min == null && max == null) return string.Empty;
            if (min == max) return min?.ToString("yyyy-MM-dd") ?? DateTime.MinValue.ToString("yyyy-MM-dd");
            return $"{min:yyyy-MM-dd} - {max:yyyy-MM-dd}";
        }
    }
}
