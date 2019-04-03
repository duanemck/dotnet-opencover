using System;

namespace duanemckdev.dotnet.tools.testx.runners
{
    public class OpenCoverRunner : ProcessExecutor
    {
        protected readonly string _exe;
        private const string dotnetExe = @"C:\Program Files\dotnet\dotnet.exe";

        public OpenCoverRunner(string workingDirectory, string exe, bool verbose) : base(workingDirectory, verbose)
		{
            _exe = exe;
        }

        public int Run(string project, string filters, string outputFile, bool mergeOutput, string extraOptions)
        {
            var coverArgs = $@"-target:""{dotnetExe}"" -targetargs:""test {project} --logger:trx"" -register:user -oldStyle -output:""{outputFile}"" -filter:""{filters}"" -hideskipped:File -returntargetcode {extraOptions}";
            if (mergeOutput)
            {
                coverArgs += " -mergeOutput";
            }
            return RunAndWait(_exe, coverArgs);
        }
    }
} 