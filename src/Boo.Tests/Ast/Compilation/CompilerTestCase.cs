using System;
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
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string booAssemblyPath = typeof(Compiler).Assembly.Location;
			File.Copy(booAssemblyPath, Path.Combine(Path.GetTempPath(), Path.GetFileName(booAssemblyPath)), true);
			
			_compiler = new Compiler();
			_parameters = _compiler.Parameters;
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
		
		string RunString(string code)
		{	
			return RunString(code, null);
		}
		
		string RunString(string code, string stdin)
		{
			_parameters.Input.Add(new StringInput("<teststring>", code));
			return Run(stdin);
		}
		
		string Run(string stdin)
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
				
				CompilerContext context = _compiler.Run();
				
				if (context.Errors.Count > 0)
				{
					Assert.Fail(context.Errors.ToString(true));
				}
				return console.ToString();
			}
			finally
			{
				_compiler.Parameters.Input.Clear();
				Console.SetOut(oldStdOut);
				if (null != stdin)
				{
					Console.SetIn(oldStdIn);
				}
			}
		}
	}
}
