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

		private static string DetectSolutionFile(int attemptsLeft, string folder)
		{
			if (folder == null)
			{
				return null;
			}
			var thisFolder = new DirectoryInfo(folder);
			var solutionFileExists = thisFolder.GetFiles("*.sln").Any();
			if (solutionFileExists)
			{
				return folder;
			}

			if (attemptsLeft > 0)
			{
				return DetectSolutionFile(attemptsLeft - 1, thisFolder.Parent?.FullName);
			}

			return null;
		}

		public static IEnumerable<string> FindAllProjectsInFolder(string root, string pattern, bool verbose)
		{
			var solutionFolder = DetectSolutionFile(3, root);
			if (solutionFolder != null)
			{
				root = solutionFolder;
				if (verbose)
				{
					Console.Out.WriteLine($"Working directory does not have a solution, moved up to {root}");
				}
			}
			var files = new List<FileInfo>();
			TraverseAndLocateProjectFiles(files, new DirectoryInfo(root), pattern, verbose);
			if (!files.Any())
			{
				throw new Exception($"No projects found in {root}");
			}
			Console.Out.WriteLine(files.Aggregate("Found the following projects:\n", (output, file) => $"{output}\t- {file.Name}\n").Trim());
			return files.Select(f => Path.Combine(Path.GetRelativePath(root, f.DirectoryName), f.Name));
		}

		private static void TraverseAndLocateProjectFiles(List<FileInfo> projectFiles, DirectoryInfo folder, string pattern, bool verbose)
		{
			if (verbose)
			{
				Console.Out.WriteLine($"Searching...{folder} for {pattern}");
			}

			var files = folder.GetFiles(pattern);
			if (verbose)
			{
				Console.Out.WriteLine($"Found {files.Length} files matching pattern");
			}

			projectFiles.AddRange(files);
			foreach (var subFolder in folder.GetDirectories())
			{
				TraverseAndLocateProjectFiles(projectFiles, subFolder, pattern, verbose);
			}
		}
	}
}

