using System.ComponentModel.DataAnnotations;

namespace ModelRepoBrowser.Models;

public class Repository
{
    [Key]
    public string HostNameId { get; set; }

    public Uri Uri { get; set; }

    public string Name { get; set; }

    public string? Title { get; set; }

    public string? ShortDescription { get; set; }

    public string? Owner { get; set; }

    public string? TechnicalContact { get; set; }

    public ISet<Repository> SubsidiarySites { get; set; } = new HashSet<Repository>();

    public ISet<Repository> ParentSites { get; set; } = new HashSet<Repository>();

    public ISet<Model> Models { get; set; } = new HashSet<Model>();

    public ISet<Catalog> Catalogs { get; set; } = new HashSet<Catalog>();
}
