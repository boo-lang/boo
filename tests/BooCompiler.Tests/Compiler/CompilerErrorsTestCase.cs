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
				Console.Write(Path.GetFileName(error.LexicalInfo.FileName));
				Console.Write("({0},{1}): ", error.LexicalInfo.Line, error.LexicalInfo.StartColumn);
				Console.Write("{0}: ", error.Code);
				Console.WriteLine(error.Message);
			}
		}
	}
	
	[TestFixture]
	public class CompilerErrorsTestCase : AbstractCompilerTestCase
	{		
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
