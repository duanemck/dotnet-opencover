using System;
using System.IO;
using System.Linq;

namespace dotnet_opencover
{
    public class CoberturaResolver : NugetExeResolver
    {
        protected override string PackageName { get; } = "OpenCoverToCoberturaConverter";
        protected override string ExeName { get; } = "OpenCoverToCoberturaConverter.exe";        
    }
}