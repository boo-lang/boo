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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Threading;
using Assembly = System.Reflection.Assembly;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

namespace BooC
{
	/// <summary>
	/// 
	/// </summary>
	class App
	{
        private static ArrayList responseFileList = new ArrayList();
        private static CompilerParameters options = null;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			int resultCode = -1;
			
			try
			{
				DateTime start = DateTime.Now;
				
				BooCompiler compiler = new BooCompiler();
				options = compiler.Parameters;
				
				ParseOptions(args, options);
				if (0 == options.Input.Count)
				{
					throw new ApplicationException(Boo.ResourceManager.GetString("BooC.NoInputSpecified"));
				}

				if (options.TraceSwitch.TraceInfo)
				{
					compiler.Parameters.Pipeline.BeforeStep += new CompilerStepEventHandler(OnBeforeStep);
					compiler.Parameters.Pipeline.AfterStep += new CompilerStepEventHandler(OnAfterStep);
				}
				
				TimeSpan setupTime = DateTime.Now - start;	
				
				start = DateTime.Now;
				CompilerContext context = compiler.Run();
				TimeSpan processingTime = DateTime.Now - start;				
				
				if (context.Errors.Count > 0)
				{
					foreach (CompilerError error in context.Errors)
					{
						Console.WriteLine(error.ToString(options.TraceSwitch.TraceInfo));
					}
					Console.WriteLine(Boo.ResourceManager.Format("BooC.Errors", context.Errors.Count));
				}
				else
				{
					resultCode = 0;
				}
				
				if (context.Warnings.Count > 0)
				{
					Console.WriteLine(context.Warnings);
					Console.WriteLine(Boo.ResourceManager.Format("BooC.Warnings", context.Warnings.Count));
				}
				
				if (options.TraceSwitch.TraceWarning)
				{						
					Console.WriteLine(Boo.ResourceManager.Format("BooC.ProcessingTime", options.Input.Count, processingTime.TotalMilliseconds, setupTime.TotalMilliseconds));					
				}
			}
			catch (Exception x)
			{
				object message = options.TraceSwitch.TraceWarning ? (object)x : (object)x.Message;
				Console.WriteLine(Boo.ResourceManager.Format("BooC.FatalError", message));
			}			
			return resultCode;
		}
		
		static string Consume(TextReader reader)
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

		static void ParseOptions(string[] args, CompilerParameters options)
		{
			ArrayList arglist = new ArrayList(args);
			ExpandResponseFiles(ref arglist);
			AddDefaultResponseFile(ref arglist);
			foreach (string arg in arglist)
			{
				if ("-" == arg)
				{
					options.Input.Add(new StringInput("<stdin>", Consume(Console.In)));
				}
				else
				{
					if (IsFlag(arg))
					{
						switch (arg[1])
						{
							case 'v':
							{
								options.TraceSwitch.Level = TraceLevel.Warning;
								Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));								
								if (arg.Length > 2)
								{
									switch (arg.Substring(1))
									{
										case "vv":
										{
											options.TraceSwitch.Level = TraceLevel.Info;
											break;
										}
										
										case "vvv":
										{
											options.TraceSwitch.Level = TraceLevel.Verbose;
											break;
										}										
									}
								}
								else
								{
									options.TraceSwitch.Level = TraceLevel.Warning;
								}
								break;
							}

							case 'r':
							{
								string assemblyName = arg.Substring(3);
								options.References.Add(LoadAssembly(assemblyName));
								break;
							}
							
							case 'o':
							{
								options.OutputAssembly = arg.Substring(arg.IndexOf(":")+1);
								break;									
							}
							
							case 't':
							{
								string targetType = arg.Substring(arg.IndexOf(":")+1);
								switch (targetType)
								{
									case "library":
									{
										options.OutputType = CompilerOutputType.Library;
										break;
									}
									
									case "exe":
									{
										options.OutputType = CompilerOutputType.ConsoleApplication;
										break;
									}
									
									case "winexe":
									{
										options.OutputType = CompilerOutputType.WindowsApplication;
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
								string pipelineName = arg.Substring(3);
								options.Pipeline = CompilerPipeline.GetPipeline(pipelineName);
								break;
							}

							case 'c':
							{
								string culture = arg.Substring(3);
								Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);
								break;
							}

							case 's':
							{
								switch (arg.Substring(1, 6))
								{
									case "srcdir":
									{
										string path = Path.GetFullPath(arg.Substring(8));
										AddFilesForPath(path, options);
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

							case 'd':
							{
								switch (arg.Substring(1))
								{
									case "debug":
									case "debug+":
									{
										options.Debug = true;
										break;
									}
									
									case "debug-":
									{
										options.Debug = false;
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
							
							default:
							{
								InvalidOption(arg);								
								break;
							}
						}
					}
					else
					{
						options.Input.Add(new FileInput(Path.GetFullPath(arg)));
					}
				}
			}
			
			if (null == options.Pipeline)
			{
				options.Pipeline = new CompileToFile();
			}
		}

		static ArrayList LoadResponseFile(string file)
		{
			file = Path.GetFullPath(file);
			if (responseFileList.Contains(file))
			{
				throw new ApplicationException(
						Boo.ResourceManager.Format("BCE0500", file));
			}
			responseFileList.Add(file);
			if (!File.Exists(file))
			{
				throw new ApplicationException(Boo.ResourceManager.Format("BCE0501", file));
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
								Boo.ResourceManager.Format("BCE0502", file),
								x);
			}
			return	arglist;
		}
		
		static void ExpandResponseFiles(ref ArrayList arglist)
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

		static void AddDefaultResponseFile(ref ArrayList arglist)
		{
			ArrayList result = new ArrayList();
			bool loadDefault = true;
			foreach (string arg in arglist)
			{
				if (arg == "-noconfig")
				{
					loadDefault = false;
				}
				else
				{
					result.Add(arg);
				}
			}
			if (loadDefault)
			{
				string file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "booc.rsp");
				if (File.Exists(file))
				{
					result.InsertRange(0, LoadResponseFile(file));
				}
			}
			arglist = result;
		}

		static Assembly LoadAssembly(string assemblyName)
		{
			Assembly reference = Assembly.LoadWithPartialName(assemblyName);
			if (null == reference)
			{
				reference = Assembly.LoadFrom(Path.GetFullPath(assemblyName));
				if (null == reference)
				{
					throw new ApplicationException(Boo.ResourceManager.Format("BooC.UnableToLoadAssembly", assemblyName));
				}
			}
			return reference;
		}		
		
		static void OnBeforeStep(object sender, CompilerStepEventArgs args)
		{
			args.Context.TraceEnter("Entering {0}", args.Step);
		}
		
		static void OnAfterStep(object sender, CompilerStepEventArgs args)
		{
			args.Context.TraceLeave("Leaving {0}", args.Step);
		}
		
		static void InvalidOption(string arg)
		{
			Console.WriteLine(Boo.ResourceManager.Format("BooC.InvalidOption", arg));
		}

		static bool IsFlag(string arg)
		{
            return arg[0] == '-';
		}

		static void AddFilesForPath(string path, CompilerParameters options)
		{
			foreach (string fname in Directory.GetFiles(path, "*.boo"))
			{
				options.Input.Add(new FileInput(Path.GetFullPath(fname)));
			}
								
			foreach (string dirName in Directory.GetDirectories(path))
			{
				AddFilesForPath(dirName, options);
			}
		}
	}
}
