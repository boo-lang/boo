#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
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

namespace Boo.Tests.Lang.Compiler
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
			_pipeline.BaseDirectory = BooTestCaseUtil.GetTestCasePath("compilation");			
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
			_pipeline.Run(new CompilerContext(new CompilerParameters(), new Boo.Lang.Ast.CompileUnit()));
			Assert.AreEqual(1, p1.RunCount);
			Assert.AreEqual(1, p2.RunCount);
		}

		[Test]
		public void SimpleXmlConfiguration()
		{
			string xml = @"
			<pipeline>
				<step type='Boo.Tests.Lang.Compiler.DummyStep, Boo.Tests' />
			</pipeline>";
			
			_pipeline.Configure(LoadXml(xml));
			
			Assert.AreEqual(1, _pipeline.Count);
			Assert.IsTrue(_pipeline[0] is DummyStep, "Expected a DummyStep!");
		}

		[Test]
		public void XmlConfigurationExtends()
		{				
			_pipeline.Load("p2");

			Assert.AreEqual(2, _pipeline.Count);
			Assert.AreEqual(typeof(DummyStep), _pipeline[0].GetType(), "Expected a DummyStep!");
			Assert.AreEqual(typeof(DummyStep2), _pipeline[1].GetType(), "Expected a DummyStep2!");
		}
		
		[Test]
		public void ExtendedXmlConfiguration()
		{			
			_pipeline.Load("p3");
			
			Assert.AreEqual(4, _pipeline.Count);
			Assert.AreEqual(typeof(DummyStep3), _pipeline[0].GetType());
			Assert.AreEqual(typeof(DummyStep), _pipeline[1].GetType());
			Assert.AreEqual(typeof(DummyStep4), _pipeline[2].GetType());
			Assert.AreEqual(typeof(DummyStep2), _pipeline[3].GetType());
		}
		
		void AssertPipeline(params ICompilerStep[] expected)
		{
			Assert.AreEqual(expected.Length, _pipeline.Count);
			for (int i=0; i<expected.Length; ++i)
			{
				Assert.AreSame(expected[i], _pipeline[i]);
			}
		}

		string GetTestCasePath(string fname)
		{
			return Path.Combine(BooTestCaseUtil.GetTestCasePath("compilation"), fname);
		}

		XmlElement LoadXml(string xml)
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml(xml);
			return document.DocumentElement;
		}
	}
}
