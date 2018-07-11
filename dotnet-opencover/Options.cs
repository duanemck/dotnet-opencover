using System.Collections.Generic;
using CommandLine;

namespace dotnet_opencover
{
    public class Options
    {
        [Option("opencover-version", Required = false, HelpText = "Optional OpenCover version.")]
        public string OpenCoverVersion{ get; set; }

        [Option("project", Required = false, HelpText = "Project to test.")]
        public string Project { get; set; }

        [Option("html", Required = false, Default = false, HelpText = "Generate an HTML report")]
        public bool GenerateReport { get; set; }

        [Option("browser", Required = false, Default = false, HelpText = "If generating an HTML report, open in the default browser")]
        public bool LaunchBrowser { get; set; }


        [Option("cobertura", Required = false, Default = false, HelpText = "Generate a cobertura report")]
        public bool Cobertura { get; set; }

        [Option("test-results-format", Required = false, Default = "trx", HelpText = "The format to use for the VSTest test results")]
        public string TestResultsFormat { get; set; }
    }
}