using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPackager
{
    static class PathHelper
    {
        public static string ToAbsolutePath(string basePath, string path)
        {
            if (string.IsNullOrEmpty(basePath))
                return path;
            if (!basePath.EndsWith("\\") && !basePath.EndsWith("/"))
                basePath += "\\";

            Uri uriBase;
            if (!Uri.TryCreate(basePath, UriKind.Absolute, out uriBase))
                return path;

            Uri uri;
            if (!Uri.TryCreate(uriBase, path, out uri))
            {
                return path;
            }

            return uri.LocalPath;
        }

        public static string ToRelativePath(string basePath, string path)
        {
            if (string.IsNullOrEmpty(basePath))
                return path;
            if (!basePath.EndsWith("\\") && !basePath.EndsWith("/"))
                basePath += "\\";

            Uri uriBase;
            if (!Uri.TryCreate(basePath, UriKind.Absolute, out uriBase))
                return path;

            Uri uri;
            if (!Uri.TryCreate(path, UriKind.Absolute, out uri))
                return path;

            return uriBase.MakeRelativeUri(uri).ToString();
        }
    }
}
