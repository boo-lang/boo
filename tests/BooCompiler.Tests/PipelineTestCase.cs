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
using System.IO;
using System.Xml;
using NUnit.Framework;
using Boo.Lang.Compiler;

namespace BooCompiler.Tests
{
	public class DummyStep : ICompilerStep
	{		
		int _runCount = 0;
		
		public void Initialize(CompilerContext context)
		{			
		}

		public void Run()
		{
			++_runCount;
		}
		
		public void Dispose()
		{			
		}

		public int RunCount
		{
			get
			{
				return _runCount;
			}
		}
	}

	public class DummyStep2 : DummyStep
	{
	}
	
	public class DummyStep3 : DummyStep
	{
	}
	
	public class DummyStep4 : DummyStep
	{
	}

	/// <summary>	
	/// </summary>
	[TestFixture]
	public class PipelineTestCase
	{	
		CompilerPipeline _pipeline;

		[SetUp]
		public void SetUp()
		{
			_pipeline = new CompilerPipeline();			
		}

		[Test]
		public void TestConstructor()
		{
			Assert.AreEqual(0, _pipeline.Count);
		}

		[Test]
		public void TestAdd()
		{
			DummyStep p = new DummyStep();			
			_pipeline.Add(p);
			Assert.AreEqual(1, _pipeline.Count);
		}

		[Test]
		public void TestRun()
		{
			DummyStep p1 = new DummyStep();		
			DummyStep p2 = new DummyStep();

			_pipeline.Add(p1);
			_pipeline.Add(p2);

			Assert.AreEqual(0, p1.RunCount);
			Assert.AreEqual(0, p2.RunCount);
			_pipeline.Run(new CompilerContext(new CompilerParameters(), new Boo.Lang.Compiler.Ast.CompileUnit()));
			Assert.AreEqual(1, p1.RunCount);
			Assert.AreEqual(1, p2.RunCount);
		}
		
		void AssertPipeline(params ICompilerStep[] expected)
		{
			Assert.AreEqual(expected.Length, _pipeline.Count);
			for (int i=0; i<expected.Length; ++i)
			{
				Assert.AreSame(expected[i], _pipeline[i]);
			}
		}
	}
}
