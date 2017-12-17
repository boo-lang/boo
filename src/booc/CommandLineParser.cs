#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast.Visitors;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Resources;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;
using Boo.Lang.Resources;

namespace booc
{
	public class CommandLineParser
	{
		public static void ParseInto(CompilerParameters options, params string[] commandLine)
		{
			new CommandLineParser(commandLine, options);
		}

		readonly CompilerParameters _options;
		readonly Set<string> _processedResponseFiles = new Set<string>();
		readonly List<string> _references = new List<string>();
		readonly List<string> _packages = new List<string>();
		bool _noConfig;
		string _pipelineName;
		bool _debugSteps;

		private CommandLineParser(IEnumerable<string> args, CompilerParameters options)
		{
			_options = options;
            _options.GenerateCollectible = false;
			_options.GenerateInMemory = false;

			var tempLibPaths = _options.LibPaths.ToArray();
			_options.LibPaths.Clear();

			Parse(args);

			//move standard libpaths below any new ones:
			_options.LibPaths.Extend(tempLibPaths);

			if (_options.StdLib)
				_options.LoadDefaultReferences();
			else if (!_noConfig)
				_references.Insert(0, "mscorlib");

			LoadReferences();
			ConfigurePipeline();

			if (_options.TraceInfo)
			{
				_options.Pipeline.BeforeStep += OnBeforeStep;
				_options.Pipeline.AfterStep += OnAfterStep;
			}

		}

		void Parse(IEnumerable<string> commandLine)
		{
			var noLogo = false;

			var args = ExpandResponseFiles(commandLine.Select(s => StripQuotes(s)));
			AddDefaultResponseFile(args);
			foreach (var arg in args)
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
				if ("-utf8" == arg)
					continue;

				switch (arg[1])
				{
					case 'h':
						{
							if (arg == "-help" || arg == "-h")
								Help();
							break;
						}

					case 'w':
						{
							if (arg == "-wsa")
								_options.WhiteSpaceAgnostic = true;
							else if (arg == "-warnaserror")
								_options.WarnAsError = true;
							else if (arg.StartsWith("-warnaserror:"))
							{
								string warnings = ValueOf(arg);
								foreach (string warning in warnings.Split(','))
									_options.EnableWarningAsError(warning);
							}
							else if (arg.StartsWith("-warn:"))
							{
								var warnings = ValueOf(arg);
								foreach (string warning in warnings.Split(','))
									_options.EnableWarning(warning);
							}
							else
								InvalidOption(arg);
							break;
						}

					case 'v':
						{
							_options.TraceLevel = TraceLevel.Warning;
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
											AddResource(ValueOf(arg));
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
								var assemblies = ValueOf(arg);
								foreach (var assemblyName in assemblies.Split(','))
									_references.Add(assemblyName);
							}
							break;
						}

					case 'l':
						{
							switch (arg.Substring(1, 3))
							{
								case "lib":
									{
										ParseLib(arg);
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
								noLogo = true;
							else if (arg == "-noconfig")
								_noConfig = true;
							else if (arg == "-nostdlib")
								_options.StdLib = false;
							else if (arg == "-nowarn")
								_options.NoWarn = true;
							else if (arg.StartsWith("-nowarn:"))
							{
								string warnings = ValueOf(arg);
								foreach (string warning in warnings.Split(','))
									_options.DisableWarning(warning);
							}
							else
								InvalidOption(arg);
							break;
						}

					case 'o':
						{
							_options.OutputAssembly = ValueOf(arg);
							break;
						}

					case 't':
						{
							string targetType = ValueOf(arg);
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
								string packages = ValueOf(arg);
								_packages.Add(packages);
							}
							else if (arg.StartsWith("-platform:"))
							{
								string arch = ValueOf(arg).ToLowerInvariant();
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
											string[] symbols = ValueOf(arg).Split(",".ToCharArray());
											foreach (string symbol in symbols)
											{
												string[] s_v = symbol.Split("=".ToCharArray(), 2);
												if (s_v[0].Length < 1) continue;
												if (_options.Defines.ContainsKey(s_v[0]))
												{
													_options.Defines[s_v[0]] = (s_v.Length > 1) ? s_v[1] : null;
													TraceInfo("REPLACED DEFINE '" + s_v[0] + "' WITH VALUE '" + ((s_v.Length > 1) ? s_v[1] : string.Empty) + "'");
												}
												else
												{
													_options.Defines.Add(s_v[0], (s_v.Length > 1) ? s_v[1] : null);
													TraceInfo("ADDED DEFINE '" + s_v[0] + "' WITH VALUE '" + ((s_v.Length > 1) ? s_v[1] : string.Empty) + "'");
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
							switch (arg.Substring(1, 8))
							{
								case "embedres":
									{
										EmbedResource(ValueOf(arg));
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

					case 'x':
						{
							if (arg.Substring(1).StartsWith("x-type-inference-rule-attribute"))
							{
								var attribute = ValueOf(arg);
								_options.Environment = new DeferredEnvironment { { typeof(TypeInferenceRuleProvider), () => new CustomTypeInferenceRuleProvider(attribute) } };
							}
							else
							{
								InvalidOption(arg);
							}
							break;
						}

                    case 'i':
                        {
                            string icon = arg.Substring(3).Trim();
                            _options.Icon = icon;
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

		private static string ValueOf(string arg)
		{
			return StripQuotes(arg.Substring(arg.IndexOf(":") + 1));
		}

		private void ParseLib(string arg)
		{
			var paths = TrimAdditionalQuote(ValueOf(arg)); // TrimAdditionalQuote to work around nant bug with spaces on lib path
			if (string.IsNullOrEmpty(paths))
			{
				Console.Error.WriteLine(string.Format(Boo.Lang.Resources.StringResources.BooC_BadLibPath, arg));
				return;
			}
			foreach (var dir in paths.Split(','))
			{
				if (Directory.Exists(dir))
					_options.LibPaths.Add(dir);
				else
					Console.Error.WriteLine(string.Format(Boo.Lang.Resources.StringResources.BooC_BadLibPath, dir));
			}
		}

		static void DoLogo()
		{
			Console.WriteLine("Boo Compiler version {0} ({1})",
				Boo.Lang.Builtins.BooVersion, Boo.Lang.Runtime.RuntimeServices.RuntimeDisplayName);
		}

		static void Help()
		{
			Console.WriteLine(
					"Usage: booc [options] file1 ...\n" +
					"Options:\n" +
					" -c:CULTURE           Sets the UI culture to be CULTURE\n" +
					" -checked[+|-]        Turns on or off checked operations (default: +)\n" +
					" -debug[+|-]          Generate debugging information (default: +)\n" +
					" -define:S1[,Sn]      Defines symbols S1..Sn with optional values (=val) (-d:)\n" +
					" -delaysign           Delays assembly signing\n" +
					" -ducky               Turns on duck typing by default\n" +
					" -embedres:FILE[,ID]  Embeds FILE with the optional ID\n" +
                    " -i:ICON              Sets the generated assembly's icon to the specified file\n" +
					" -keycontainer:NAME   The key pair container used to strongname the assembly\n" +
					" -keyfile:FILE        The strongname key file used to strongname the assembly\n" +
					" -lib:DIRS            Adds the comma-separated DIRS to the assembly search path\n" +
					" -noconfig            Does not load the standard configuration\n" +
					" -nologo              Does not display the compiler logo\n" +
					" -nostdlib            Does not reference any of the default libraries\n" +
					" -nowarn[:W1,Wn]      Suppress all or a list of compiler warnings\n" +
					" -o:FILE              Sets the output file name to FILE\n" +
					" -p:PIPELINE          Sets the pipeline to PIPELINE\n" +
					" -pkg:P1[,Pn]         References packages P1..Pn (on supported platforms)\n" +
					" -platform:ARCH       Specifies target platform (anycpu, x86, x64 or itanium)\n" +
					" -reference:A1[,An]   References assemblies (-r:)\n" +
					" -resource:FILE[,ID]  Embeds FILE as a resource\n" +
					" -srcdir:DIR          Adds DIR as a directory where sources can be found\n" +
					" -strict              Turns on strict mode.\n" +
					" -target:TYPE         Sets the target type (exe, library or winexe) (-t:)\n" +
					" -unsafe              Allows to compile unsafe code.\n" +
					" -utf8                Source file(s) are in utf8 format\n" +
					" -v, -vv, -vvv        Sets verbosity level from warnings to very detailed\n" +
					" -warn:W1[,Wn]        Enables a list of optional warnings.\n" +
					" -warnaserror[:W1,Wn] Treats all or a list of warnings as errors\n" +
					" -wsa                 Enables white-space-agnostic build\n"
					);
		}

		private void EmbedResource(string resourceFile)
		{
			int comma = resourceFile.LastIndexOf(',');
			if (comma >= 0)
			{
				string resourceName = resourceFile.Substring(comma + 1);
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
				string resourceName = resourceFile.Substring(comma + 1);
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
			var pipeline = _pipelineName != null ? CompilerPipeline.GetPipeline(_pipelineName) : new CompileToFile();
			_options.Pipeline = pipeline;
			if (_debugSteps)
			{
				var stepDebugger = new StepDebugger();
				pipeline.BeforeStep += stepDebugger.BeforeStep;
				pipeline.AfterStep += stepDebugger.AfterStep;
			}
		}

		private static string StripQuotes(string s)
		{
			if (s.Length > 1 && (IsDelimitedBy(s, "\"") || IsDelimitedBy(s, "'")))
				return s.Substring(1, s.Length - 2);
			return s;
		}

		private static bool IsDelimitedBy(string s, string delimiter)
		{
			return s.StartsWith(delimiter) && s.EndsWith(delimiter);
		}

		private static string TrimAdditionalQuote(string s)
		{
			return s.EndsWith("\"") ? s.Substring(0, s.Length - 1) : s;
		}

		private class StepDebugger
		{
			private string _last;
			private Stopwatch _stopWatch;

			public void BeforeStep(object sender, CompilerStepEventArgs args)
			{
				_stopWatch = Stopwatch.StartNew();
			}

			public void AfterStep(object sender, CompilerStepEventArgs args)
			{
				_stopWatch.Stop();
				Console.WriteLine("********* {0} - {1} *********", args.Step, _stopWatch.Elapsed);

				var writer = new StringWriter();
				args.Context.CompileUnit.Accept(new BooPrinterVisitor(writer, BooPrinterVisitor.PrintOptions.PrintLocals));
				var code = writer.ToString();
				if (code != _last)
					Console.WriteLine(code);
				else
					Console.WriteLine("no changes");
				_last = code;
			}
		}

		List<string> LoadResponseFile(string file)
		{
			file = Path.GetFullPath(file);
			if (_processedResponseFiles.Contains(file))
				throw new ApplicationException(string.Format(Boo.Lang.Resources.StringResources.BCE0500, file));
			_processedResponseFiles.Add(file);
			if (!File.Exists(file))
				throw new ApplicationException(string.Format(Boo.Lang.Resources.StringResources.BCE0501, file));

			var arglist = new List<string>();
			try
			{
				using (var sr = new StreamReader(file))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						line = line.Trim();
						if (line.Length > 0 && line[0] != '#')
						{
							if (line.StartsWith("@") && line.Length > 2)
								arglist.AddRange(LoadResponseFile(line.Substring(1)));
							else
								arglist.Add(StripQuotes(line));
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
				throw new ApplicationException(string.Format(Boo.Lang.Resources.StringResources.BCE0502, file), x);
			}
			return arglist;
		}

		List<string> ExpandResponseFiles(IEnumerable<string> args)
		{
			var result = new List<string>();
			foreach (var arg in args)
			{
				if (arg.StartsWith("@") && arg.Length > 2)
					result.AddRange(LoadResponseFile(arg.Substring(1)));
				else
					result.Add(arg);
			}
			return result;
		}

		void AddDefaultResponseFile(List<string> args)
		{
			if (!args.Contains("-noconfig"))
			{
				string file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "booc.rsp");
				if (File.Exists(file))
					args.InsertRange(0, LoadResponseFile(file));
			}
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
			Console.Error.WriteLine(StringResources.BooC_InvalidOption, arg, message);
		}

		static bool IsFlag(string arg)
		{
			return arg[0] == '-';
		}

		static void AddFilesForPath(string path, CompilerParameters options)
		{
			foreach (var fname in Directory.GetFiles(path, "*.boo"))
			{
				if (!fname.EndsWith(".boo")) continue;
				options.Input.Add(new FileInput(Path.GetFullPath(fname)));
			}

			foreach (var dirName in Directory.GetDirectories(path))
				AddFilesForPath(dirName, options);
		}


		void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			TraceInfo("ASSEMBLY LOADED: " + GetAssemblyLocation(args.LoadedAssembly));
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
				loc = "<dynamic>" + a.FullName;
			}
			return loc;
		}
		

		void MonitorAppDomain()
		{
			if (_options.TraceInfo)
			{
				AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
				foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
					TraceInfo("ASSEMBLY AT STARTUP: " + GetAssemblyLocation(a));
			}
		}

		private void TraceInfo(string s)
		{
			if (_options.TraceInfo)
				Console.Error.WriteLine(s);
		}

		static string Consume(TextReader reader)
		{
			var writer = new StringWriter();
			var line = reader.ReadLine();
			while (null != line)
			{
				writer.WriteLine(line);
				line = reader.ReadLine();
			}
			return writer.ToString();
		}

		void LoadReferences()
		{
			foreach (var r in _references)
				_options.References.Add(_options.LoadAssembly(r, true));
			foreach (var p in _packages)
				_options.LoadReferencesFromPackage(p);
		}

		
	}
}
