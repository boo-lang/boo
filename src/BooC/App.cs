#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Xml;
using Assembly = System.Reflection.Assembly;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Stepss;

namespace BooC
{
	/// <summary>
	/// 
	/// </summary>
	class App
	{
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
				CompilerParameters options = compiler.Parameters;
				
				ParseOptions(args, options);
				if (0 == options.Input.Count)
				{
					throw new ApplicationException(Boo.ResourceManager.GetString("BooC.NoInputSpecified"));
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
				
				if (options.TraceSwitch.TraceWarning)
				{			
					Console.WriteLine(Boo.ResourceManager.Format("BooC.ProcessingTime", options.Input.Count, processingTime.TotalMilliseconds, setupTime.TotalMilliseconds));					
				}
			}
			catch (Exception x)
			{
				Console.WriteLine(Boo.ResourceManager.Format("BooC.FatalError", x.Message));
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
			bool hasPipeline = false;
			
			foreach (string arg in args)
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
								ICompilerPipelineDefinition definition = GetPipelineDefinition(pipelineName);
								if (null == definition)
								{
									options.Pipeline.Load(pipelineName);
								}
								else
								{
									definition.Define(options.Pipeline);
								}
								hasPipeline = true;
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
			
			if (!hasPipeline)
			{
				options.Pipeline.Load(typeof(CompileToFile));
			}
		}

		static Assembly LoadAssembly(string assemblyName)
		{
			Assembly reference = Assembly.LoadWithPartialName(assemblyName);
			if (null == reference)
			{
				string fname = Path.GetFullPath(assemblyName);
				reference = Assembly.LoadFrom(assemblyName);
				if (null == reference)
				{
					throw new ApplicationException(Boo.ResourceManager.Format("BooC.UnableToLoadAssembly", assemblyName));
				}
			}
			return reference;
		}		
		
		static void InvalidOption(string arg)
		{
			Console.WriteLine(Boo.ResourceManager.Format("BooC.InvalidOption", arg));
		}

		static bool IsFlag(string arg)
		{
			return arg[0] == '-' || arg[0] == '/';
		}

		static XmlElement LoadXmlDocument(string fname)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(fname);
			return doc.DocumentElement;
		}

		static void AddFilesForPath(string path, CompilerParameters options)
		{
			foreach (string filename in Directory.GetFiles(path, "*.boo"))
			{
				options.Input.Add(new FileInput(Path.GetFullPath(filename)));
			}
								
			foreach (string dirname in Directory.GetDirectories(path))
			{
				AddFilesForPath(dirname, options);
			}
		}
		
		static ICompilerPipelineDefinition GetPipelineDefinition(string name)
		{
			switch (name)
			{
				case "parse": return new Parse();
				case "core": return new CorePipelineDefinition();
				case "boom": return new CompileToMemory();
				case "booi": return new Run();
				case "booc": return new CompileToFile();
				case "roundtrip": return new RoundtripPipelineDefinition();
				case "boo": return new ParseAndPrint();
				case "xml": return new ParseAndPrintXml();
			}
			return null;
		}
	}
}
