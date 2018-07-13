using System;

namespace duanemckdev.dotnet.tools.testx.runners
{
    public class ProcessException : Exception
    {
        public int ExitCode { get; }

        public ProcessException(int exitCode, string message):base(message)
        {
            ExitCode = exitCode;
        }
    }
}