using System.Text.RegularExpressions;

namespace ModelRepoBrowser;

public static partial class LogHelper
{
    [GeneratedRegex(@"\r\n|\r|\n")]
    private static partial Regex NewLinesRegex();

    public static string? Escape(string? input)
    {
        return input == null ? null : NewLinesRegex().Replace(input, " ");
    }
}
