﻿using ModelRepoBrowser.Crawler.XmlModels;
using System.Xml;
using System.Xml.Serialization;

namespace ModelRepoBrowser.Crawler
{
    public static class RepositoryFilesDeserializer
    {
        private static Dictionary<Type, XmlSerializer> serializers = new();

        private static T? DeserializeDatasection<T>(Stream stream)
        {
            XmlSerializer? serializer;
            if (!serializers.TryGetValue(typeof(T), out serializer))
            {
                serializer = new XmlSerializer(typeof(T));
                serializers.Add(typeof(T), serializer);
            }

            using (var xmlReader = new XmlTextReader(stream))
            {
                xmlReader.Namespaces = false;
                xmlReader.ReadToDescendant("DATASECTION");
                var subtree = xmlReader.ReadSubtree();

                return (T?)serializer.Deserialize(subtree);
            }
        }

        /// <summary>
        /// Parse an ilisite.xml from an <paramref name="xmlStream"/>.
        /// </summary>
        /// <param name="xmlStream">The XML-<see cref="Stream"/> that contains an ilisite.xml.</param>
        /// <returns>The parsed <see cref="Site"/> object of the ilisite.xml.</returns>
        /// <exception cref="InvalidOperationException">Could not deserialize the provided XML.</exception>
        internal static Site? ParseIliSite(Stream xmlStream)
        {
            var dataSection = DeserializeDatasection<IliSitesDataSection>(xmlStream);
            return dataSection?.SiteMetadata?.Site;
        }

        /// <summary>
        /// Parse an ilimodels.xml from an <paramref name="xmlStream"/>.
        /// </summary>
        /// <param name="xmlStream">The XML-<see cref="Stream"/> that contains an ilimodels.xml.</param>
        /// <returns>Flat list of all parsed ModelsMetadata contained in ilimodels.xml.</returns>
        /// <exception cref="InvalidOperationException">Could not deserialize the provided XML.</exception>
        internal static IEnumerable<ModelMetadata> ParseIliModels(Stream xmlStream)
        {
            var dataSection = DeserializeDatasection<IliModelsDatasection>(xmlStream);
            var result = dataSection?.Items
                                .Where(x => x?.ModelMetadata is not null)
                                .SelectMany(x => x.ModelMetadata);

            return result ?? Enumerable.Empty<ModelMetadata>();
        }

        /// <summary>
        /// Parse an ilidata.xml from an <paramref name="xmlStream"/>.
        /// </summary>
        /// <param name="xmlStream">The XML-<see cref="Stream"/> that contains an ilidata.xml.</param>
        /// <returns>Flat list of all parsed Catalog-DatasetMetadata contained in ilidata.xml.</returns>
        /// <exception cref="InvalidOperationException">Could not deserialize the provided XML.</exception>
        internal static IEnumerable<DatasetMetadata> ParseIliData(Stream xmlStream)
        {
            var dataSection = DeserializeDatasection<IliDataDatasection>(xmlStream);

            var result = dataSection?.DatasetIdx16DataIndex?
                                .Where(x => x?.DatasetMetadata is not null)
                                .SelectMany(x => x.DatasetMetadata)
                                .Where(CrawlerHelperExtensions.IsCatalog);

            return result ?? Enumerable.Empty<DatasetMetadata>();
        }
    }
}
