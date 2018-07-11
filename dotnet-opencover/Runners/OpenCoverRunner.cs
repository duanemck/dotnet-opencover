using System;

namespace dotnet_opencover
{
    public class OpenCoverRunner : ProcessExecutor
    {
        private readonly string _exe;
        private const string dotnetExe = @"C:\Program Files\dotnet\dotnet.exe";

        public OpenCoverRunner(string exe)
        {
            _exe = exe;
        }

        public int Run(string project, string filters)
        {
            var coverArgs = $@"-target:""{dotnetExe}"" -targetargs:""test {project} --logger:trx"" -register:user -oldStyle";
            return RunAndWait(_exe, coverArgs);
        }
    }
} 