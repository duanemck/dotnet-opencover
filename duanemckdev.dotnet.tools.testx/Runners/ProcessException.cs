using System;

namespace duanemckdev.dotnet.tools.testx.runners
{
    public class ProcessException : Exception
    {
        public ProcessException(int exitCode, string message):base(message)
        {
            
        }
    }
}