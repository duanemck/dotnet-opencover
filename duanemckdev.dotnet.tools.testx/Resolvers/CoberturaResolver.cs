using System;
using System.IO;
using System.Linq;

namespace duanemckdev.dotnet.tools.testx.resolvers
{
    public class CoberturaResolver : NugetExeResolver
    {
        protected override string PackageName { get; } = "OpenCoverToCoberturaConverter";
        protected override string ExeName { get; } = "OpenCoverToCoberturaConverter.exe";        
    }
}