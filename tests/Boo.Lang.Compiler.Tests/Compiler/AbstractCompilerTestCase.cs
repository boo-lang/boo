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

namespace Boo.Lang.Compiler.Tests
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Steps;
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
			
			CopyAssembly(typeof(Boo.Lang.List).Assembly);
			CopyAssembly(typeof(Boo.Lang.Compiler.BooCompiler).Assembly);
			CopyAssembly(GetType().Assembly);
			CopyAssembly(typeof(NUnit.Framework.Assert).Assembly);
			
			_baseTestCasesPath = Path.Combine(BooTestCaseUtil.TestCasesPath, "compilation");
			
			_compiler = new BooCompiler();
			_parameters = _compiler.Parameters;
			//_parameters.TraceSwitch.Level = TraceLevel.Verbose;
			_parameters.OutputAssembly = Path.Combine(Path.GetTempPath(), "testcase.exe");
			_parameters.Pipeline = SetUpCompilerPipeline();
			_parameters.References.Add(typeof(NUnit.Framework.Assert).Assembly);			
		}
		
		public void CopyAssembly(System.Reflection.Assembly assembly)
		{
			string location = assembly.Location;
			File.Copy(location, Path.Combine(Path.GetTempPath(), Path.GetFileName(location)), true);			
		}
		
		[TestFixtureTearDown]
		public virtual void TearDownFixture()
		{
			Trace.Listeners.Clear();
		}
		
		[SetUp]
		public virtual void SetUpTest()
		{
			System.Threading.Thread current = System.Threading.Thread.CurrentThread;
			
			_parameters.Input.Clear();			
			
			current.CurrentCulture = current.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;			
		}		
		
		protected abstract CompilerPipeline SetUpCompilerPipeline();
		
		protected void RunCompilerTestCase(string name)
		{					
			string fname = GetTestCasePath(name);
			_parameters.Input.Add(new FileInput(fname));
			RunAndAssert();
		}
		
		protected void RunMultiFileTestCase(params string[] files)
		{
			foreach (string file in files)
			{
				_parameters.Input.Add(new FileInput(GetTestCasePath(file)));
			}
			RunAndAssert();
		}
		
		protected void RunAndAssert()
		{			
			CompilerContext context;
			string output = Run(null, out context);
			Assert.AreEqual(_parameters.Input.Count, context.CompileUnit.Modules.Count, "compilation must generate as many modules as were compiler inputs");
			string expected = context.CompileUnit.Modules[0].Documentation;
			Assert.AreEqual(expected.Trim(), output.Trim(), _parameters.Input[0].Name);
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
			return context.Parameters.Input[0].Name;
		}
		
		protected virtual string GetTestCasePath(string fname)
		{
			return Path.Combine(_baseTestCasesPath, fname);
		}
	}
}
