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
