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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Stepss;

namespace Boo.Lang.Compiler.Tests
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
			CultureInfo savedCulture = Thread.CurrentThread.CurrentUICulture;			
			Thread.CurrentThread.CurrentUICulture = culture;

			try
			{
				BooCompiler compiler = new BooCompiler();
				CompilerParameters options = compiler.Parameters;
				options.Input.Add(new Boo.Lang.Compiler.IO.StringInput("testcase", TestCase));
				options.Pipeline.Load(typeof(Parse));
				
				CompilerErrorCollection errors = compiler.Run().Errors;
	
				Assertion.AssertEquals(1, errors.Count);
				Assertion.AssertEquals(message, errors[0].Message);
			}
			finally
			{
				Thread.CurrentThread.CurrentUICulture = savedCulture;
			}
		}
	}
}
