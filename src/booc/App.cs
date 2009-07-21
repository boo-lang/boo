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
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Threading;
using Boo.Lang.Compiler.Ast.Visitors;
using Assembly = System.Reflection.Assembly;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Resources;
using Boo.Lang;

namespace BooC
{
	/// <summary>
	///
	/// </summary>
	class App
	{
		ArrayList _responseFileList = new ArrayList();
		CompilerParameters _options = null;
		
		ArrayList _references = new ArrayList();
		ArrayList _packages = new ArrayList();
		bool _noConfig = false;
		string _pipelineName = null;
		bool _debugSteps = false;
		bool _whiteSpaceAgnostic = false;
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			if (((IList)args).Contains("-utf8"))
			{
				using (StreamWriter writer = new StreamWriter(Console.OpenStandardError(), Encoding.UTF8))
				{
					// leave the byte order mark in its own line and out
					writer.WriteLine();
					
					Console.SetError(writer);
					return new App().Run(args);
				}
			}
			else
			{
				return new App().Run(args);
			}
		}
		
		public int Run(string[] args)
		{
			int resultCode = 127;

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
			
			CheckBooCompiler();
			
			try
			{
				Stopwatch setupTime = Stopwatch.StartNew();

				_options = new CompilerParameters(false); //false means no stdlib loading yet
				_options.GenerateInMemory = false;
				
				ArrayList tempLibPaths = _options.LibPaths.Clone() as ArrayList;
				_options.LibPaths.Clear();
				
				BooCompiler compiler = new BooCompiler(_options);
				
				ParseOptions(args);
				
				if (0 == _options.Input.Count)
				{
					throw new ApplicationException(Boo.Lang.ResourceManager.GetString("BooC.NoInputSpecified"));
				}
				
				//move standard libpaths below any new ones:
				foreach(object o in tempLibPaths)
					_options.LibPaths.Add(o);
				
				if (_options.StdLib)
				{
					_options.LoadDefaultReferences();
				}
				else if (!_noConfig)
				{
					_references.Insert(0, "mscorlib");
				}
				
				LoadReferences();
				ConfigurePipeline();
				
				if (_options.TraceInfo)
				{
					compiler.Parameters.Pipeline.BeforeStep += new CompilerStepEventHandler(OnBeforeStep);
					compiler.Parameters.Pipeline.AfterStep += new CompilerStepEventHandler(OnAfterStep);
				}

				setupTime.Stop();

				Stopwatch processingTime = Stopwatch.StartNew();
				CompilerContext context = compiler.Run();
				processingTime.Stop();

				if (context.Warnings.Count > 0)
				{
					Console.Error.WriteLine(context.Warnings);
					Console.Error.WriteLine(Boo.Lang.ResourceManager.Format("BooC.Warnings", context.Warnings.Count));
				}
				
				if (context.Errors.Count > 0)
				{
					foreach (CompilerError error in context.Errors)
					{
						Console.Error.WriteLine(error.ToString(_options.TraceInfo));
					}
					Console.Error.WriteLine(Boo.Lang.ResourceManager.Format("BooC.Errors", context.Errors.Count));
				}
				else
				{
					resultCode = 0;
				}
				
				if (_options.TraceWarning)
				{
					Console.Error.WriteLine(Boo.Lang.ResourceManager.Format("BooC.ProcessingTime", _options.Input.Count, processingTime.ElapsedMilliseconds, setupTime.ElapsedMilliseconds));
				}
			}
			catch (Exception x)
			{
				object message = (_options.TraceWarning)
									? (object)x : (object)x.Message;
				Console.Error.WriteLine(Boo.Lang.ResourceManager.Format("BooC.FatalError", message));
			}
			return resultCode;
		}
		
		void LoadReferences()
		{
			foreach (string r in _references)
			{
				_options.References.Add(_options.LoadAssembly(r, true));
			}
			foreach (string p in _packages)
			{
				_options.LoadReferencesFromPackage(p);
			}
		}
		
		void CheckBooCompiler()
		{
			string path = Path.Combine(Path.GetDirectoryName(
						Assembly.GetExecutingAssembly().Location),
					"Boo.Lang.Compiler.dll");
			if (File.Exists(path))
			{
				foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (a.FullName.StartsWith("Boo.Lang.Compiler"))
					{
						if (string.Compare(a.Location, path, true) != 0)
						{
							//can't use ResourceManager, boo.lang.dll may be out of date
							string msg=string.Format("WARNING: booc is not using the Boo.Lang.Compiler.dll next to booc.exe.  Using '{0}' instead of '{1}'.  You may need to remove boo dlls from the GAC using gacutil or Mscorcfg.",
									a.Location, path);
							//has to be all 1 line for things like msbuild that parse booc output.
							Console.Error.WriteLine(msg);
						}
						break;
					}
				}
			}
		}
		
		string Consume(TextReader reader)
		{
			StringWriter writer = new StringWriter();
			string line = reader.ReadLine();
			while (null != line)
			{
				writer.WriteLine(line);
				line = reader.ReadLine();
			}
			return writer.ToString();
		}
		
		void DoLogo()
		{
			Console.WriteLine("Boo Compiler version {0} ({1})",
				Builtins.BooVersion.ToString(), Boo.Lang.Runtime.RuntimeServices.RuntimeDisplayName);
		}
		
		void Help ()
		{
			Console.WriteLine(
					"Usage: booc [options] file1 ...\n" +
					"Options:\n" +
					" -c:CULTURE           Sets the UI culture to be CULTURE\n" +
					" -debug[+|-]          Generate debugging information (default: +)\n" +
					" -define:S1[,Sn]      Defines symbols S1..Sn with optional values (=val) (-d:)\n" +
					" -delaysign           Delays assembly signing\n" +
					" -ducky               Turns on duck typing by default\n" +
					" -checked[+|-]        Turns on or off checked operations (default: +)\n" +
					" -embedres:FILE[,ID]  Embeds FILE with the optional ID\n"+
					" -lib:DIRS            Adds the comma-separated DIRS to the assembly search path\n" +
					" -noconfig            Does not load the standard configuration\n" +
					" -nostdlib            Does not reference any of the default libraries\n" +
					" -nologo              Does not display the compiler logo\n" +
					" -nowarn[:W1,Wn]      Suppress all or a list of compiler warnings\n" +
					" -p:PIPELINE          Sets the pipeline to PIPELINE\n" +
					" -o:FILE              Sets the output file name to FILE\n" +
					" -keyfile:FILE        The strongname key file used to strongname the assembly\n" +
					" -keycontainer:NAME   The key pair container used to strongname the assembly\n" +
					" -reference:A1[,An]   References assemblies (-r:)\n" +
					" -srcdir:DIR          Adds DIR as a directory where sources can be found\n" +
					" -target:TYPE         Sets the target type (exe, library or winexe)\n" +
					" -resource:FILE[,ID]  Embeds FILE as a resource\n" +
					" -pkg:P1[,Pn]         References packages P1..Pn (on supported platforms)\n" +
					" -strict              Turns on strict mode.\n" +
					" -unsafe              Allows to compile unsafe code.\n" +
					" -platform:ARCH       Specifies target platform (anycpu, x86, x64 or itanium)\n" +
					" -utf8                Source file(s) are in utf8 format\n" +
					" -v, -vv, -vvv        Sets verbosity level from warnings to very detailed\n" +
					" -warn:W1[,Wn]        Enables a list of optional warnings.\n" +
					" -warnaserror[:W1,Wn] Treats all or a list of warnings as errors\n" +
					" -wsa                 Enables white-space-agnostic build\n"
					);
		}

		void ParseOptions(string[] args)
		{
			bool noLogo = false;
			
			ArrayList arglist = new ArrayList(args);
			ExpandResponseFiles(ref arglist);
			AddDefaultResponseFile(ref arglist);
			foreach (string arg in arglist)
			{
				if ("-" == arg)
				{
					_options.Input.Add(new StringInput("<stdin>", Consume(Console.In)));
					continue;
				}
				if (!IsFlag(arg))
				{
					_options.Input.Add(new FileInput(StripQuotes(arg)));
					continue;
				}
				if ("-utf8" == arg) continue;
				
				switch (arg[1])
				{
					case 'h':
					{
						if (arg == "-help" || arg == "-h")
						{
							Help();
						}
						break;
					}
					
					case 'w':
					{
						if (arg == "-wsa")
						{
							_options.WhiteSpaceAgnostic = _whiteSpaceAgnostic = true;
						}
						else if (arg == "-warnaserror")
						{
							_options.WarnAsError = true;
						}
						else if (arg.StartsWith("-warnaserror:"))
						{
							string warnings = StripQuotes(arg.Substring(arg.IndexOf(":")+1));
							foreach (string warning in warnings.Split(','))
							{
								_options.EnableWarningAsError(warning);
							}
						}
						else if (arg.StartsWith("-warn:"))
						{
							string warnings = StripQuotes(arg.Substring(arg.IndexOf(":")+1));
							foreach (string warning in warnings.Split(','))
							{
								_options.EnableWarning(warning);
							}
						}
						else
						{
							InvalidOption(arg);
						}
						break;
					}
					
					case 'v':
					{
						_options.EnableTraceSwitch();
						_options.TraceLevel = TraceLevel.Warning;
						Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
						if (arg.Length > 2)
						{
							switch (arg.Substring(1))
							{
								case "vv":
								{
									_options.TraceLevel = TraceLevel.Info;
									MonitorAppDomain();
									break;
								}
								
								case "vvv":
								{
									_options.TraceLevel = TraceLevel.Verbose;
									break;
								}
							}
						}
						else
						{
							_options.TraceLevel = TraceLevel.Warning;
						}
						break;
					}

					case 'r':
					{
						if (arg.IndexOf(":") > 2 && arg.Substring(1, 9) != "reference")
						{
							switch (arg.Substring(1, 8))
							{
								case "resource":
								{	
									int start = arg.IndexOf(":") + 1;
									AddResource(StripQuotes(arg.Substring(start)));
									break;
								}

								default:
								{
									InvalidOption(arg);
									break;
								}
							}
						}
						else
						{
							string assemblies = StripQuotes(arg.Substring(arg.IndexOf(":")+1));
							foreach (string assemblyName in assemblies.Split(','))
							{
								_references.Add(assemblyName);
							}
						}
						break;
					}
					
					case 'l':
					{
						switch (arg.Substring(1, 3))
						{
							case "lib":
							{
								string paths = arg.Substring(arg.IndexOf(":")+1);
								if (paths == "")
								{
									Console.Error.WriteLine(Boo.Lang.ResourceManager.Format("BooC.BadLibPath", arg));
									break;
								}
								
								foreach(string dir in paths.Split(new Char[] {','}))
								{
									if (Directory.Exists(dir))
									{
										_options.LibPaths.Add(dir);
									}
									else
									{
										Console.Error.WriteLine(Boo.Lang.ResourceManager.Format("BooC.BadLibPath", dir));
									}
								}
								break;
							}

							default:
							{
								InvalidOption(arg);
								break;
							}
						}
						break;
					}
					
					case 'n':
					{
						if (arg == "-nologo")
						{
							noLogo = true;
						}
						else if (arg == "-noconfig")
						{
							_noConfig = true;
						}
						else if (arg == "-nostdlib")
						{
							_options.StdLib = false;
						}
						else if (arg == "-nowarn")
						{
							_options.NoWarn = true;
						}
						else if (arg.StartsWith("-nowarn:"))
						{
							string warnings = StripQuotes(arg.Substring(arg.IndexOf(":")+1));
							foreach (string warning in warnings.Split(','))
							{
								_options.DisableWarning(warning);
							}
						}
						else
						{
							InvalidOption(arg);
						}
						break;
					}
					
					case 'o':
					{
						_options.OutputAssembly = StripQuotes(arg.Substring(arg.IndexOf(":")+1));
						break;
					}
					
					case 't':
					{
						string targetType = arg.Substring(arg.IndexOf(":")+1);
						switch (targetType)
						{
							case "library":
							{
								_options.OutputType = CompilerOutputType.Library;
								break;
							}
							
							case "exe":
							{
								_options.OutputType = CompilerOutputType.ConsoleApplication;
								break;
							}
							
							case "winexe":
							{
								_options.OutputType = CompilerOutputType.WindowsApplication;
								break;
							}
							
							default:
							{
								InvalidOption(arg);
								break;
							}
						}
						break;
					}

					case 'p':
					{
						if (arg.StartsWith("-pkg:"))
						{
							string packages = StripQuotes(arg.Substring(arg.IndexOf(":")+1));
							_packages.Add(packages);
						}
						else if (arg.StartsWith("-platform:"))
						{
							string arch = StripQuotes(arg.Substring(arg.IndexOf(":")+1)).ToLowerInvariant();
							switch (arch)
							{
								case "anycpu":
									break;
								case "x86":
									_options.Platform = "x86";
									break;
								case "x64":
									_options.Platform = "x64";
									break;
								case "itanium":
									_options.Platform = "itanium";
									break;
								default:
									InvalidOption(arg, "Valid platform types are: `anycpu', `x86', `x64' or `itanium'.");
									break;
							}
						}
						else if (arg.StartsWith("-p:"))
						{
							_pipelineName = StripQuotes(arg.Substring(3));
						}
						else
						{
							InvalidOption(arg);
						}
						break;
					}

					case 'c':
					{
						switch (arg.Substring(1))
						{
							case "checked":
							case "checked+":
							{
								_options.Checked = true;
								break;
							}
							
							case "checked-":
							{
								_options.Checked = false;
								break;
							}
							
							default:
							{
								string culture = arg.Substring(3);
								Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);
								break;
							}
						}
						break;
					}

					case 's':
					{
						switch (arg.Substring(1, 6))
						{
							case "srcdir":
							{
								string path = StripQuotes(arg.Substring(8));
								AddFilesForPath(path, _options);
								break;
							}

							case "strict-":
								break;

							case "strict":
							case "strict+":
							{
								_options.Strict = true;
								break;
							}

							default:
							{
								InvalidOption(arg);
								break;
							}
						}
						break;
					}
					
					case 'k':
					{
						if (arg.Substring(1, 7) == "keyfile")
						{
							_options.KeyFile = StripQuotes(arg.Substring(9));
						}
						else if (arg.Substring(1, 12) == "keycontainer")
						{
							_options.KeyContainer = StripQuotes(arg.Substring(14));
						}	
						else
						{
							InvalidOption(arg);
						}
						break;
					}
					
					case 'd':
					{
						switch (arg.Substring(1))
						{
							case "debug":
							case "debug+":
							{
								_options.Debug = true;
								break;
							}
							
							case "debug-":
							{
								_options.Debug = false;
								break;
							}
							
							case "ducky":
							{
								_options.Ducky = true;
								break;
							}

							case "debug-steps":
							{
								_debugSteps = true;
								break;
							}
							
							case "delaysign":
							{
								_options.DelaySign = true;
								break;
							}
							
							default:
							{
								if (arg.StartsWith("-d:") || arg.StartsWith("-define:"))
								{
									int skip = arg.StartsWith("-d:") ? 3 : 8;
									string[] symbols = StripQuotes(arg.Substring(skip)).Split(",".ToCharArray());
									foreach (string symbol in symbols)
									{
										string[] s_v = symbol.Split("=".ToCharArray(), 2);
										if (s_v[0].Length < 1) continue;
										if (_options.Defines.ContainsKey(s_v[0]))
										{
											_options.Defines[s_v[0]] = (s_v.Length > 1) ? s_v[1] : null;
											Trace.WriteLine("REPLACED DEFINE '"+s_v[0]+"' WITH VALUE '"+((s_v.Length > 1) ? s_v[1] : string.Empty) +"'");
										}
										else
										{
											_options.Defines.Add(s_v[0], (s_v.Length > 1) ? s_v[1] : null);
											Trace.WriteLine("ADDED DEFINE '"+s_v[0]+"' WITH VALUE '"+((s_v.Length > 1) ? s_v[1] : string.Empty) +"'");
										}
									}
								}
								else
								{
									InvalidOption(arg);
								}
								break;
							}
						}
						break;
					}
					
					case 'e':
					{
						switch (arg.Substring(1,8))
						{
							case "embedres":
							{
								if (!IsMono)
								{
									throw new ApplicationException("-embedres is only supported on mono. Try -resource.");
								}
								int start = arg.IndexOf(":") + 1;
								EmbedResource(StripQuotes(arg.Substring(start)));
								break;
							}

							default:
							{
								InvalidOption(arg);
								break;
							}
						}
						break;
					}

					case 'u':
					{
						if (arg == "-unsafe")
							_options.Unsafe = true;
						else
							InvalidOption(arg);
						break;
					}

					default:
					{
						if (arg == "--help")
						{
							Help();
						}
						else
						{
							InvalidOption(arg);
						}
						break;
					}
				}
			}
			
			if (!noLogo)
			{
				DoLogo();
			}
		}

		private bool IsMono
		{
			get { return Type.GetType("Mono.Runtime") != null;  }
		}

		private void EmbedResource(string resourceFile)
		{

			int comma = resourceFile.LastIndexOf(',');
			if (comma >= 0)
			{
				string resourceName = resourceFile.Substring(comma+1);
				resourceFile = resourceFile.Substring(0, comma);
				_options.Resources.Add(new NamedEmbeddedFileResource(resourceFile, resourceName));
			}
			else
			{
				_options.Resources.Add(new EmbeddedFileResource(resourceFile));
			}
		}

		private void AddResource(string resourceFile)
		{
			int comma = resourceFile.LastIndexOf(',');
			if (comma >= 0)
			{
				string resourceName = resourceFile.Substring(comma+1);
				resourceFile = resourceFile.Substring(0, comma);
				_options.Resources.Add(new NamedFileResource(resourceFile, resourceName));
			}
			else
			{
				_options.Resources.Add(new FileResource(resourceFile));
			}
		}

		private void ConfigurePipeline()
		{
			if (null != _pipelineName)
			{
				_options.Pipeline = CompilerPipeline.GetPipeline(_pipelineName);
			}
			else
			{
				_options.Pipeline = new CompileToFile();
			}			
			if (_whiteSpaceAgnostic)
			{
				_options.Pipeline[0] = new Boo.Lang.Parser.WSABooParsingStep();
			}
			if (_debugSteps)
			{
				_options.Pipeline.AfterStep += new CompilerStepEventHandler(new StepDebugger().AfterStep);
			}
		}
		
		private string StripQuotes(string s)
		{
			if (s.Length > 1 && s.StartsWith("\"") && s.EndsWith("\""))
			{
				return s.Substring(1,s.Length-2);
			}
			return s;
		}
		
		private class StepDebugger
		{
			string _last;
			
			public void AfterStep(object sender, CompilerStepEventArgs args)
			{
				Console.WriteLine("********* {0} *********", args.Step);
				
				StringWriter writer = new StringWriter();
				args.Context.CompileUnit.Accept(new BooPrinterVisitor(writer, BooPrinterVisitor.PrintOptions.PrintLocals));
				string code = writer.ToString();
				if (code != _last)
				{
					Console.WriteLine(code);
				}
				else
				{
					Console.WriteLine("no changes");
				}
				_last = code;
			}
		}

		ArrayList LoadResponseFile(string file)
		{
			file = Path.GetFullPath(file);
			if (_responseFileList.Contains(file))
			{
				throw new ApplicationException(
						Boo.Lang.ResourceManager.Format("BCE0500", file));
			}
			_responseFileList.Add(file);
			if (!File.Exists(file))
			{
				throw new ApplicationException(Boo.Lang.ResourceManager.Format("BCE0501", file));
			}
			ArrayList arglist = new ArrayList();
			try
			{
				using (StreamReader sr = new StreamReader(file))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						line = line.Trim();
						if (line.Length > 0 && line[0] != '#')
						{
							if (line.StartsWith("@") && line.Length > 2)
							{
								arglist.AddRange(LoadResponseFile(line.Substring(1)));
							}
							else
							{
								arglist.Add(line);
							}
						}
					}
				}
			}
			catch (ApplicationException)
			{
				throw;
			}
			catch (Exception x)
			{
				throw new ApplicationException(
								Boo.Lang.ResourceManager.Format("BCE0502", file),
								x);
			}
			return	arglist;
		}
		
		void ExpandResponseFiles(ref ArrayList arglist)
		{
			ArrayList result = new ArrayList();
			foreach (string arg in arglist)
			{
				if (arg.StartsWith("@") && arg.Length > 2)
				{
					result.AddRange(LoadResponseFile(arg.Substring(1)));
				}
				else
				{
					result.Add(arg);
				}
			}
			arglist = result;
		}

		void AddDefaultResponseFile(ref ArrayList arglist)
		{
			ArrayList result = new ArrayList();
			foreach (string arg in arglist)
			{
				if (arg == "-noconfig")
				{
					_noConfig = true;
				}
				else
				{
					result.Add(arg);
				}
			}
			if (!_noConfig)
			{
				string file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "booc.rsp");
				if (File.Exists(file))
				{
					result.InsertRange(0, LoadResponseFile(file));
				}
			}
			arglist = result;
		}

		Stopwatch stepStopwatch;

		void OnBeforeStep(object sender, CompilerStepEventArgs args)
		{
			args.Context.TraceEnter("Entering {0}", args.Step);
			stepStopwatch = Stopwatch.StartNew();
		}
		
		void OnAfterStep(object sender, CompilerStepEventArgs args)
		{
			stepStopwatch.Stop();
			args.Context.TraceLeave("Leaving {0} ({1}ms)", args.Step, stepStopwatch.ElapsedMilliseconds);
		}

		void InvalidOption(string arg)
		{
			InvalidOption(arg, null);
		}

		void InvalidOption(string arg, string message)
		{
			Console.Error.WriteLine(Boo.Lang.ResourceManager.Format("BooC.InvalidOption", arg, message));
		}

		bool IsFlag(string arg)
		{
			return arg[0] == '-';
		}

		void AddFilesForPath(string path, CompilerParameters _options)
		{
			foreach (string fname in Directory.GetFiles(path, "*.boo"))
			{
				if (!fname.EndsWith(".boo")) continue;
				_options.Input.Add(new FileInput(Path.GetFullPath(fname)));
			}
								
			foreach (string dirName in Directory.GetDirectories(path))
			{
				AddFilesForPath(dirName, _options);
			}
		}
		
		void MonitorAppDomain()
		{
			if (_options.TraceInfo)
			{
				AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(OnAssemblyLoad);
				foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())
				{
					Trace.WriteLine("ASSEMBLY AT STARTUP: "+GetAssemblyLocation(a));
				}
			}
		}
		
		static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			Trace.WriteLine("ASSEMBLY LOADED: " + GetAssemblyLocation(args.LoadedAssembly));
		}
		
		static string GetAssemblyLocation(Assembly a)
		{
			string loc;
			try
			{
				loc = a.Location;
			}
			catch (Exception)
			{
				loc = "<dynamic>"+a.FullName;
			}
			return loc;
		}
		
		static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string simpleName = args.Name.Split(',')[0];
			string fileName = Path.Combine(Environment.CurrentDirectory, 
							simpleName + ".dll");
			if (File.Exists(fileName))
			{
				return Assembly.LoadFile(fileName);
			}
			return null;
		}
		
	}
}
