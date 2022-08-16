﻿using Bogus;
using ModelRepoBrowser.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRepoBrowser;

public static class RepoBrowserContextExtensions
{
    public static void SeedData(this RepoBrowserContext context)
    {
        using var transaction = context.Database.BeginTransaction();

        // Set Bogus Data System Clock
        Bogus.DataSets.Date.SystemClock = () => DateTime.Parse("01.01.2022 00:00:00", new CultureInfo("de_CH", false));

        var schemaLanguages = new[] { "ili2_4", "ili1", "ili2_2", "ili2_3" };

        var fakeRepositories = new Faker<Repository>()
            .StrictMode(true)
            .RuleFor(r => r.HostNameId, f => f.Internet.DomainName())
            .RuleFor(r => r.Uri, f => new Uri(f.Internet.Url()))
            .RuleFor(r => r.Name, f => f.Random.Word())
            .RuleFor(r => r.Title, f => f.Random.Word())
            .RuleFor(r => r.ShortDescription, f => f.Random.Words())
            .RuleFor(r => r.Owner, f => f.Name.FullName())
            .RuleFor(r => r.TechnicalContact, f => f.Name.FullName())
            .RuleFor(r => r.SubsidiarySites, f => new HashSet<Repository>())
            .RuleFor(r => r.ParentSites, f => new HashSet<Repository>())
            .RuleFor(r => r.Models, f => new HashSet<Model>())
            .RuleFor(r => r.Catalogs, f => new HashSet<Catalog>());
        Repository SeededRepository(int seed) => fakeRepositories.UseSeed(seed).Generate();
        var repositories = Enumerable.Range(1, 20).Select(SeededRepository).ToList();

        foreach (var child in repositories.Skip(1).Take(9))
        {
            repositories[0].SubsidiarySites.Add(child);
            child.ParentSites.Add(repositories[0]);
        }

        foreach (var child in repositories.Skip(10))
        {
            repositories[9].SubsidiarySites.Add(child);
            child.ParentSites.Add(repositories[9]);
        }

        var modelIds = 1;
        var modelRange = Enumerable.Range(modelIds, 100);
        var fakeModels = new Faker<Model>()
            .StrictMode(true)
            .RuleFor(m => m.Id, f => modelIds++)
            .RuleFor(m => m.MD5, f => f.Random.Hash(32))
            .RuleFor(m => m.Name, f => string.Join("_", f.Random.WordsArray(1, 5)))
            .RuleFor(m => m.SchemaLanguage, f => f.PickRandom(schemaLanguages))
            .RuleFor(m => m.File, f => f.System.FilePath().Trim('/').OrDefault(f, 0.1f, "obsolete" + f.System.FilePath()))
            .RuleFor(m => m.Version, f => f.Date.Past().ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo))
            .RuleFor(m => m.PublishingDate, f => f.Date.Past())
            .RuleFor(m => m.DependsOnModel, new List<string>())
            .RuleFor(m => m.Tags, f => f.Random.WordsArray(0, 5).ToList())
            .RuleFor(m => m.ShortDescription, f => f.Random.Words())
            .RuleFor(m => m.Issuer, f => f.Internet.Email())
            .RuleFor(m => m.TechnicalContact, f => f.Internet.Email())
            .RuleFor(m => m.FurtherInformation, f => f.Lorem.Sentence())
            .RuleFor(m => m.ModelRepository, f => f.PickRandom(repositories));
        Model SeededModel(int seed) => fakeModels.UseSeed(seed).Generate();
        var models = modelRange.Select(SeededModel);
        context.Models.AddRange(models);
        context.SaveChanges();

        var catalogIds = 1;
        var catalogRange = Enumerable.Range(catalogIds, 20);
        var fakeCatalogs = new Faker<Catalog>()
            .StrictMode(true)
            .RuleFor(c => c.CatalogId, f => catalogIds++)
            .RuleFor(c => c.Identifier, f => f.Random.Word())
            .RuleFor(c => c.Version, f => f.Date.Past().ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo))
            .RuleFor(c => c.PrecursorVersion, f => f.Date.Past().ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo))
            .RuleFor(c => c.PublishingDate, f => f.Date.Past())
            .RuleFor(c => c.Owner, f => f.Name.FullName())
            .RuleFor(c => c.File, new List<string>())
            .RuleFor(c => c.Title, f => f.Random.Word())
            .RuleFor(c => c.ReferencedModels, f => f.PickRandom(models, 2).Select(m => m.Name).ToList().OrDefault(f, defaultValue: new List<string>()));
        Catalog SeededCatalog(int seed) => fakeCatalogs.UseSeed(seed).Generate();
        var catalogs = catalogRange.Select(SeededCatalog);
        context.Catalogs.AddRange(catalogs);
        context.SaveChanges();

        transaction.Commit();
    }
}
