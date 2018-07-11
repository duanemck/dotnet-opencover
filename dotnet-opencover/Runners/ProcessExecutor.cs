using System;
using System.Diagnostics;

namespace dotnet_opencover
{
    public class ProcessExecutor
    {
        protected virtual int RunAndWait(string exe, string args)
        {
            Console.Out.WriteLine($"{exe} {args}");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                Arguments = args,
                FileName = exe
            };
            var process = Process.Start(psi);
            process?.WaitForExit();
            return process?.ExitCode ?? 1;
        }
    }
}