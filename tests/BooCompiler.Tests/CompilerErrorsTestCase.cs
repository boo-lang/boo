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
﻿
namespace BooCompiler.Tests
{
	using System;
	using System.IO;
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Steps;
	using Boo.Lang.Compiler.Pipelines;
	
	public class ErrorPrinterStep : AbstractCompilerComponent, ICompilerStep
	{
		public void Run()
		{
			foreach (CompilerError error in Errors)
			{
				OutputWriter.Write(Path.GetFileName(error.LexicalInfo.FileName));
				OutputWriter.Write("({0},{1}): ", error.LexicalInfo.Line, error.LexicalInfo.StartColumn);
				OutputWriter.Write("{0}: ", error.Code);
				OutputWriter.WriteLine(error.Message);
			}
		}
	}
	
	[TestFixture]
	public class CompilerErrorsTestCase : AbstractCompilerTestCase
	{	
		[Test]
		public void BCE0023()
		{
			RunCompilerTestCase("bce0023.boo");
		}
		
		[Test]
		public void BCE0080()
		{
			RunCompilerTestCase("bce0080.boo");
		}
		
		[Test]
		public void BCE0017()
		{
			RunCompilerTestCase("bce0017.boo");
		}
		
		[Test]
		public void BCE0053()
		{
			RunCompilerTestCase("bce0053.boo");
		}
		
		[Test]
		public void ReadOnlyProperty()
		{
			RunCompilerTestCase("readonly_property.boo");
		}
		
		[Test]
		public void InstanceFieldCantBeUsedWithoutInstance()
		{
			RunCompilerTestCase("instancefield0.boo");
		}
		
		[Test]
		public void TestReturnTypes()
		{
			RunCompilerTestCase("return0.boo");
		}
		
		[Test]
		public void IsNotIs()
		{
			RunCompilerTestCase("is0.boo");
		}
		
		[Test]
		public void IsaArgumentMustBeType()
		{
			RunCompilerTestCase("isa0.boo");
		}
		
		[Test]
		public void OverrideNonVirtualMethod()
		{
			RunCompilerTestCase("override0.boo");
		}
		
		[Test]
		public void OverrideNonExistingMethod()
		{
			RunCompilerTestCase("override1.boo");
		}
		
		[Test]
		public void OverrideWithDifferentReturnType()
		{
			RunCompilerTestCase("override2.boo");
		}
		
		[Test]
		public void Assignment()
		{
			RunCompilerTestCase("assign0.boo");
		}
		
		[Test]
		public void PrimitiveNamesCantBeRedefined()
		{
			RunCompilerTestCase("primitives0.boo");
		}
		
		[Test]
		public void PropertyTypeChecking()
		{
			RunCompilerTestCase("property0.boo");
		}
		
		[Test]
		public void BreakWithoutLoop()
		{
			RunCompilerTestCase("break0.boo");
		}
		
		[Test]
		public void AsWithValueTypes()
		{
			RunCompilerTestCase("as0.boo");
		}
		
		[Test]
		public void ForIterator()
		{
			RunCompilerTestCase("for0.boo");
		}
		
		[Test]
		public void ContinueWithoutLoop()
		{
			RunCompilerTestCase("continue0.boo");
		}
		
		[Test]
		public void StaticLexicalScope()
		{
			RunCompilerTestCase("nameresolution0.boo");
		}
		
		[Test]
		public void DeclarationAlreadyExists()
		{
			RunCompilerTestCase("declaration0.boo");
		}
		
		[Test]
		public void DeclarationInUnpackAlreadyExists()
		{
			RunCompilerTestCase("declaration1.boo");
		}
		
		[Test]
		public void DeclarationConflictsWithParameter()
		{
			RunCompilerTestCase("declaration2.boo");
		}
		
		[Test]
		public void DeclarationTypeError()
		{
			RunCompilerTestCase("declaration3.boo");
		}		
		
		[Test]
		public void RecursiveMethodsMustDeclareTheirReturnType()
		{
			RunCompilerTestCase("recursive0.boo");
		}
		
		[Test]
		public void NoCompatibleConstructor()
		{
			RunCompilerTestCase("constructor0.boo");
		}
		
		[Test]
		public void BaseClassCycle()
		{
			RunCompilerTestCase("basetypes0.boo");
		}
		
		[Test]
		public void BaseInterfaceCycle()
		{
			RunCompilerTestCase("basetypes1.boo");
		}
		
		[Test]
		public void AbstractMethodCannotHaveBody()
		{
			RunCompilerTestCase("abstract0.boo");
		}
		
		[Test]
		public void EventParameterListCompatibility()
		{
			RunCompilerTestCase("event0.boo");
		}		
		
		[Test]
		public void AddressOfCanOnlyBeUsedInDelegateConstructors()
		{
			RunCompilerTestCase("addressof.boo");
		}
		
		protected override CompilerPipeline SetUpCompilerPipeline()
		{
			CompilerPipeline pipeline = new Boo.Lang.Compiler.Pipelines.Compile();
			pipeline.Add(new ErrorPrinterStep());
			return pipeline;
		}
		
		protected override bool IgnoreErrors
		{
			get
			{
				return true;
			}
		}
		
		protected override string GetTestCasePath(string name)
		{
			return Path.Combine(Path.Combine(_baseTestCasesPath, "../errors"), name);
		}		
	}
}
