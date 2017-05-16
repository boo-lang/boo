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

using System.Reflection;

namespace BooCompiler.Tests
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Steps;
	using Boo.Lang.Compiler.Pipelines;
	using NUnit.Framework;

	public abstract class AbstractCompilerTestCase
	{
		private BooCompiler _compiler;

		protected CompilerParameters _parameters;

		protected string _baseTestCasesPath;

		protected StringWriter _output;

		protected virtual bool VerifyGeneratedAssemblies
		{
			get
			{
#if MSBUILD
				return true;
#else
				return GetEnvironmentFlag("PEVERIFY", true);
#endif
			}
		}

#if !MSBUILD
		static bool GetEnvironmentFlag(string name, bool defaultValue)		{			var value = Environment.GetEnvironmentVariable(name);
			return value == null ? defaultValue : bool.Parse(value);
		}
#endif

		[TestFixtureSetUp]
		public virtual void SetUpFixture()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			_baseTestCasesPath = Path.Combine(BooTestCaseUtil.TestCasesPath, GetRelativeTestCasesPath());
			_compiler = new BooCompiler();
			_parameters = _compiler.Parameters;
			_parameters.OutputWriter = _output = new StringWriter();
			_parameters.Pipeline = SetUpCompilerPipeline();
			_parameters.References.Add(typeof(AbstractCompilerTestCase).Assembly);
			_parameters.References.Add(typeof(BooCompiler).Assembly);
			Directory.CreateDirectory(TestOutputPath);
			_parameters.OutputAssembly = Path.Combine(TestOutputPath, "testcase.exe");
			_parameters.Defines.Add("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL", null);
			_parameters.GenerateCollectible = false;
			CustomizeCompilerParameters();
			if (VerifyGeneratedAssemblies) CopyDependencies();
		}

		private string TestOutputPath
		{
			get
			{
#if MSBUILD
				return Path.Combine(Path.GetTempPath(), "msbuild");
#else
				return Path.GetTempPath();
#endif
			}
		}

		protected virtual string GetRelativeTestCasesPath()
		{
			return "compilation";
		}

		protected virtual void CustomizeCompilerParameters()
		{
		}

		protected virtual void CopyDependencies()
		{
			CopyAssembly(typeof(Boo.Lang.List).Assembly);
			CopyAssembly(typeof(Boo.Lang.Compiler.Ast.Node).Assembly);
			CopyAssembly(typeof(Boo.Lang.Extensions.MacroMacro).Assembly);
			CopyAssembly(GetType().Assembly);
			CopyAssembly(Assembly.Load("BooSupportingClasses"));
#if !MSBUILD
			CopyAssembly(System.Reflection.Assembly.Load("BooModules"));
#endif
		}
		
		protected void CopyAssembliesFromTestCasePath()
		{
			foreach (string fname in Directory.GetFiles(_baseTestCasesPath, "*.dll"))
				CopyAssembly(fname);
		}

		public void CopyAssembly(Assembly assembly)
		{
			if (null == assembly) throw new ArgumentNullException("assembly");
			CopyAssembly(assembly.Location);
		}
		
		public void CopyAssembly(string location)
		{
			var destFileName = Path.Combine(TestOutputPath, Path.GetFileName(location));
			if (File.Exists(destFileName) && !IsNewer(location, destFileName))
				return;
			File.Copy(location, destFileName, true);
		}

		private bool IsNewer(string fileName, string thanFileName)
		{
			return File.GetLastWriteTime(fileName) > File.GetLastWriteTime(thanFileName);
		}

		[TestFixtureTearDown]
		public virtual void TearDownFixture()
		{	
		}

		[SetUp]
		public virtual void SetUpTest()
		{
			System.Threading.Thread current = System.Threading.Thread.CurrentThread;

			_parameters.OutputType = CompilerOutputType.Auto;
			_parameters.Input.Clear();
			_parameters.Strict = false;
			_parameters.ResetWarnings();
			_parameters.ResetWarningsAsErrors();

			current.CurrentCulture = current.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
		}

		/// <summary>
		/// Override in derived classes to use a different pipeline.
		/// </summary>
		protected virtual CompilerPipeline SetUpCompilerPipeline()
		{
			CompilerPipeline pipeline = VerifyGeneratedAssemblies
				? new CompileToFileAndVerify()
				: new CompileToMemory();

			pipeline.Add(new RunAssembly());
			return pipeline;
		}

		protected virtual void RunCompilerTestCase(string name)
		{
			string fname = GetTestCasePath(name);
			_parameters.Input.Add(new FileInput(fname));
			RunAndAssert();
		}

		protected void RunMultiFileTestCase(params string[] files)
		{
			foreach (string file in files)
				_parameters.Input.Add(new FileInput(GetTestCasePath(file)));
			RunAndAssert();
		}

		protected void RunAndAssert()
		{
			CompilerContext context;
			var output = Run(null, out context);
		    var modules = context.CompileUnit.Modules;
		    Assert.IsTrue(modules.Count > 0, output);
		    var expected = modules[0].Documentation ?? "";
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
		
		private bool HasErrors(CompilerContext context)
		{
			return context.Errors.Count > 0;
		}

		protected string Run(string stdin, out CompilerContext context)
		{
			var oldStdOut = Console.Out;
			var oldStdIn = Console.In;

			try
			{
				Console.SetOut(_output);
				if (stdin != null)
					Console.SetIn(new StringReader(stdin));

				context = _compiler.Run();

				if (HasErrors(context) && !IgnoreErrors)
				{
					Assert.Fail(GetFirstInputName(context)
								+ ": "
								+ context.Errors.ToString(true)
								+ context.Warnings);				
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
			get { return false; }
		}

		string GetFirstInputName(CompilerContext context)
		{
			return context.Parameters.Input[0].Name;
		}
		
		public string BaseTestCasesPath
		{
			get { return _baseTestCasesPath; }
		}

		protected virtual string GetTestCasePath(string fname)
		{
			return Path.Combine(_baseTestCasesPath, fname);
		}

		class AssemblyResolver
		{
			string _path;

			public AssemblyResolver(string path)
			{
				_path = path;
			}

			public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
			{
				string simpleName = GetSimpleName(args.Name);
				string basePath = Path.Combine(_path, simpleName);
				Assembly asm = ProbeFile(basePath + ".dll");
				if (asm != null) return asm;
				return ProbeFile(basePath + ".exe");
			}

			private string GetSimpleName(string name)
			{
				return System.Text.RegularExpressions.Regex.Split(name, ",\\s*")[0];
			}

			private Assembly ProbeFile(string fname)
			{
				if (!File.Exists(fname)) return null;
				try
				{
					return Assembly.LoadFrom(fname);
				}
				catch (Exception x)
				{
					Console.Error.WriteLine(x);
				}
				return null;
			}
		}

		protected ResolveEventHandler InstallAssemblyResolver(string path)
		{
			ResolveEventHandler handler = new AssemblyResolver(path).AssemblyResolve;
			AppDomain.CurrentDomain.AssemblyResolve += handler;
			return handler;
		}

		protected void RemoveAssemblyResolver(ResolveEventHandler handler)
		{
			AppDomain.CurrentDomain.AssemblyResolve -= handler;
		}
	}
}
