using System;
using System.IO;
using System.Reflection;

namespace GhostNetwork.Publications.Api.Helpers.OpenApi
{
    public static class XmlPathProvider
    {
        public static string XmlPath => Path.Combine(
            AppContext.BaseDirectory,
            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    }
}