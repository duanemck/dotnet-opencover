using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using duanemckdev.dotnet.tools.testx.resolvers;
using duanemckdev.dotnet.tools.testx.runners;


namespace duanemckdev.dotnet.tools.testx
{
    class Program
    {
        private const string CoverageLocation = "coverage";
        private static readonly string ResultsFile = $"{CoverageLocation}\\results.xml";
        private static readonly string CoberturaFile = $"{CoverageLocation}\\cobertura.xml";
        private static readonly string ReportFolder = $"{CoverageLocation}\\htmlreport";

        static int Main(string[] args)
        {
            try
            {
                Console.Out.WriteLine($"@@@@@@@@@@@@@@@@@@@@@@@@@{args.Aggregate("", (a, b) => $"{a},{b}")}@@@@@@@@@@@@@@@@@@@@@@");
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(options =>
                    {
                        var openCoverExe = new OpenCoverResolver().Resolve(options.OpenCoverVersion);
                        var projectFile = MsBuildProjectFinder.FindMsBuildProject(Directory.GetCurrentDirectory(), options.Project);

                        if (!Directory.Exists(CoverageLocation))
                        {
                            Directory.CreateDirectory(CoverageLocation);
                        }

                        LogHeader($"Running tests (instrumented by OpenCover) for {projectFile}");
                        new OpenCoverRunner(openCoverExe).Run(projectFile, options.OpenCoverFilters, ResultsFile, options.OpenCoverMerge);
                        LogFooter();

                        if (options.GenerateReport)
                        {
                            LogHeader("Generating HTML Report");
                            var reporterExe = new ReportGeneratorResolver().Resolve(null);
                            new ReportGeneratorRunner(reporterExe).GenerateReport(ResultsFile, ReportFolder);

                            if (options.LaunchBrowser)
                            {
                                HtmlLauncher.OpenBrowser($"{ReportFolder}\\index.htm");
                            }

                            LogFooter();
                        }                        

                        if (options.Cobertura)
                        {
                            LogHeader("Generating Cobertura Report");
                            var converterExe = new CoberturaResolver().Resolve(null);
                            new CoberturaRunner(converterExe).Run(ResultsFile, CoberturaFile);
                            LogFooter();
                        }

                        LogHeader($"Coverage results in {CoverageLocation}");
                        LogFooter();
                    });

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return 1;
            }
            return 0;
            
        }

        private static void LogHeader(string message)
        {
            Console.Out.WriteLine("======================================================================================================================================================");
            Console.Out.WriteLine($"\t{message}");
            Console.Out.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------");            
        }

        private static void LogFooter()
        {
            Console.Out.WriteLine("======================================================================================================================================================");
        }
    }
}
