#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
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
using Boo.Lang.Ast.Compiler;
using Boo.Lang.Ast.Compiler.IO;

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
				
				Compiler compiler = new Compiler();
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
					foreach (Error error in context.Errors)
					{
						Console.WriteLine(error.ToString(options.Verbose));
					}
					Console.WriteLine(Boo.ResourceManager.Format("BooC.Errors", context.Errors.Count));
				}
				else
				{
					resultCode = 0;
				}
				
				if (options.Verbose)
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

		static void ParseOptions(string[] args, CompilerParameters options)
		{
			bool hasPipeline = false;
			
			foreach (string arg in args)
			{
				if ("-" == arg)
				{
					options.Input.Add(new ReaderInput("<stdin>", Console.In));
				}
				else
				{
					if (IsFlag(arg))
					{
						switch (arg[1])
						{
							case 'v':
							{
								Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
								options.Verbose = true;
								
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
								LoadPipeline(options.Pipeline, arg.Substring(3));								
								hasPipeline = true;
								break;
							}

							case 'c':
							{
								string culture = arg.Substring(3);
								Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);
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
				LoadPipeline(options.Pipeline, "booc");
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
		
		static void LoadPipeline(CompilerPipeline pipeline, string name)
		{			
			if (!name.EndsWith(".pipeline"))
			{				
				name += ".pipeline";
				if (!File.Exists(name) && !Path.IsPathRooted(name))
				{
					name = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
				}
			}
			
			try
			{
				pipeline.Configure(LoadXmlDocument(name));
			}
			catch (Exception x)
			{
				throw new ApplicationException(Boo.ResourceManager.Format("BooC.UnableToLoadPipeline", name, x.Message));
			}
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
	}
}
