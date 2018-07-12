using System;
using System.IO;
using System.Linq;

namespace duanemckdev.dotnet.tools.testx.resolvers
{
    public class OpenCoverResolver : NugetExeResolver
    {
        protected override string PackageName { get; } = "OpenCover";
        protected override string ExeName { get; } = "OpenCover.Console.exe";
    }
}