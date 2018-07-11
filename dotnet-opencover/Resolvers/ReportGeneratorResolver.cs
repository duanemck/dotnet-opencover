using System;
using System.IO;
using System.Linq;

namespace dotnet_opencover
{
    public class ReportGeneratorResolver : NugetExeResolver
    {
        protected override string PackageName { get; } = "ReportGenerator";
        protected override string ExeName { get; } = "ReportGenerator.exe";
    }
}