#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace BooCompiler.Tests
{
	using System;
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipelines;

	/// <summary>	
	/// </summary>
	[TestFixture]
	public class LocalizationTestCase
	{
		const string TestCase = "foo class";
		
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

#if !VISUAL_STUDIO
		[Test]
		public void TestPtBrCulture()
		{
			AssertCultureDependentMessage("Token inesperado: foo.", CultureInfo.CreateSpecificCulture("pt-BR"));
		}
#endif

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
	
				Assert.AreEqual(1, errors.Count);
				Assert.AreEqual(message, errors[0].Message);
			}
			finally
			{
				Thread.CurrentThread.CurrentUICulture = savedCulture;
			}
		}
	}
}
