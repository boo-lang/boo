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
