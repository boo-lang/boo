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
	public enum TestEnum
	{
		Foo = 5,
		Bar = 10,
		Baz = 11
	}
	
	public class Person
	{
		string _fname;
		string _lname;
		
		public Person()
		{			
		}
		
		public string FirstName
		{
			get
			{
				return _fname;
			}
			
			set
			{
				_fname = value;
			}
		}
		
		public string LastName
		{
			get
			{
				return _lname;
			}
			
			set
			{
				_lname = value;
			}
		}
	}
	
	public class Clickable
	{
		public Clickable()
		{			
		}
		
		public event EventHandler Click;
		
		public void RaiseClick()
		{
			if (null != Click)
			{
				Click(this, EventArgs.Empty);
			}
		}
	}
	
	[TestFixture]
	public class CompilerTestCase
	{
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
			
			_testCasesPath = Path.Combine(GetBaseBooDirectory(), "testcases/compilation");
			
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
			Assert.AreEqual("Hello!\n", RunString("print('Hello!')"));
		}
		
		[Test]
		public void TestHello2()
		{
			string stdin = "Test2\n";
			string code = "name = prompt(''); print(\"Hello, ${name}!\")";
			Assert.AreEqual("Hello, Test2!\n", RunString(code, stdin));			
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
		
		[Test]
		public void TestIfModifier0()
		{
			RunCompilerTestCase("if0.boo", "write() if true");
		}
		
		[Test]
		public void TestList0()
		{
			RunCompilerTestCase("list0.boo", "[]");
		}
		
		[Test]
		public void TestList1()
		{
			RunCompilerTestCase("list1.boo", "[1, 2, 3]");
		}
		
		[Test]
		public void TestEnum0()
		{
			RunCompilerTestCase("enum0.boo", "TestEnum.Foo");
		}
		
		[Test]
		public void TestVar0()
		{
			RunCompilerTestCase("var0.boo", "var as string");
		}
		
		[Test]
		public void TestVar1()
		{
			RunCompilerTestCase("var1.boo", "var as string = expression");
		}
		
		[Test]
		public void TestDelegate0()
		{
			RunCompilerTestCase("delegate0.boo", "basic delegate support");
		}
		
		[Test]
		public void TestProperty0()
		{
			RunCompilerTestCase("property0.boo", "basic property support");
		}
		
		[Test]
		public void TestProperty1()
		{
			RunCompilerTestCase("property1.boo", "unpack property");
		}
		
		[Test]
		public void TestTuple0()
		{
			RunCompilerTestCase("tuple0.boo", "simple tuple creation");
		}
		
		[Test]
		public void TestTuple1()
		{
			RunCompilerTestCase("tuple1.boo", "tuple unpacking");
		}
		
		[Test]
		public void TestTuple3()
		{
			RunCompilerTestCase("tuple2.boo", "string.Format(template, tuple)");
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
				return console.ToString().Replace("\r\n", "\n");
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
		
		string GetBaseBooDirectory()
		{
			return Path.GetDirectoryName(
			         Path.GetDirectoryName(GetType().Assembly.Location)
			         );
		}
	}
}
