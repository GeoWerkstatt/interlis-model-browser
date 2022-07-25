namespace ModelRepoBrowser.Models
{
    public class Catalog
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public string PrecursorVersion { get; set; }

        public DateTime PublishingDate { get; set; }

        public string Owner { get; set; }

        //public IDictionary<string, string> Files { get; set; }

        //public IDictionary<string, string> Title { get; set; }

        //public ISet<string> ReferencedModels { get; set; }
    }
}
