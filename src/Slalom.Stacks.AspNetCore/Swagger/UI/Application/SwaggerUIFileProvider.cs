using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Slalom.Stacks.AspNetCore.Swagger.UI.Application
{
    public class SwaggerUIFileProvider : IFileProvider
    {
        private const string StaticFilesNamespace =
            "Slalom.Stacks.AspNetCore.bower_components.swagger_ui.dist";
        private const string IndexResourceName =
            "Slalom.Stacks.AspNetCore.Swagger.UI.Template.index.html";

        private readonly Assembly _thisAssembly;
        private readonly EmbeddedFileProvider _staticFileProvider;
        private readonly IDictionary<string, string> _indexParameters;

        public SwaggerUIFileProvider(IDictionary<string, string> indexParameters)
        {
            _thisAssembly = this.GetType().GetTypeInfo().Assembly;
            _staticFileProvider = new EmbeddedFileProvider(_thisAssembly, StaticFilesNamespace);
            _indexParameters = indexParameters;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _staticFileProvider.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == "/index.html")
                return new SwaggerUIIndexFileInfo(_thisAssembly, IndexResourceName, _indexParameters);

            return _staticFileProvider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return _staticFileProvider.Watch(filter);
        }
    }
}