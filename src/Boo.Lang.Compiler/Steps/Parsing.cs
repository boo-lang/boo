using System;
using System.IO;
using System.Reflection;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{
	public class Parsing : ICompilerStep
	{
		private ICompilerStep _parser;

		public void Initialize(CompilerContext context)
		{
			Parser.Initialize(context);
		}

		public void Run()
		{
			Parser.Run();
		}

		public void Dispose()
		{
			if (_parser != null)
			{
				_parser.Dispose();
				_parser = null;
			}
		}

		private ICompilerStep Parser
		{
			get { return (_parser ?? (_parser = NewParserStep())); }
		}

		static ICompilerStep NewParserStep()
		{
			return (ICompilerStep)Activator.CreateInstance(ConfiguredParserType);
		}

		private static Type ConfiguredParserType
		{
			get
			{
				var parserType = My<CompilerParameters>.Instance.WhiteSpaceAgnostic ? "Boo.Lang.Parser.WSABooParsingStep" : "Boo.Lang.Parser.BooParsingStep";
				return ParserAssembly().GetType(parserType, true);
			}
		}

		static Assembly _parserAssembly;

		static Assembly ParserAssembly()
		{
			return _parserAssembly ?? (_parserAssembly = FindParserAssembly());
		}

		private static Assembly FindParserAssembly()
		{
			var thisLocation = Permissions.WithDiscoveryPermission(() => ThisAssembly().Location) ?? "";
			if (string.IsNullOrEmpty(thisLocation))
				return LoadParserAssemblyByName();
			var parserLocation = thisLocation.EndsWith("Boo.Lang.Compiler.dll")
			                     	? thisLocation.Substring(0, thisLocation.Length - "Boo.Lang.Compiler.dll".Length) + "Boo.Lang.Parser.dll"
			                     	: "";
			return File.Exists(parserLocation) ? Assembly.LoadFrom(parserLocation) : LoadParserAssemblyByName();
		}

		private static Assembly LoadParserAssemblyByName()
		{
			return Assembly.Load(ThisAssembly().FullName.Replace("Boo.Lang.Compiler", "Boo.Lang.Parser"));
		}

		private static Assembly ThisAssembly()
		{
			return typeof(Parsing).Assembly;
		}
	}
}
