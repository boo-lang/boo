using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Boo.Ast.Compilation;

namespace Boo.Tests.Ast.Compilation
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

	/// <summary>	
	/// </summary>
	[TestFixture]
	public class PipelineTestCase : Assertion
	{	
		Pipeline _pipeline;

		[SetUp]
		public void SetUp()
		{
			_pipeline = new Pipeline();
		}

		[Test]
		public void TestConstructor()
		{
			AssertEquals(0, _pipeline.Count);
		}

		[Test]
		public void TestAdd()
		{
			DummyStep p = new DummyStep();			
			_pipeline.Add(p);
			AssertEquals(1, _pipeline.Count);
		}

		[Test]
		public void TestRun()
		{
			DummyStep p1 = new DummyStep();		
			DummyStep p2 = new DummyStep();

			_pipeline.Add(p1);
			_pipeline.Add(p2);

			AssertEquals(0, p1.RunCount);
			AssertEquals(0, p2.RunCount);
			_pipeline.Run(new CompilerContext(new CompilerParameters(), new Boo.Ast.CompileUnit()));
			AssertEquals(1, p1.RunCount);
			AssertEquals(1, p2.RunCount);
		}

		[Test]
		public void TestXmlConfiguration()
		{
			string xml = @"
			<pipeline>
				<step type='Boo.Tests.Ast.Compilation.DummyStep, Boo.Tests' />
			</pipeline>";
			
			_pipeline.Configure(LoadXml(xml));
			
			AssertEquals(1, _pipeline.Count);
			Assert("Expected a DummyStep!", _pipeline[0] is DummyStep);
		}

		[Test]
		public void TestXmlConfigurationExtends()
		{		
			XmlDocument doc = new XmlDocument();
			doc.Load(GetTestCasePath("p2.pipeline"));

			_pipeline.Configure(doc.DocumentElement);

			AssertEquals(2, _pipeline.Count);
			Assert("Expected a DummyStep!", _pipeline[0] is DummyStep);
			Assert("Expected a DummyStep2!", _pipeline[1] is DummyStep2);
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
