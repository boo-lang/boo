#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace BooCompiler.Tests
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
		
		protected StringWriter _output;
		
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
			_parameters.OutputWriter = _output = new StringWriter();
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
			string expected = context.CompileUnit.Modules[0].Documentation;
			if (null == expected)
			{
				expected = "";
			}
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
				Console.SetOut(_output);
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
				return _output.ToString().Replace("\r\n", "\n");
			}
			finally
			{				
				_output.GetStringBuilder().Length = 0;
				
				Console.SetOut(oldStdOut);
				Console.SetIn(oldStdIn);
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
