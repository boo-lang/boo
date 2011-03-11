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

using System;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.IO;
using Boo.Lang.Resources;
using Assembly = System.Reflection.Assembly;
using Boo.Lang.Compiler;
using Boo.Lang;

namespace booc
{
	/// <summary>
	///
	/// </summary>
	public class App
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static int Main(string[] args)
		{
			if (((IList)args).Contains("-utf8"))
				return RunInUtf8Mode(args);
			return AppRun(args);
		}

		private static int AppRun(string[] args)
		{
			return new App().Run(args);
		}

		private static int RunInUtf8Mode(string[] args)
		{
			using (var writer = new StreamWriter(Console.OpenStandardError(), Encoding.UTF8))
			{
				// leave the byte order mark in its own line and out
				writer.WriteLine();
					
				Console.SetError(writer);
				return AppRun(args);
			}
		}

		public int Run(string[] args)
		{
			int resultCode = 127;

			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
			
			CheckBooCompiler();

			var parameters = new CompilerParameters(false);
			try
			{
				var setupTime = Stopwatch.StartNew();

				CommandLineParser.ParseInto(parameters, args);

				if (0 == parameters.Input.Count)
					throw new ApplicationException(StringResources.BooC_NoInputSpecified);
				
				var compiler = new BooCompiler(parameters);
				setupTime.Stop();

				var processingTime = Stopwatch.StartNew();
				var context = compiler.Run();
				processingTime.Stop();

				if (context.Warnings.Count > 0)
				{
					Console.Error.WriteLine(context.Warnings);
					Console.Error.WriteLine(StringResources.BooC_Warnings, context.Warnings.Count);
				}

				if (context.Errors.Count == 0)
					resultCode = 0;
				else
				{
					foreach (CompilerError error in context.Errors)
						Console.Error.WriteLine(error.ToString(parameters.TraceInfo));
					Console.Error.WriteLine(StringResources.BooC_Errors, context.Errors.Count);
				}

				if (parameters.TraceWarning)
					Console.Error.WriteLine(StringResources.BooC_ProcessingTime, parameters.Input.Count,
					                        processingTime.ElapsedMilliseconds, setupTime.ElapsedMilliseconds);
			}
			catch (Exception x)
			{
				var message = (parameters.TraceWarning) ? x : (object)x.Message;
				Console.Error.WriteLine(string.Format(Boo.Lang.Resources.StringResources.BooC_FatalError, message));
			}
			return resultCode;
		}

		static void CheckBooCompiler()
		{
			var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Boo.Lang.Compiler.dll");
			if (!File.Exists(path))
				return;

			foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (!a.FullName.StartsWith("Boo.Lang.Compiler"))
					continue;
				if (string.Compare(a.Location, path, true) != 0)
				{
					//can't use ResourceManager, boo.lang.dll may be out of date
					string msg =
						string.Format(
							"WARNING: booc is not using the Boo.Lang.Compiler.dll next to booc.exe.  Using '{0}' instead of '{1}'.  You may need to remove boo dlls from the GAC using gacutil or Mscorcfg.",
							a.Location, path);
					//has to be all 1 line for things like msbuild that parse booc output.
					Console.Error.WriteLine(msg);
				}
				break;
			}
		}

		static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			var simpleName = args.Name.Split(',')[0];
			var fileName = Path.Combine(Environment.CurrentDirectory, simpleName + ".dll");
			if (File.Exists(fileName))
				return Assembly.LoadFile(fileName);
			return null;
		}
		
	}
}
