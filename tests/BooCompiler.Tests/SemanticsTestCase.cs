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
		public void AssertMacroWithMessage()
		{
			RunCompilerTestCase("assert0.boo");
		}
		
		[Test]
		public void AssertMacroWithoutMessage()
		{
			RunCompilerTestCase("assert1.boo");
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
