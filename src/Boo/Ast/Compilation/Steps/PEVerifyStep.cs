using System;
using System.Diagnostics;
using Boo.Ast.Compilation;

namespace Boo.Ast.Compilation.Steps
{
	public class PEVerifyStep : AbstractCompilerStep
	{
		public override void Run()
		{			
			if (Errors.Count > 0)
			{
				return;
			}			
			
			if (128 == (int)System.Environment.OSVersion.Platform)
			{
				_context.TraceVerbose("PEVerifyStep can't run on linux");
				// linux
				return;
			}
			
			Process p = new Process();
			p.StartInfo.Arguments = CompilerParameters.OutputAssembly;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = "peverify.exe";
			p.Start();
			p.WaitForExit();
			
			if (0 != p.ExitCode)
			{
				Errors.Add(new Error(LexicalInfo.Empty, p.StandardOutput.ReadToEnd()));
			}
		}
	}
}
