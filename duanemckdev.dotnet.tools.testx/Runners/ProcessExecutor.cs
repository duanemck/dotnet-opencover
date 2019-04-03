﻿using System;
using System.Diagnostics;

namespace duanemckdev.dotnet.tools.testx.runners
{
	public class ProcessExecutor
	{
		private readonly bool _verbose;
		private readonly string _workingDirectory;

		public ProcessExecutor(string workingDirectory, bool verbose)
		{
			_verbose = verbose;
			_workingDirectory = workingDirectory;
		}

		protected int RunAndWait(string exe, string args)
		{
			if (_verbose)
			{
				Console.Out.WriteLine("Executing:");
				Console.Out.WriteLine($"\t{exe} {args}");
				Console.Out.WriteLine("\nNOTE: All further output will be from the process");
				Console.Out.WriteLine("-------------------------------------------------------------");
			}

			ProcessStartInfo psi = new ProcessStartInfo
			{
				Arguments = args,
				FileName = exe,
				WorkingDirectory = _workingDirectory
			};
			var process = Process.Start(psi);
			process?.WaitForExit();
			var exitCode = process?.ExitCode ?? 1;
			return exitCode;
		}
	}
}