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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Pipelines;

namespace BooCompiler.Tests
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
				Boo.Lang.Compiler.BooCompiler compiler = new Boo.Lang.Compiler.BooCompiler();
				CompilerParameters options = compiler.Parameters;
				options.Input.Add(new Boo.Lang.Compiler.IO.StringInput("testcase", TestCase));
				options.Pipeline = new Boo.Lang.Compiler.Pipelines.Parse();
				
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
