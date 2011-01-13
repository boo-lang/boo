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
			var parserLocation = thisLocation.EndsWith("Boo.Lang.Compiler.dll", StringComparison.OrdinalIgnoreCase)
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
