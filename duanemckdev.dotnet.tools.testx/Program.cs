using System;
using System.Collections.Generic;
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
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(options =>
                    {
                        if (options.RunForAllProjects != null || options.Project == "all")
                        {
                            Console.Out.WriteLine($"Discovering all projects ({options.RunForAllProjects})...");
                            var projectFiles =
                                TraverseAndLocateProjectFiles(new DirectoryInfo(Directory.GetCurrentDirectory()), options.RunForAllProjects);
                            if (!projectFiles.Any())
                            {
                                Console.Out.WriteLine("No projects found");
                                return;
                            }
                            Console.Out.WriteLine(projectFiles.Aggregate("Found the following projects:\n", (output,file)=> $"{output}\t- {file.Name}\n").Trim());
                            foreach (var projectFile in projectFiles)
                            {
                                RunForProject(projectFile.FullName, options);
                            }
                        }
                        else
                        {
                            var projectFile = MsBuildProjectFinder.FindMsBuildProject(Directory.GetCurrentDirectory(), options.Project);
                            RunForProject(projectFile, options);
                        }  
                        
                        GenerateReports(options);
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

        private static List<FileInfo> TraverseAndLocateProjectFiles(DirectoryInfo folder, string pattern)
        {
            var files = new List<FileInfo>();
            TraverseAndLocateProjectFiles(files, folder, pattern);
            return files;
        }

        private static void TraverseAndLocateProjectFiles(List<FileInfo> projectFiles, DirectoryInfo folder, string pattern)
        {
            projectFiles.AddRange(folder.GetFiles(pattern));
            foreach (var subFolder in folder.GetDirectories())
            {
                TraverseAndLocateProjectFiles(projectFiles, subFolder, pattern);
            }
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

        private static void GenerateReports(Options options)
        {
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
