#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Tests.Lang.Compiler
{
	using System;
	using System.IO;
	using Boo.Lang.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipeline;
	using Boo.Antlr;
	using NUnit.Framework;
	
	[TestFixture]
	public class SemanticsTestCase : AbstractCompilerTestCase
	{
		protected override void SetUpCompilerPipeline(CompilerPipeline pipeline)
		{
			pipeline.
					Add(new Boo.Antlr.BooParsingStep()).
					Add(new ImportResolutionStep()).
					Add(new AstAttributesStep()).
					Add(new MacroExpansionStep()).
					Add(new AstNormalizationStep()).							
					Add(new SemanticStep()).
					Add(new BooPrinterStep());
		}
		
		protected override string GetTestCasePath(string name)
		{
			return Path.Combine(Path.Combine(_baseTestCasesPath, "../semantics"), name);
		}
		
		[Test]
		public void EqualityOperator()
		{
			RunCompilerTestCase("equality0.boo");
		}
		
		[Test]
		public void EnumSemantics()
		{
			RunCompilerTestCase("enum0.boo");
		}
		
		[Test]
		public void HashInOperator()
		{
			RunCompilerTestCase("hash0.boo");
		}
		
		[Test]
		public void ModuleMustBecomePrivateFinalClassWithPrivateConstructor()
		{
			RunCompilerTestCase("module0.boo");
		}
		
		[Test]
		public void ClassesMustBePublicByDefault()
		{
			RunCompilerTestCase("classes0.boo");
		}
		
		[Test]
		public void ClassesShouldHaveAutoConstructor()
		{
			RunCompilerTestCase("classes1.boo");
		}
		
		[Test]
		public void SimpleClassField()
		{
			RunCompilerTestCase("field0.boo");
		}
		
		[Test]
		public void SimpleClassFieldWithInitializer()
		{
			RunCompilerTestCase("field1.boo");
		}
		
		[Test]
		public void TestFatorialMethod()
		{
			RunCompilerTestCase("method2.boo");
		} 
		
		[Test]
		public void InvertedFatorial()
		{
			RunCompilerTestCase("method3.boo");
		}
		
		[Test]
		public void MutuallyRecursiveMethods()
		{
			RunCompilerTestCase("method6.boo");
		}
		
		[Test]
		public void MutuallyRecursiveMethodsAndMore()
		{
			RunCompilerTestCase("method7.boo");
		}
		
		[Test]
		public void UsingMacroWithBinaryExpressionArgument()
		{
			RunCompilerTestCase("using0.boo");
		}
		
		[Test]
		public void UsingMacroNested()
		{
			RunCompilerTestCase("using1.boo");
		}
		
		[Test]
		public void LockMacro()
		{
			RunCompilerTestCase("lock0.boo");
		}
		
		[Test]
		public void ReturnNull()
		{
			RunCompilerTestCase("null0.boo");			
		}
		
		[Test]
		public void InString()
		{
			RunCompilerTestCase("in_string.boo");
		}
		
		[Test]
		public void SliceProperty()
		{
			RunCompilerTestCase("slice_property.boo");
		}
		
		[Test]
		public void SlicePropertyInt()
		{
			RunCompilerTestCase("slice_property_int.boo");
		}
		
		[Test]
		public void AssignProperty()
		{
			RunCompilerTestCase("assign_property.boo");
		}
		
		[Test]
		public void Len()
		{
			RunCompilerTestCase("len.boo");
		}
		
		[Test]
		public void TupleFunctionTypeInference()
		{
			RunCompilerTestCase("tuple_function.boo");
		}
		
		[Test]
		public void NumericPromotionInReturnTypes()
		{
			RunCompilerTestCase("numericpromo0.boo");
		}
	}
}
