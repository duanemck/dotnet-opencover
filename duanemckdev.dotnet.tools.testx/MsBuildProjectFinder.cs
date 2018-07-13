using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace duanemckdev.dotnet.tools.testx
{
    // https://raw.githubusercontent.com/aspnet/DotNetTools/master/src/dotnet-watch/Internal/MsBuildProjectFinder.cs
    public class MsBuildProjectFinder
    {
        public static string FindMsBuildProject(string searchBase, string project)
        {

            var projectPath = project ?? searchBase;

            if (!Path.IsPathRooted(projectPath))
            {
                projectPath = Path.Combine(searchBase, projectPath);
            }

            if (Directory.Exists(projectPath))
            {
                var projects = Directory.EnumerateFileSystemEntries(projectPath, "*.*proj", SearchOption.TopDirectoryOnly)
                    .Where(f => !".xproj".Equals(Path.GetExtension(f), StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (projects.Count > 1)
                {
                    throw new FileNotFoundException($"Multiple projects found in {projectPath}");
                }

                if (projects.Count == 0)
                {
                    throw new FileNotFoundException($"No projects found in {projectPath}");
                }

                return projects[0];
            }

            if (!File.Exists(projectPath))
            {
                throw new FileNotFoundException($"Projects not found {projectPath}");
            }

            return projectPath;
        }

        public static IEnumerable<string> FindAllProjectsInFolder(string root, string pattern)
        {
            var files = new List<FileInfo>();
            TraverseAndLocateProjectFiles(files, new DirectoryInfo(root), pattern);
            if (!files.Any())
            {
                Console.Out.WriteLine("No projects found");
                return new List<string>();
            }
            Console.Out.WriteLine(files.Aggregate("Found the following projects:\n", (output, file) => $"{output}\t- {file.Name}\n").Trim());
            return files.Select(f => f.FullName);
        }

        private static void TraverseAndLocateProjectFiles(List<FileInfo> projectFiles, DirectoryInfo folder, string pattern)
        {
            projectFiles.AddRange(folder.GetFiles(pattern));
            foreach (var subFolder in folder.GetDirectories())
            {
                TraverseAndLocateProjectFiles(projectFiles, subFolder, pattern);
            }
        }
    }
}

