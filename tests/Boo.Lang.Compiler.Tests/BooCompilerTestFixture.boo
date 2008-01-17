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

namespace Boo.Lang.Compiler.Tests

import System
import System.IO
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import NUnit.Framework

class CaptureContext(ICompilerStep):

	[getter(CompilerContext)]
	_context as CompilerContext
	
	def Initialize(context as CompilerContext):
		_context = context
	
	def Run():
		pass
	
	def Dispose():
		pass

[TestFixture]
class CompilerTestFixture:

	_compiler as Boo.Lang.Compiler.BooCompiler
	
	[SetUp]
	def SetUp():
		_compiler = Boo.Lang.Compiler.BooCompiler()
	
	[Test]
	def DefaultDebugSetting():
		Assert.AreEqual(true, _compiler.Parameters.Debug, "Debug must be true by default")
	
	[Test]
	def DefaultPipeline():
		Assert.IsNull(_compiler.Parameters.Pipeline, "Pipeline must be null!")
	
	[Test]
	[ExpectedException(InvalidOperationException)]
	def RunWithoutPipeline():
		_compiler.Run()
	
	[Test]
	def RunWithPipeline():
		capture = CaptureContext()
		
		_compiler.Parameters.Pipeline = CompilerPipeline()
		_compiler.Parameters.Pipeline.Add(capture)
		
		context = _compiler.Run()
		Assert.IsNotNull(context)
		Assert.AreSame(context, capture.CompilerContext)
	
	[Test]
	def DefaultOutputType():
		Assert.AreEqual(CompilerOutputType.ConsoleApplication, _compiler.Parameters.OutputType,
				"Default compiler output type must be ConsoleApplication.")
	
	[Test]
	def DefaultAssemblyReferences():
		references = _compiler.Parameters.References
		Assert.AreEqual(4, references.Count)
		Assert.IsTrue(references.Contains(typeof(string).Assembly), "(ms)corlib.dll must be referenced by default!")
		Assert.IsTrue(references.Contains(Assembly.LoadWithPartialName("System")), "System.dll must be referenced by default!")
		Assert.IsTrue(references.Contains(typeof(Boo.Lang.Builtins).Assembly), "Boo.dll must referenced by default!")
		Assert.IsTrue(references.Contains(typeof(Boo.Lang.Extensions.PrintMacro).Assembly), "Boo.Lang.Extensions.dll must be referenced by default!")
		
	[Test]
	def DefaultGenerateInMemory():
		assert _compiler.Parameters.GenerateInMemory
		
	[Test]
	def CompileOnlyToFile():
		_compiler.Parameters.GenerateInMemory = false
		_compiler.Parameters.Pipeline = Boo.Lang.Compiler.Pipelines.CompileToFile()
		_compiler.Parameters.Input.Add(StringInput("foo", "print 'foo'"))
		_compiler.Parameters.OutputAssembly = fname = Path.GetTempFileName()
		
		context = _compiler.Run()
		Assert.AreEqual(0, len(context.Errors), context.Errors.ToString(true))
		assert File.Exists(fname)
		assert context.GeneratedAssembly is null 
		
		asm = System.Reflection.Assembly.LoadFrom(fname)
		assert asm is not null
		assert asm.EntryPoint is not null
		types = asm.GetTypes()
		Assert.AreEqual(1, len(types))
		writer = System.IO.StringWriter()
		saved = Console.Out
		Console.SetOut(writer)
		try:
			asm.EntryPoint.Invoke(null, (array(string, 0), ))
		ensure:
			Console.SetOut(saved)
		Assert.AreEqual("foo", writer.ToString().Trim())


	[Test]
	def VerboseCompile():
		_compiler.Parameters.TraceSwitch.Level = System.Diagnostics.TraceLevel.Verbose; 
		CompileOnlyToFile()
		
		
