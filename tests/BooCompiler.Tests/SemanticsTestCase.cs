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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipelines;
	using NUnit.Framework;
	
	[TestFixture]
	public class SemanticsTestCase : AbstractCompilerTestCase
	{
		protected override CompilerPipeline SetUpCompilerPipeline()
		{
			return new CompileToBoo();
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
		public void FieldInitializationOrder()
		{
			RunCompilerTestCase("field2.boo");
		}
		
		[Test]
		public void StaticFieldInitializerAndNonStaticFieldInitializer()
		{
			RunCompilerTestCase("field3.boo");
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
		public void LockMacroWithMultipleArguments()
		{
			RunCompilerTestCase("lock1.boo");
		}
		
		[Test]
		public void LockAttributeInMethods()
		{
			RunCompilerTestCase("lock2.boo");
		}
		
		[Test]
		public void ReturnNull()
		{
			RunCompilerTestCase("null0.boo");			
		}
		
		[Test]
		public void ReturnNullOnlyIsTypedObject()
		{
			RunCompilerTestCase("null1.boo");
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
		public void ArrayFunctionTypeInference()
		{
			RunCompilerTestCase("array_function.boo");
		}
		
		[Test]
		public void NumericPromotionInReturnTypes()
		{
			RunCompilerTestCase("numericpromo0.boo");
		}
		
		[Test]
		public void TypeIsResolvedAgainstParentScopeWhenNotFound()
		{
			RunCompilerTestCase("type_resolution0.boo");
		}
		
		[Test]
		public void InterfaceMethodsTurnIntoAbstractMethodsWhenNotImplemented()
		{
			RunCompilerTestCase("interface0.boo");
		}
		
		[Test]
		public void SliceString()
		{
			RunCompilerTestCase("stringslice0.boo");
		}
		
		[Test]
		public void SliceSliceString()
		{
			RunCompilerTestCase("stringslice1.boo");
		}
		
		[Test]
		public void StaticFieldInitializer()
		{
			RunCompilerTestCase("static_field_initializer.boo");
		}
		
		[Test]
		public void AbstractMethodMakesClassAbstract()
		{
			RunCompilerTestCase("abstract_method0.boo");
		}
	}
}
