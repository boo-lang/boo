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
