using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (options.Verbose)
            {
                LogOptions();
            }
        }

        public int Run()
        {
            try
            {
                IEnumerable<int> exitCodes = null;
                CleanupPreviousResults();
                if (_options.Project == "all")
                {
                    _options.RunForAllProjects = "*Tests.csproj";
                }
                if (_options.RunForAllProjects != null)
                {
                    LogHeader($"Discovering projects with pattern {_options.RunForAllProjects}");
                    var projectFiles = MsBuildProjectFinder.FindAllProjectsInFolder(Directory.GetCurrentDirectory(),
                        _options.RunForAllProjects, _options.Verbose);
                    LogFooter();
                    exitCodes = projectFiles.Select(RunForProject).ToList();                    
                }
                else
                {
                    var projectFile =
                        MsBuildProjectFinder.FindMsBuildProject(Directory.GetCurrentDirectory(), _options.Project);
                    var exitCode = RunForProject(projectFile);
                    exitCodes = new List<int>(){exitCode};
                }

                GenerateReports();
                LogHeader($"Coverage results in {CoverageLocation}");
                LogFooter();

                if (exitCodes.Any(code => code != 0))
                {
                    Console.Error.WriteLine("One or more of the test executions exited with non-zero exit code");
                    return 1;
                }
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

        private void CleanupPreviousResults()
        {
            if (Directory.Exists(CoverageLocation))
            {
                Directory.Delete(CoverageLocation, true);
                
            }
            Directory.CreateDirectory(CoverageLocation);
        }

        private void LogOptions()
        {
            LogHeader("Options");
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
            if (_options.OpenCoverVersion != null)
            {
                Console.Out.WriteLine($"\t Override OpenCover version to: {_options.OpenCoverVersion}");
            }

            Console.Out.WriteLine($"\t Will{(_options.OpenCoverMerge?"":" NOT")} merge open cover reports");
            Console.Out.WriteLine($"\t Will{(_options.Cobertura ? "" : " NOT")} generate cobertura format report");
            Console.Out.WriteLine($"\t Will{(_options.GenerateReport ? "" : " NOT")} generate HTML report");
            Console.Out.WriteLine($"\t Will{(_options.LaunchBrowser ? "" : " NOT")} launch browser with HTML report");
            LogFooter();
        }

        private int RunForProject(string projectFile)
        {
            var openCoverExe = new OpenCoverResolver().Resolve(_options.OpenCoverVersion);
            
            LogHeader($"Running tests (instrumented by OpenCover) for {projectFile}", true);
            var exitCode = new OpenCoverRunner(openCoverExe, _options.Verbose).Run(projectFile, _options.OpenCoverFilters, ResultsFile, _options.OpenCoverMerge, _options.OpenCoverOptions);
            LogFooter();
            return exitCode;
        }

        private  void GenerateReports()
        {
            if (_options.GenerateReport)
            {
                LogHeader("Generating HTML Report");
                var reporterExe = new ReportGeneratorResolver().Resolve(null);
                new ReportGeneratorRunner(reporterExe, _options.Verbose).GenerateReport(ResultsFile, ReportFolder);

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
                new CoberturaRunner(converterExe, _options.Verbose).Run(ResultsFile, CoberturaFile);
                LogFooter();
            }
        }

        private void LogHeader(string message, bool forceLog = false)
        {
            if (_options.Verbose || forceLog)
            {
                Console.Out.WriteLine(
                    "======================================================================================================================================================");
                Console.Out.WriteLine($"\t{message}");
                Console.Out.WriteLine(
                    "------------------------------------------------------------------------------------------------------------------------------------------------------");
            }
        }

        private void LogFooter()
        {
            if (_options.Verbose)
            {
                Console.Out.WriteLine(
                    "======================================================================================================================================================");
            }
        }
    }
}