namespace ModelRepoBrowser;

public static class LogHelper
{
    public static string? Escape(string? input)
    {
        return input?.ReplaceLineEndings(" ");
    }
}
