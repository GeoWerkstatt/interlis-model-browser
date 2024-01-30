using System.Text.RegularExpressions;

namespace ModelRepoBrowser;

public static partial class StringExtensions
{
    [GeneratedRegex(@"\r\n|\r|\n")]
    private static partial Regex NewLinesRegex();

    /// <summary>
    /// Replaces new lines in the specified <paramref name="input"/> string with a space.
    /// Can be used to escape new lines in log messages to prevent log forging.
    /// </summary>
    /// <param name="input">The input string to escape.</param>
    /// <returns>The input string with all new lines replaced by a space.</returns>
    public static string? EscapeNewLines(this string? input)
    {
        return input == null ? null : NewLinesRegex().Replace(input, " ");
    }
}
