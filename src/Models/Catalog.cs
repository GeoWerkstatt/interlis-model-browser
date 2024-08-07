﻿namespace ModelRepoBrowser.Models;

public class Catalog
{
    public int CatalogId { get; set; }

    public string Identifier { get; set; }

    public string Version { get; set; }

    public string? PrecursorVersion { get; set; }

    public DateTime? PublishingDate { get; set; }

    public string? Owner { get; set; }

    public List<string> File { get; set; }

    public string? Title { get; set; }

    public List<string> ReferencedModels { get; set; }

    public Repository ModelRepository { get; set; }
}
