using System;
using System.IO;
using System.Linq;

namespace duanemckdev.dotnet.tools.testx.resolvers
{
    public class ReportGeneratorResolver : NugetExeResolver
    {
        protected override string PackageName { get; } = "ReportGenerator";
        protected override string ExeName { get; } = "ReportGenerator.exe";
    }
}