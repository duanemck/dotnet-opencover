using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;


namespace dotnet_opencover
{
    class Program
    {
        private const string ResultsFile = "results.xml";
        private const string CoberturaFile = "cobertura.xml";
        private const string ReportFolder = "coverage";

        static int Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(options =>
                    {
                        var openCoverExe = new OpenCoverResolver().Resolve(options.OpenCoverVersion);
                        var projectFile = MsBuildProjectFinder.FindMsBuildProject(Directory.GetCurrentDirectory(), options.Project);

                        LogHeader($"Running tests (instrumented by OpenCover) for {projectFile}");
                        new OpenCoverRunner(openCoverExe).Run(projectFile, "");
                        LogFooter();

                        if (options.GenerateReport)
                        {
                            LogHeader("Generating HTML Report");
                            var reporterExe = new ReportGeneratorResolver().Resolve(null);
                            new ReportGeneratorRunner(reporterExe).GenerateReport(ResultsFile, ReportFolder);

                            if (options.LaunchBrowser)
                            {
                                HtmlLauncher.OpenBrowser("coverage\\index.htm");
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
