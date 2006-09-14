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
	using NUnit.Framework;
	
	[TestFixture]
	public class CompilationTestFixture : AbstractCompilerTestCase
	{				
		[Test]
		public void TestHello()
		{
			Assert.AreEqual("Hello!\n", RunString("print('Hello!')"));
		}
		
		[Test]
		public void TestHello2()
		{
			string stdin = "Test2\n";
			string code = "name = prompt(''); print(\"Hello, ${name}!\")";
			Assert.AreEqual("Hello, Test2!\n", RunString(code, stdin));			
		}
		
		[Test]
		public void TypeReferenceRepresentsType()
		{
			RunCompilerTestCase("typereference0.boo");
		}		
		
		[Test]
		public void TestIfModifier0()
		{
			RunCompilerTestCase("if0.boo");
		}
		
		[Test]
		public void TimeSpanLiteral()
		{
			RunCompilerTestCase("timespan0.boo");
		}		

		[Test]
		public void TestMatch0()
		{
			RunCompilerTestCase("match0.boo");
		}
		
		[Test]
		public void TestMatch1()
		{
			RunCompilerTestCase("match1.boo");
		}
		
		[Test]
		public void TestNot0()
		{
			RunCompilerTestCase("not0.boo");
		}		
		
		[Test]
		public void ArrayMember()
		{
			RunCompilerTestCase("in0.boo");
		}
		
		[Test]
		public void ArrayNotMember()
		{
			RunCompilerTestCase("in1.boo");
		}		
		
		[Test]
		public void IsNotIs()
		{
			RunCompilerTestCase("is0.boo");
		}		
		
		[Test]
		public void RealType()
		{
			RunCompilerTestCase("double0.boo");
		}
		
		[Test]
		public void PreIncrement()
		{
			RunCompilerTestCase("preincrement0.boo");
		}
		
		[Test]
		public void PreDecrement()
		{
			RunCompilerTestCase("predecrement0.boo");
		}
		
		[Test]
		public void SumLocals()
		{
			RunCompilerTestCase("sum0.boo");			
		}
		
		[Test]
		public void MultLocals()
		{
			RunCompilerTestCase("mult0.boo");
		}
		
		[Test]
		public void InPlaceAddLocals()
		{
			RunCompilerTestCase("inplaceadd0.boo");
		}
		
		[Test]
		public void InPlaceField()
		{
			RunCompilerTestCase("inplace1.boo");
		}
		
		[Test]
		public void LongLiterals()
		{
			RunCompilerTestCase("long0.boo");
		}
		
		[Test]
		public void BooleanFromBoxedValueTypes()
		{
			RunCompilerTestCase("bool0.boo");
		}
		
		[Test]
		public void GreaterThanEqualForInts()
		{
			RunCompilerTestCase("gte_int.boo");
		}
		
		[Test]
		public void LessThanEqualForInts()
		{
			RunCompilerTestCase("lte_int.boo");
		}		
		
		[Test]
		public void IndexedProperty()
		{
			RunCompilerTestCase("indexed.boo");
		}
		
		[Test]
		public void IndexPropertyWithSyntacticAttribute()
		{
			RunCompilerTestCase("indexed2.boo");
		}
		
		[Test]
		public void ImportInternalNamespace()
		{
			RunMultiFileTestCase("multifile0.boo", "Character.boo");
		}
		
		[Test]
		public void ImportAutomaticallyFromModulesInTheSameNamespace()
		{
			RunMultiFileTestCase("multifile1.boo", "Character.boo");
		}
		
		[Test]
		public void ImportFunctionsFromModulesInTheGlobalNamespace()
		{
			RunMultiFileTestCase("multifile2.boo", "math.boo");
		}
		
		[Test]
		public void ImportFunctionsFromModulesInTheSameNamespace()
		{
			RunMultiFileTestCase("multifile3.boo", "mathwithns.boo");
		}
		
		[Test]
		public void ClassesCanUseModuleMethods()
		{
			RunCompilerTestCase("module_methods0.boo");
		}
		
		[Test]
		public void RangeBuiltin()
		{
			RunCompilerTestCase("range.boo");
		}
		
		[Test]
		public void StringAddition()
		{
			RunCompilerTestCase("stringadd0.boo");
		}
		
		[Test]
		public void StringMultiplyByInt()
		{
			RunCompilerTestCase("stringmultiply0.boo");
		}
		
		[Test]
		public void ListRichOperators()
		{
			RunCompilerTestCase("listoperators.boo");
		}
		
		[Test]
		public void CustomerAddresses()
		{
			RunCompilerTestCase("customer_addresses.boo");
		}
		
		[Test]
		public void NumberExponentiation()
		{
			RunCompilerTestCase("pow0.boo");
		}
		
		[Test]
		public void UnlessModifier()
		{
			RunCompilerTestCase("unless.boo");
		}
		
		[Test]
		public void StringFormattingWithTripleQuotedString()
		{
			RunCompilerTestCase("formatting0.boo");
		}
		
		[Test]
		public void UnpackLocals()
		{
			RunCompilerTestCase("unpack_locals.boo");
		}
		
		[Test]
		public void StatementModifierOnUnpack()
		{
			RunCompilerTestCase("modifiers0.boo");
		}		
		
		[Test]
		public void StaticFieldSimple()
		{
			RunCompilerTestCase("static_field0.boo");
		}
		
		[Test]
		public void StaticLiteralField()
		{
			RunCompilerTestCase("static_literalfield0.boo");
		}
		
		[Test]
		public void StaticConstructorIsCalledBeforeFirstStaticFieldAccess()
		{
			RunCompilerTestCase("static_constructor0.boo");
		}
		
		[Test]
		public void IncrementProperty()
		{
			RunCompilerTestCase("increment_property0.boo");
		}
		
		[Test]
		public void IncrementPropertyAndUseValue()
		{
			RunCompilerTestCase("increment_property1.boo");
		}
		
		[Test]
		public void EnumComparisons()
		{
			RunCompilerTestCase("enum_comparisons.boo");
		}
		
		[Test]
		public void TypeIsCallable()
		{
			RunCompilerTestCase("typeiscallable.boo");
		}
		
		[Test]
		public void TypeAsICallable()
		{
			RunCompilerTestCase("typeiscallable1.boo");
		}
		
		[Test]
		public void OverloadedMethodsCanBeDeclaredInAnyOrder()
		{
			RunCompilerTestCase("logservice.boo");
		}
		
		[Test]
		public void ParameterAsLValue()
		{
			RunCompilerTestCase("parameter_as_lvalue.boo");
		}
		
		[Test]
		public void NullIsCompatibleWithInternalClasses()
		{
			RunCompilerTestCase("null0.boo");
		}
		
		[Test]
		public void TextReaderIsEnumerable()
		{
			RunCompilerTestCase("textreaderisenumerable.boo");
		}
		
		[Test]
		public void RegularExpressionLiteralIsRegex()
		{
			RunCompilerTestCase("re0.boo");
		}
		
		[Test]
		public void RegularExpressionMatch()
		{
			RunCompilerTestCase("re1.boo");
		}
		
		[Test]
		public void CaseInsensitiveHash()
		{
			RunCompilerTestCase("caseinsensitivehash.boo");
		}
		
		[Test]
		public void EnumeratorItemTypeForClasses()
		{
			RunCompilerTestCase("enumeratoritemtype0.boo");
		}
		
		[Test]
		public void EnumeratorItemTypeForInternalClasses()
		{
			RunCompilerTestCase("enumeratoritemtype1.boo");
		}
		
		[Test]
		public void EnumeratorItemTypeForMethods()
		{
			RunCompilerTestCase("enumeratoritemtype2.boo");
		}
		
		[Test]
		public void UnaryMinusWithLocal()
		{
			RunCompilerTestCase("unary0.boo");
		}
		
		[Test]
		public void RedefineBuiltin()
		{
			RunCompilerTestCase("redefine_builtin.boo");
		}
		
		[Test]
		public void ExternalConstants()
		{
			RunCompilerTestCase("const0.boo");
		}
		
		[Test]
		public void ListSort()
		{
			RunCompilerTestCase("sort.boo");
		}
		
		[Test]
		public void CustomCollection()
		{
			RunCompilerTestCase("CustomCollection.boo");
		}
		
		[Test]
		public void UsingNestedType()
		{
			RunCompilerTestCase("UsingNestedType.boo");
		}
	}
}
