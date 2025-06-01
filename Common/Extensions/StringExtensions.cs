namespace Common.Extensions;
using static Domain.Models.Constants.TextConstants;

public static class StringExtensions
{
    public static string FormatAsLog(this string message, bool performedBySynchronizer)
        => $"({DateTime.Now})" +
            $"{COLUMN_SEPARATOR}{(performedBySynchronizer ? SOURCE_SYNCHRONIZER : SOURCE_EXTERNAL).PadRight(SOURCE_COLUMN_WIDTH)}" +
            $"{COLUMN_SEPARATOR}{message}";
}