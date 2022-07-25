using System.ComponentModel.DataAnnotations;

namespace ModelRepoBrowser.Models
{
    public class Model
    {
        [Key]
        public string? MD5 { get; set; }
        public string? Name { get; set; }

        public string? SchemaLanguage { get; set; }

        public string? File { get; set; }

        public string? Version { get; set; }

        public DateTime PublishingDate { get; set; }

        //public ISet<string> DependsOnModel { get; set; } = new HashSet<string>();

        //public ISet<string> Tags { get; set; } = new HashSet<string>();

        public string? ShortDescription { get; set; }

        public string? Issuer { get; set; }

        public string? TechnicalContact { get; set; }

        public string? FurtherInformation { get; set; }


        public Repository? ModelRepository { get; set; }

        public Uri? Uri => ModelRepository.Uri is null ? null : new Uri(ModelRepository.Uri, File ?? string.Empty);
    }
}
