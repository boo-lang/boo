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
﻿
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.Pipelines;
using NUnit.Framework;

namespace BooCompiler.Tests
{
	class CaptureContext : ICompilerStep
	{
		CompilerContext _context;
		
		public void Initialize(CompilerContext context)
		{
			_context = context;
		}
		
		public void Run()
		{
		}
		
		public void Dispose()
		{
		}
		
		public CompilerContext Context
		{
			get
			{
				return _context;
			}
		}
	}
	
	[TestFixture]
	public class CompilerTestFixture
	{
		Boo.Lang.Compiler.BooCompiler _compiler;
		
		[SetUp]
		public void SetUp()
		{
			_compiler = new Boo.Lang.Compiler.BooCompiler();
		}
		
		[Test]
		public void DefaultDebugSetting()
		{
			Assert.AreEqual(true, _compiler.Parameters.Debug, "Debug must be true by default");
		}
		
		[Test]
		public void DefaultPipeline()
		{
			Assert.IsNull(_compiler.Parameters.Pipeline, "Pipeline must be null!");
		}
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void RunWithoutPipeline()
		{			
			_compiler.Run();
		}
		
		[Test]
		public void RunWithPipeline()
		{
			CaptureContext capture = new CaptureContext();
			
			_compiler.Parameters.Pipeline = new CompilerPipeline();
			_compiler.Parameters.Pipeline.Add(capture);
			
			CompilerContext context = _compiler.Run();
			Assert.IsNotNull(context);
			Assert.AreSame(context, capture.Context);
		}
		
		[Test]
		public void DefaultOutputType()
		{
			Assert.AreEqual(CompilerOutputType.ConsoleApplication, _compiler.Parameters.OutputType,
					"Default compiler output type must be ConsoleApplication."); 
		}		
		
		[Test]
		public void DefaultAssemblyReferences()
		{
			AssemblyCollection references = _compiler.Parameters.References;
			Assert.AreEqual(4, references.Count);
			Assert.IsTrue(references.Contains(typeof(string).Assembly), "(ms)corlib.dll must be referenced by default!");
			Assert.IsTrue(references.Contains(Assembly.LoadWithPartialName("System")), "System.dll must be referenced by default!");
			Assert.IsTrue(references.Contains(typeof(Boo.Lang.Builtins).Assembly), "Boo.dll must referenced by default!");
			Assert.IsTrue(references.Contains(typeof(Boo.Lang.Compiler.BooCompiler).Assembly), "Boo.Lang.Compiler.dll must be referenced by default!");
		}
	}
}
