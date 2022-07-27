using ModelRepoBrowser.Crawler.XmlModels;

namespace ModelRepoBrowser.Crawler
{
    internal static class CrawlerFilterExtensions
    {
        internal static bool IsCatalog(DatasetMetadata dataset)
            => dataset.categories?.Any(c => c.value.Equals("http://codes.interlis.ch/type/referenceData", StringComparison.Ordinal)) ?? false;

        private const string ModelCode = "http://codes.interlis.ch/model/";
        internal static List<string> GetReferencedModels(this DatasetMetadata data)
            => data.categories?
            .Select(c => c.value)
            .Where(v => v is not null && v.StartsWith(ModelCode, StringComparison.Ordinal))
            .Select(v => v.Substring(ModelCode.Length))
            .Distinct()
            .ToList() ?? new List<string>();

        internal static string GetTitle(this DatasetMetadata data)
            => data.title?.MultilingualText?.LocalisedTexts?.FirstOrDefault(lt => string.Empty.Equals(lt.Language, StringComparison.OrdinalIgnoreCase))?.Language ?? string.Empty;

        internal static List<string> GetFiles(this DatasetMetadata data)
            => data.files?
                .Where(f => f.file is not null)
                .SelectMany(f => f.file)
                .Select(f => f.path)
                .ToList()
            ?? new List<string>();
    }
}
