using System;

namespace duanemckdev.dotnet.tools.testx.runners
{
    public class CoberturaRunner : ProcessExecutor
    {
        private readonly string _exe;
        
        public CoberturaRunner(string workingDirectory,string exe, bool verbose) : base(workingDirectory,verbose)
        {
            _exe = exe;
        }

        public int Run(string input, string output)
        {
            var coverArgs = $@"-input:{input} -output:{output}";
            return RunAndWait(_exe, coverArgs);
        }
    }
}