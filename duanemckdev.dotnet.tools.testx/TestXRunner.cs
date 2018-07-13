using System;
using System.IO;
using CSharpx;
using duanemckdev.dotnet.tools.testx.resolvers;
using duanemckdev.dotnet.tools.testx.runners;

namespace duanemckdev.dotnet.tools.testx
{
    public class TestXRunner
    {
        private const string CoverageLocation = "coverage";
        private static readonly string ResultsFile = $"{CoverageLocation}\\results.xml";
        private static readonly string CoberturaFile = $"{CoverageLocation}\\cobertura.xml";
        private static readonly string ReportFolder = $"{CoverageLocation}\\htmlreport";
        private readonly Options _options;

        public TestXRunner(Options options)
        {
            _options = options;
            LogOptions();
        }

        public int Run()
        {
            try
            {
                if (_options.RunForAllProjects != null || _options.Project == "all")
                {
                    var projectFiles = MsBuildProjectFinder.FindAllProjectsInFolder(Directory.GetCurrentDirectory(),
                        _options.RunForAllProjects);
                    projectFiles.ForEach(file => RunForProject(file, _options));
                }
                else
                {
                    var projectFile =
                        MsBuildProjectFinder.FindMsBuildProject(Directory.GetCurrentDirectory(), _options.Project);
                    RunForProject(projectFile, _options);
                }

                GenerateReports();
                LogHeader($"Coverage results in {CoverageLocation}");
                LogFooter();
                return 0;
            }
            catch (ProcessException processException)
            {
                Console.Error.WriteLine(processException.Message);
                return processException.ExitCode;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                Console.Error.WriteLine(exception.StackTrace);
                return 1;
            }
        }

        private void LogOptions()
        {
            Console.Out.WriteLine("Options Passed:\n");
            if (_options.RunForAllProjects != null)
            {
                Console.Out.WriteLine($"\t Will discover projects with pattern {_options.RunForAllProjects}");
            }
            else if (_options.Project != null)
            {
                Console.Out.WriteLine($"\t Will run with project {_options.Project}");
            }
            else
            {
                Console.Out.WriteLine($"\t No project specified, will try find one in working directory");
            }

            Console.Out.WriteLine($"\t Test results format: {_options.TestResultsFormat}");
            Console.Out.WriteLine($"\t OpenCover filters: {_options.OpenCoverFilters}");
            Console.Out.WriteLine($"\t Override OpenCover version to: {_options.OpenCoverVersion}");

            Console.Out.WriteLine($"\t Will {(_options.OpenCoverMerge?"":"NOT")} merge open cover reports");
            Console.Out.WriteLine($"\t Will {(_options.Cobertura ? "" : "NOT")} generate cobertura format report");
            Console.Out.WriteLine($"\t Will {(_options.GenerateReport ? "" : "NOT")} generate HTML report");
            Console.Out.WriteLine($"\t Will {(_options.LaunchBrowser ? "" : "NOT")} launch browser with HTML report");
        }

        private static void RunForProject(string projectFile, Options options)
        {
            var openCoverExe = new OpenCoverResolver().Resolve(options.OpenCoverVersion);

            if (!Directory.Exists(CoverageLocation))
            {
                Directory.CreateDirectory(CoverageLocation);
            }

            LogHeader($"Running tests (instrumented by OpenCover) for {projectFile}");
            new OpenCoverRunner(openCoverExe).Run(projectFile, options.OpenCoverFilters, ResultsFile, options.OpenCoverMerge);
            LogFooter();


        }

        private  void GenerateReports()
        {
            if (_options.GenerateReport)
            {
                LogHeader("Generating HTML Report");
                var reporterExe = new ReportGeneratorResolver().Resolve(null);
                new ReportGeneratorRunner(reporterExe).GenerateReport(ResultsFile, ReportFolder);

                if (_options.LaunchBrowser)
                {
                    HtmlLauncher.OpenBrowser($"{ReportFolder}\\index.htm");
                }

                LogFooter();
            }

            if (_options.Cobertura)
            {
                LogHeader("Generating Cobertura Report");
                var converterExe = new CoberturaResolver().Resolve(null);
                new CoberturaRunner(converterExe).Run(ResultsFile, CoberturaFile);
                LogFooter();
            }
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