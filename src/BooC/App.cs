using System;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Xml;
using Assembly = System.Reflection.Assembly;
using Boo.Ast;
using Boo.Ast.Visiting;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.IO;

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
		static void Main(string[] args)
		{
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
				
				if (options.Verbose)
				{			
					Console.WriteLine(Boo.ResourceManager.Format("BooC.ProcessingTime", options.Input.Count, processingTime.TotalMilliseconds, setupTime.TotalMilliseconds));					
				}
			}
			catch (Exception x)
			{
				Console.WriteLine(Boo.ResourceManager.Format("BooC.FatalError", x.Message));
			}			
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
								options.Verbose = true;
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
								Console.WriteLine(Boo.ResourceManager.Format("BooC.InvalidOption", arg));
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
		
		static void LoadPipeline(Pipeline pipeline, string name)
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
