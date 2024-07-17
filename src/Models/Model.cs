using ModelRepoBrowser.Crawler;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ModelRepoBrowser.Models;

public class Model
{
    public int Id { get; set; }

    /// <summary>
    /// The MD5 Hash of the INTERLIS file that contains this model.
    /// </summary>
    public string? MD5 { get; set; }

    public string Name { get; set; }

    public string SchemaLanguage { get; set; }

    public string File { get; set; }

    public string Version { get; set; }

    public DateTime? PublishingDate { get; set; }

    public string? Title { get; set; }

    public List<string> DependsOnModel { get; set; }

    public List<string> Tags { get; set; }

    public string? ShortDescription { get; set; }

    public string? Issuer { get; set; }

    public string? TechnicalContact { get; set; }

    public string? FurtherInformation { get; set; }

    /// <summary>
    /// The actual content of the INTERLIS file.
    /// </summary>
    [JsonIgnore]
    public InterlisFile FileContent { get; set; }

    [NotMapped]
    public bool? IsDependOnModelResult { get; set; } = false;

    [NotMapped]
    public List<string> CatalogueFiles { get; set; }

    public Repository ModelRepository { get; set; }

    public Uri? Uri => string.IsNullOrEmpty(File) ? null : ModelRepository.Uri.Append(File);
}
