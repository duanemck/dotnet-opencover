using System;
using System.Diagnostics;

namespace duanemckdev.dotnet.tools.testx.runners
{
    public class ProcessExecutor
    {
        protected virtual int RunAndWait(string exe, string args)
        {
            Console.Out.WriteLine("Executing:");
            Console.Out.WriteLine($"\t{exe} {args}");
            Console.Out.WriteLine("\nNOTE: All further output will be from the process");
            Console.Out.WriteLine("-------------------------------------------------------------");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                Arguments = args,
                FileName = exe
            };
            var process = Process.Start(psi);
            process?.WaitForExit();
            var exitCode = process?.ExitCode ?? 1;            
            if (exitCode != 0)
            {
                throw new ProcessException(exitCode, $"{exe} exited with non-zero exit code");
            }

            return exitCode;
        }
    }
}