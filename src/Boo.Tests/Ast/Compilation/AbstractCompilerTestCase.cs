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

namespace Boo.Tests.Lang.Compiler
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Boo.Lang.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Pipeline;
	using NUnit.Framework;
	
	public abstract class AbstractCompilerTestCase
	{
		protected BooCompiler _compiler;
		
		protected CompilerParameters _parameters;
		
		protected string _baseTestCasesPath;
		
		[TestFixtureSetUp]
		public virtual void SetUpFixture()
		{
			 Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Error));

			string booAssemblyPath = typeof(BooCompiler).Assembly.Location;
			string thisAssemblyPath = GetType().Assembly.Location;
			File.Copy(booAssemblyPath, Path.Combine(Path.GetTempPath(), Path.GetFileName(booAssemblyPath)), true);
			File.Copy(thisAssemblyPath, Path.Combine(Path.GetTempPath(), Path.GetFileName(thisAssemblyPath)), true);
			
			_baseTestCasesPath = Path.Combine(BooTestCaseUtil.TestCasesPath, "compilation");
			
			_compiler = new BooCompiler();
			_parameters = _compiler.Parameters;
			//_parameters.TraceSwitch.Level = TraceLevel.Verbose;
			_parameters.OutputAssembly = Path.Combine(Path.GetTempPath(), "testcase.exe");
			SetUpCompilerPipeline(_parameters.Pipeline);
			
		}
		
		[TestFixtureTearDown]
		public virtual void TearDownFixture()
		{
			Trace.Listeners.Clear();
		}
		
		[SetUp]
		public virtual void SetUpTest()
		{
			_parameters.Input.Clear();
		}		
		
		protected abstract void SetUpCompilerPipeline(CompilerPipeline pipeline);
		
		protected void RunCompilerTestCase(string name)
		{
			RunCompilerTestCase(name, GetTestCasePath(name));
		}
		
		protected void RunCompilerTestCase(string name, string description)
		{			
			_parameters.Input.Add(new FileInput(GetTestCasePath(name)));
			
			CompilerContext context;
			string output = Run(null, out context);
			Assert.AreEqual(_parameters.Input.Count, context.CompileUnit.Modules.Count, "compilation must generate as many modules as were compiler inputs");
			string expected = context.CompileUnit.Modules[0].Documentation;
			Assert.AreEqual(expected.Trim(), output.Trim(), description);
		}
		
		protected string RunString(string code)
		{	
			return RunString(code, null);
		}
		
		protected string RunString(string code, string stdin)
		{
			_parameters.Input.Add(new StringInput("<teststring>", code));
			
			CompilerContext context;
			return Run(stdin, out context);
		}
		
		protected string Run(string stdin, out CompilerContext context)
		{
			TextWriter oldStdOut = Console.Out;
			TextReader oldStdIn = Console.In;
			
			try
			{
				StringWriter console = new StringWriter();
				Console.SetOut(console);
				if (null != stdin)
				{
					Console.SetIn(new StringReader(stdin));
				}
				
				context = _compiler.Run();
				
				if (context.Errors.Count > 0)
				{
					if (!IgnoreErrors)
					{
						Assert.Fail(GetFirstInputName(context) + ": " + context.Errors.ToString(false));
					}
				}
				return console.ToString().Replace("\r\n", "\n");
			}
			finally
			{				
				Console.SetOut(oldStdOut);
				if (null != stdin)
				{
					Console.SetIn(oldStdIn);
				}
			}
		}
		
		protected virtual bool IgnoreErrors
		{
			get
			{
				return false;
			}
		}
		
		string GetFirstInputName(CompilerContext context)
		{
			return context.CompilerParameters.Input[0].Name;
		}
		
		protected virtual string GetTestCasePath(string fname)
		{
			return Path.Combine(_baseTestCasesPath, fname);
		}
	}
}
