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

namespace BooCompiler.Tests

import System
import System.IO
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.Pipelines
import NUnit.Framework

abstract class AbstractCompilerTestCase:
	private _compiler as BooCompiler

	protected _parameters as CompilerParameters

	protected _baseTestCasesPath as string

	protected _output as StringWriter

	protected virtual VerifyGeneratedAssemblies as bool:
		get:
			ifdef MSBUILD:
				return true
			ifdef not MSBUILD:
				return GetEnvironmentFlag("PEVERIFY", true)

	ifdef not MSBUILD:
		private static def GetEnvironmentFlag(name as string, defaultValue as bool) as bool:
			value = Environment.GetEnvironmentVariable(name)
			return defaultValue if value is null
			return bool.Parse(value)

	[OneTimeSetUp]
	public virtual def SetUpFixture():
		System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture

		_baseTestCasesPath = Path.Combine(BooTestCaseUtil.TestCasesPath, GetRelativeTestCasesPath())
		_compiler = BooCompiler()
		_parameters = _compiler.Parameters
		_output = StringWriter()
		_parameters.OutputWriter = _output
		_parameters.Pipeline = SetUpCompilerPipeline()
		_parameters.References.Add(typeof(AbstractCompilerTestCase).Assembly)
		_parameters.References.Add(typeof(SupportingClasses.ByRef).Assembly)
		_parameters.References.Add(typeof(BooCompiler).Assembly)
		Directory.CreateDirectory(TestOutputPath)
		_parameters.OutputAssembly = Path.Combine(TestOutputPath, "testcase.exe")
		_parameters.Defines.Add("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL", null)
		_parameters.GenerateCollectible = false
		CustomizeCompilerParameters()
		CopyDependencies() if VerifyGeneratedAssemblies

	private TestOutputPath as string:
		get: return Path.Combine(Path.GetTempPath(), "boo-tests", GetType().Name)

	protected virtual def GetRelativeTestCasesPath() as string:
		return "compilation"

	protected virtual def CustomizeCompilerParameters():
		pass

	protected virtual def CopyDependencies():
		CopyAssembly(typeof(Boo.Lang.List).Assembly)
		CopyAssembly(typeof(Boo.Lang.Compiler.Ast.Node).Assembly)
		CopyAssembly(typeof(Boo.Lang.Extensions.MacroMacro).Assembly)
		CopyAssembly(GetType().Assembly)
		CopyAssembly(Assembly.Load("BooSupportingClasses"))
		CopyAssembly(typeof(SupportingClasses.ByRef).Assembly)
		ifdef not MSBUILD:
			CopyAssembly(System.Reflection.Assembly.Load("BooModules"))

	protected def CopyAssembliesFromTestCasePath():
		for fname as string in Directory.GetFiles(_baseTestCasesPath, "*.dll"):
			CopyAssembly(fname)

	public def CopyAssembly(assembly as Assembly):
		raise ArgumentNullException("assembly") if assembly is null
		CopyAssembly(assembly.Location)

	public def CopyAssembly(location as string):
		destFileName = Path.Combine(TestOutputPath, Path.GetFileName(location))
		return if File.Exists(destFileName) and not IsNewer(location, destFileName)
		File.Copy(location, destFileName, true)

	private def IsNewer(fileName as string, thanFileName as string) as bool:
		return File.GetLastWriteTime(fileName) > File.GetLastWriteTime(thanFileName)

	[TearDown]
	public def QueueGeneratedAssemblyForVerification():
		return unless VerifyGeneratedAssemblies

		VerificationQueue.Enqueue(_parameters.OutputAssembly,
			GetType().Name + "." + TestContext.CurrentContext.Test.Name)

	[OneTimeTearDown]
	public virtual def TearDownFixture():
		pass

	[SetUp]
	public virtual def SetUpTest():
		current = System.Threading.Thread.CurrentThread

		_parameters.OutputType = CompilerOutputType.Auto
		_parameters.Input.Clear()
		_parameters.Strict = false
		_parameters.ResetWarnings()
		_parameters.ResetWarningsAsErrors()

		current.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture
		current.CurrentCulture = current.CurrentUICulture

	protected virtual def SetUpCompilerPipeline() as CompilerPipeline:
		"""
		Override in derived classes to use a different pipeline.
		"""
		pipeline as CompilerPipeline
		if VerifyGeneratedAssemblies:
			pipeline = CompileToFile()
		else:
			pipeline = CompileToMemory()

		pipeline.Add(RunAssembly())
		pipeline.Replace(typeof(Boo.Lang.Compiler.Steps.Parsing), BooTestCaseUtil.ParsingStep())
		return pipeline

	protected virtual def RunCompilerTestCase(name as string):
		fname = GetTestCasePath(name)
		_parameters.Input.Add(FileInput(fname))
		RunAndAssert()

	protected def RunMultiFileTestCase(*files as (string)):
		for file in files:
			_parameters.Input.Add(FileInput(GetTestCasePath(file)))
		RunAndAssert()

	protected def RunAndAssert():
		context as CompilerContext = null
		output = Run(null, context)
		modules = context.CompileUnit.Modules
		Assert.IsTrue(modules.Count > 0, output)
		expected = modules[0].Documentation
		expected = "" if expected is null
		Assert.AreEqual(expected.Trim(), output.Trim(), _parameters.Input[0].Name)

	protected def RunString(code as string) as string:
		return RunString(code, null)

	protected def RunString(code as string, stdin as string) as string:
		_parameters.Input.Add(StringInput("<teststring>", code))

		context as CompilerContext = null
		return Run(stdin, context)

	private def HasErrors(context as CompilerContext) as bool:
		return context.Errors.Count > 0

	protected def Run(stdin as string, ref context as CompilerContext) as string:
		oldStdOut = Console.Out
		oldStdIn = Console.In

		try:
			Console.SetOut(_output)
			Console.SetIn(StringReader(stdin)) if stdin is not null

			context = _compiler.Run()

			if HasErrors(context) and not IgnoreErrors:
				Assert.Fail(GetFirstInputName(context)
							+ ": "
							+ context.Errors.ToString(true)
							+ context.Warnings)
			return _output.ToString().Replace("\r\n", "\n")
		ensure:
			_output.GetStringBuilder().Length = 0

			Console.SetOut(oldStdOut)
			Console.SetIn(oldStdIn)

	protected virtual IgnoreErrors as bool:
		get: return false

	private def GetFirstInputName(context as CompilerContext) as string:
		return context.Parameters.Input[0].Name

	public BaseTestCasesPath as string:
		get: return _baseTestCasesPath

	protected virtual def GetTestCasePath(fname as string) as string:
		return Path.Combine(_baseTestCasesPath, fname)

	private class AssemblyResolver:
		private _path as string

		def constructor(path as string):
			_path = path

		public def AssemblyResolve(sender as object, args as ResolveEventArgs) as Assembly:
			simpleName = GetSimpleName(args.Name)
			basePath = Path.Combine(_path, simpleName)
			asm = ProbeFile(basePath + ".dll")
			return asm if asm is not null
			return ProbeFile(basePath + ".exe")

		private def GetSimpleName(name as string) as string:
			return System.Text.RegularExpressions.Regex.Split(name, ",\\s*")[0]

		private def ProbeFile(fname as string) as Assembly:
			return null unless File.Exists(fname)
			try:
				return Assembly.LoadFrom(fname)
			except x as Exception:
				Console.Error.WriteLine(x)
			return null

	protected def InstallAssemblyResolver(path as string) as ResolveEventHandler:
		handler = ResolveEventHandler(AssemblyResolver(path).AssemblyResolve)
		AppDomain.CurrentDomain.AssemblyResolve += handler
		return handler

	protected def RemoveAssemblyResolver(handler as ResolveEventHandler):
		AppDomain.CurrentDomain.AssemblyResolve -= handler
