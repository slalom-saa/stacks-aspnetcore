﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Slalom.Stacks.AspNetCore.Swagger.UI.Application
{
    public class SwaggerUIIndexFileInfo : IFileInfo
    {
        private readonly Assembly _assembly;
        private readonly string _resourcePath;
        private readonly IDictionary<string, string> _parameters;

        private long? _length;

        public SwaggerUIIndexFileInfo(Assembly assembly, string resourcePath, IDictionary<string, string> parameters)
        {
            _assembly = assembly;
            _resourcePath = resourcePath;
            _parameters = parameters;
        }

        public bool Exists => true;

        public long Length
        {
            get
            {
                if (!_length.HasValue)
                {
                    using (var stream = this.CreateParameterizedStream())
                    {
                        _length = stream.Length;
                    }
                }
                return _length.Value;
            }
        }

        public string PhysicalPath => null;

        public string Name => "index.html";

        public DateTimeOffset LastModified => DateTimeOffset.UtcNow;

        public bool IsDirectory => false;

        public Stream CreateReadStream()
        {
            var stream = this.CreateParameterizedStream();
            if (!_length.HasValue)
            {
                _length = stream.Length;
            }
            return stream;
        }

        private Stream CreateParameterizedStream()
        {
            var names = _assembly.GetManifestResourceNames();
            using (var templateStream = _assembly.GetManifestResourceStream(_resourcePath))
            {
                var templateText = new StreamReader(templateStream).ReadToEnd();
                var parameterizedTextBuilder = new StringBuilder(templateText);
                foreach (var entry in _parameters)
                {
                    parameterizedTextBuilder.Replace(entry.Key, entry.Value);
                }

                return new MemoryStream(Encoding.UTF8.GetBytes(parameterizedTextBuilder.ToString()));
            }
        }
    }
}