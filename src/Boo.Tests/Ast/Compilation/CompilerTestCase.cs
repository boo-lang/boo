using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Boo.Ast.Compilation;
using Boo.Ast.Compilation.IO;
using Boo.Ast.Compilation.Steps;
using NUnit.Framework;

namespace Boo.Tests.Ast.Compilation
{
	[TestFixture]
	public class CompilerTestCase
	{
		public static string NewLine = Environment.NewLine;
		
		Compiler _compiler;
		
		CompilerParameters _parameters;
		
		string _testCasesPath;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			 Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Error));

			string booAssemblyPath = typeof(Compiler).Assembly.Location;
			string thisAssemblyPath = GetType().Assembly.Location;
			File.Copy(booAssemblyPath, Path.Combine(Path.GetTempPath(), Path.GetFileName(booAssemblyPath)), true);
			File.Copy(thisAssemblyPath, Path.Combine(Path.GetTempPath(), Path.GetFileName(thisAssemblyPath)), true);
			
			_testCasesPath = Path.Combine(Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).AbsolutePath), "../testcases/compilation");
			
			_compiler = new Compiler();
			_parameters = _compiler.Parameters;
			//_parameters.TraceSwitch.Level = TraceLevel.Verbose;
			_parameters.OutputAssembly = Path.Combine(Path.GetTempPath(), "testcase.exe");
			_parameters.Pipeline.
							Add(new BooParsingStep()).
							Add(new UsingResolutionStep()).
							Add(new AstAttributesStep()).
							Add(new ModuleStep()).
							Add(new AstNormalizationStep()).
							Add(new AssemblySetupStep()).
							Add(new SemanticStep()).
							Add(new EmitAssemblyStep()).
							Add(new SaveAssemblyStep()).
							Add(new PEVerifyStep()).
							Add(new RunAssemblyStep());
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			Trace.Listeners.Clear();
		}
		
		[SetUp]
		public void SetUpTest()
		{
			_parameters.Input.Clear();
		}		
		
		[Test]
		public void TestDefaultOutputType()
		{
			Assert.AreEqual(CompilerOutputType.ConsoleApplication, _parameters.OutputType,
					"Default compiler output type must be ConsoleApplication."); 
		}
		
		[Test]
		public void TestDefaultAssemblyReferences()
		{
			AssemblyCollection references = _parameters.References;
			Assert.AreEqual(3, references.Count);
			Assert.IsTrue(references.Contains(typeof(string).Assembly), "(ms)corlib.dll must be referenced by default!");
			Assert.IsTrue(references.Contains(Assembly.LoadWithPartialName("System")), "System.dll must be referenced by default!");
			Assert.IsTrue(references.Contains(typeof(Boo.Lang.Builtins).Assembly), "Boo.dll must referenced by default!");
		}
		
		[Test]
		public void TestHello()
		{
			Assert.AreEqual("Hello!" + NewLine, RunString("print('Hello!')"));
		}
		
		[Test]
		public void TestHello2()
		{
			string stdin = "Test2" + NewLine;
			string code = "name = prompt(''); print(\"Hello, ${name}!\")";
			Assert.AreEqual("Hello, Test2!" + NewLine, RunString(code, stdin));			
		}
		
		[Test]
		public void TestUsingSimpleNamespace()
		{
			RunCompilerTestCase("using0.boo", "using System");
		}		
			
		[Test]
		public void TestUsingQualifiedType()
		{
			RunCompilerTestCase("using1.boo", "using System.Console");
		}
		
		[Test]
		public void TestUsingQualifiedNamespace()
		{
			RunCompilerTestCase("using2.boo", "using System.Text");
		}
		
		[Test]
		public void TestUsingAssemblyQualifiedNamespace()
		{
			RunCompilerTestCase("using3.boo", "using System.Drawing from System.Drawing");
		}
		
		[Test]
		public void TestUsingAlias()
		{
			RunCompilerTestCase("using4.boo", "using System as S");
		}
		
		[Test]
		public void TestUsingAssemblyQualifiedNamespace2()
		{
			RunCompilerTestCase("using5.boo", "using System.Drawing from different assembly");
		}
		
		[Test]
		public void TestUsingSameAssemblyQualifiedNamespaces()
		{
			RunCompilerTestCase("using6.boo", "using System.Drawing from two assemblies");
		}
		
		[Test]
		public void TestSimpleFor()
		{
			RunCompilerTestCase("for0.boo", "for item in list");
		}
		
		[Test]
		public void TestTypedFor()
		{
			RunCompilerTestCase("for1.boo", "for item as string in list");
		}
		
		[Test]
		public void TestUnpackFor()
		{
			RunCompilerTestCase("for2.boo", "for first, second in list");
		}
	
		void RunCompilerTestCase(string name, string description)
		{			
			_parameters.Input.Add(new FileInput(GetTestCasePath(name)));
			
			CompilerContext context;
			string output = Run(null, out context);
			Assert.AreEqual(_parameters.Input.Count, context.CompileUnit.Modules.Count, "compilation must generate as many modules as were compiler inputs");
			string expected = context.CompileUnit.Modules[0].Documentation;
			Assert.AreEqual(expected, output, description);
		}
		
		string RunString(string code)
		{	
			return RunString(code, null);
		}
		
		string RunString(string code, string stdin)
		{
			_parameters.Input.Add(new StringInput("<teststring>", code));
			
			CompilerContext context;
			return Run(stdin, out context);
		}
		
		string Run(string stdin, out CompilerContext context)
		{
			TextWriter oldStdOut = Console.Out;
			TextReader oldStdIn = Console.In;
			
			try
			{
				StringWriter console = new StringWriter();
				Console.SetOut(console);
				if (null != stdin)
				{
					Console.SetIn(new StringReader(stdin));
				}
				
				context = _compiler.Run();
				
				if (context.Errors.Count > 0)
				{
					Assert.Fail(context.Errors.ToString(true));
				}
				return console.ToString();
			}
			finally
			{				
				Console.SetOut(oldStdOut);
				if (null != stdin)
				{
					Console.SetIn(oldStdIn);
				}
			}
		}
		
		string GetTestCasePath(string fname)
		{
			return Path.Combine(_testCasesPath, fname);
		}
	}
}
