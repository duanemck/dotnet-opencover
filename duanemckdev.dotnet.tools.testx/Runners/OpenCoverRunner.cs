using System;

namespace duanemckdev.dotnet.tools.testx.runners
{
    public class OpenCoverRunner : ProcessExecutor
    {
        protected readonly string _exe;
        private const string dotnetExe = @"C:\Program Files\dotnet\dotnet.exe";

        public OpenCoverRunner(string exe, bool verbose) : base(verbose)
        {
            _exe = exe;
        }

        public int Run(string project, string filters, string outputFile, bool mergeOutput)
        {
            var coverArgs = $@"-target:""{dotnetExe}"" -targetargs:""test {project} --logger:trx"" -register:user -oldStyle -output:""{outputFile}"" -filter:""{filters}"" -hideskipped:File";
            if (mergeOutput)
            {
                coverArgs += " -mergeOutput";
            }
            return RunAndWait(_exe, coverArgs);
        }
    }
} 