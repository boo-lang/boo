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
using System.IO;
using System.Xml;
using NUnit.Framework;
using Boo.Lang.Compiler;

namespace Boo.Lang.Compiler.Tests
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
		[ExpectedException(typeof(ArgumentException))]
		public void CantAddStepWithTheSameID()
		{
			_pipeline.Add(new CompilerPipelineItem("dummy", new DummyStep()));
			_pipeline.Add(new CompilerPipelineItem("dummy", new DummyStep()));
		}
		
		[Test]
		public void InsertBefore()
		{
			DummyStep first = new DummyStep();
			DummyStep second = new DummyStep();
			
			_pipeline.Add(new CompilerPipelineItem("first", first));
			_pipeline.Add(new CompilerPipelineItem("second", second));
			
			AssertPipeline(first, second);
			
			DummyStep before1 = new DummyStep();
			_pipeline.InsertBefore("first", before1);
			
			AssertPipeline(before1, first, second);
			
			DummyStep before2 = new DummyStep();
			_pipeline.InsertBefore("second", before2);
			
			AssertPipeline(before1, first, before2, second);
		}
		
		[Test]
		public void InsertAfter()
		{
			DummyStep first = new DummyStep();
			_pipeline.Add(new CompilerPipelineItem("first", first));
			
			DummyStep second = new DummyStep();
			_pipeline.InsertAfter("first", second);
			AssertPipeline(first, second);
			
			DummyStep third = new DummyStep();
			_pipeline.InsertAfter("first", new CompilerPipelineItem("third", third));
			AssertPipeline(first, third, second);
			
			DummyStep fourth = new DummyStep();
			_pipeline.InsertAfter("third", fourth);
			AssertPipeline(first, third, fourth, second);
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
				Assert.AreSame(expected[i], _pipeline[i].CompilerStep);
			}
		}
	}
}
