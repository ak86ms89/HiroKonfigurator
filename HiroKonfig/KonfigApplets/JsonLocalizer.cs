using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace JsonLocalizing
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private IFileProvider FileProvider { get; }
        private string Name { get; }
        private string ResourcesPath { get; }

        public JsonStringLocalizer(IFileProvider fileProvider, string resourcePath, string name)
        {
            FileProvider = fileProvider;
            Name = name;
            ResourcesPath = resourcePath;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public LocalizedString this[string name]
        {
            get
            {
                var stringMap = LoadStringMap();

                return new LocalizedString(name, stringMap[name]);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var stringMap = LoadStringMap();

                return new LocalizedString(name, string.Format(stringMap[name], arguments));
            }
        }

        private Dictionary<string, string> LoadStringMap()
        {
            var cultureInfo = CultureInfo.CurrentUICulture;

            var cultureName = cultureInfo.TwoLetterISOLanguageName;

            var fileInfo =
                FileProvider.GetFileInfo(
                    Path.Combine(ResourcesPath, $"{Name}-{cultureName}.json"));

            if (!fileInfo.Exists)
            {
                fileInfo =
                    FileProvider.GetFileInfo(
                        Path.Combine(ResourcesPath, $"{Name}.json"));
            }

            using var stream = fileInfo.CreateReadStream();

            return JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream).Result;
        }
    }

    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private string ResourcesPath { get; }

        public JsonStringLocalizerFactory(IOptions<LocalizationOptions> options)
        {
            ResourcesPath = options.Value.ResourcesPath;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var resources = new EmbeddedFileProvider(resourceSource.Assembly);
            return new JsonStringLocalizer(resources, ResourcesPath, resourceSource.Name);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            throw new NotImplementedException();
        }
    }

}
