using System;
using System.IO;
using System.Linq;

namespace dotnet_opencover
{
    public abstract class NugetExeResolver
    {
        protected abstract string PackageName { get; }
        protected abstract string ExeName { get; }

        public string Resolve(string specificVersion)
        {
            var userProfile = Environment.GetEnvironmentVariable("userprofile");

            if (string.IsNullOrEmpty(userProfile))
            {
                throw new Exception($"Couldn't find userprofile folder at environment variable 'userprofile'. Used to find packages in %userprofile%/.nuget/packages/");
            }

            var pathToExe = Path.Combine(userProfile, ".nuget", "packages", PackageName);

            if (!Directory.Exists(pathToExe))
            {
                throw new Exception($"Couldn't find {PackageName} folder at {pathToExe}. Have you used dotnet restore on a project with the {PackageName} dependancy?");
            }

            if (specificVersion != null)
            {
                var availableVersions = Directory.GetDirectories(pathToExe);

                pathToExe = Path.Combine(pathToExe, specificVersion);

                if (!Directory.Exists(pathToExe))
                {
                    throw new Exception($"Couldn't find the {PackageName} version at {pathToExe}. Version specified: {specificVersion}. Available: '{string.Join("', '", availableVersions)}'");
                }
            }
            else
            {
                pathToExe = Path.Combine(pathToExe, Directory.GetDirectories(pathToExe).OrderByDescending(d => d).First());
            }

            // Older versions don't use the nuget tools folder system
            if (Directory.Exists(Path.Combine(pathToExe, "tools")))
            {
                pathToExe = Path.Combine(pathToExe, "tools");
            }

            pathToExe = Path.Combine(pathToExe, ExeName);

            if (!File.Exists(pathToExe))
            {
                throw new Exception($"Couldn't find {ExeName} at {pathToExe}");
            }
            return pathToExe;
        }
    }
}