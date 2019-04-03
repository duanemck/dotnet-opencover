namespace duanemckdev.dotnet.tools.testx.runners
{
    public class ReportGeneratorRunner : ProcessExecutor
    {
        private readonly string _exe;

        public ReportGeneratorRunner(string workingDirectory, string exe, bool verbose) : base(workingDirectory, verbose)
		{
            _exe = exe;
        }

        public int GenerateReport(string report, string output)
        {
            var args = $"-reports:{report} -targetdir:{output}";
            return RunAndWait(_exe, args);
        }
    }
}