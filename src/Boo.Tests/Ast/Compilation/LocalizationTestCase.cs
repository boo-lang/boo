using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Boo.Ast.Compilation;

namespace Boo.Tests.Ast.Compilation
{
	/// <summary>	
	/// </summary>
	[TestFixture]
	public class LocalizationTestCase
	{
		const string TestCase = "foo def";
		
		[Test]
		public void TestNeutralCulture()
		{
			AssertCultureDependentMessage("Unexpected token: foo.", CultureInfo.InvariantCulture);
		}

		[Test]
		public void TestEnUsCulture()
		{
			AssertCultureDependentMessage("Unexpected token: foo.", CultureInfo.CreateSpecificCulture("en-US"));
		}

		[Test]
		public void TestPtBrCulture()
		{
			AssertCultureDependentMessage("Token inesperado: foo.", CultureInfo.CreateSpecificCulture("pt-BR"));
		}

		void AssertCultureDependentMessage(string message, CultureInfo culture)
		{
			Thread.CurrentThread.CurrentUICulture = culture;

			Compiler compiler = new Compiler();
			CompilerParameters options = compiler.Parameters;
			options.Input.Add(new Boo.Ast.Compilation.IO.StringInput("testcase", TestCase));
			options.Pipeline.Add(new Boo.Ast.Compilation.Steps.BooParsingStep());
			
			ErrorCollection errors = compiler.Run().Errors;

			Assertion.AssertEquals(1, errors.Count);
			Assertion.AssertEquals(message, errors[0].Message);
		}
	}
}
