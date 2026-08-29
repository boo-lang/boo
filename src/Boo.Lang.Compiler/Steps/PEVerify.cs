#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Diagnostics;
	using System.IO;

	/// <summary>
	/// Verifies the generated assembly with ilverify.
	/// </summary>
	/// <remarks>
	/// peverify.exe only ships with the .NET Framework SDK and Mono's pedump
	/// fails every .NET image on System.Private.CoreLib, so neither is usable.
	/// ilverify comes from the dotnet-ilverify tool:
	///
	///     dotnet tool install --global dotnet-ilverify
	///
	/// When it is not installed the step warns and passes. BOO_ILVERIFY points
	/// at a specific executable.
	/// </remarks>
	public class PEVerify : AbstractCompilerStep
	{
		override public void Run()
		{
			if (Errors.Count > 0)
				return;

			var verifier = FindVerifier();
			if (verifier == null)
			{
				Context.TraceInfo("ilverify was not found; skipping verification");
				return;
			}

			try
			{
				var process = StartVerifier(verifier);
				var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
				process.WaitForExit();

				if (0 != process.ExitCode)
					Errors.Add(new CompilerError(Ast.LexicalInfo.Empty, output));
			}
			catch (Exception e)
			{
				Warnings.Add(new CompilerWarning("Could not run " + verifier));
				Context.TraceWarning("Could not run " + verifier + " : " + e.Message);
			}
		}

		private Process StartVerifier(string verifier)
		{
			var assembly = Context.GeneratedAssemblyFileName;
			var startInfo = new ProcessStartInfo
			{
				FileName = verifier,
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			};

			startInfo.ArgumentList.Add(assembly);

			// The verifier resolves references itself, so it needs both the
			// runtime's assemblies and whatever sits beside the output.
			startInfo.ArgumentList.Add("-r");
			startInfo.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "*.dll"));

			var outputDirectory = Path.GetDirectoryName(Path.GetFullPath(assembly));
			if (!string.IsNullOrEmpty(outputDirectory))
			{
				startInfo.ArgumentList.Add("-r");
				startInfo.ArgumentList.Add(Path.Combine(outputDirectory, "*.dll"));
			}

			var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
			if (!string.IsNullOrEmpty(runtimeDirectory))
			{
				startInfo.ArgumentList.Add("-r");
				startInfo.ArgumentList.Add(Path.Combine(runtimeDirectory, "*.dll"));
			}

			return Process.Start(startInfo);
		}

		private static string FindVerifier()
		{
			var configured = Environment.GetEnvironmentVariable("BOO_ILVERIFY");
			if (!string.IsNullOrEmpty(configured))
				return File.Exists(configured) ? configured : null;

			foreach (var candidate in CandidatePaths())
				if (File.Exists(candidate))
					return candidate;

			return null;
		}

		private static System.Collections.Generic.IEnumerable<string> CandidatePaths()
		{
			var name = Environment.OSVersion.Platform == PlatformID.Win32NT ? "ilverify.exe" : "ilverify";

			var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			if (!string.IsNullOrEmpty(home))
			{
				yield return Path.Combine(home, ".dotnet", "tools", name);
				yield return Path.Combine(home, ".local", "share", "dotnet", ".dotnet", "tools", name);
			}

			var path = Environment.GetEnvironmentVariable("PATH") ?? "";
			foreach (var dir in path.Split(Path.PathSeparator))
				if (dir.Length > 0)
					yield return Path.Combine(dir, name);
		}
	}
}
