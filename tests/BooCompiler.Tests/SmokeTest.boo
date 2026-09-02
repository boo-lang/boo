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

import NUnit.Framework

[TestFixture]
class SmokeTest(AbstractCompilerTestCase):
	protected override def CopyDependencies():
		super.CopyDependencies()
		CopyAssembly(typeof(Boo.Lang.PatternMatching.MatchMacro).Assembly)

	[Test]
	def ExternalModuleWithNoNamespace():
		RunCompilerTestCase("ExternalModuleWithNoNamespace.boo")

	[Test]
	def InternalMacroBootstrapping():
		RunCompilerTestCase("InternalMacroBootstrapping.boo")

	[Test]
	def InternalMacrosInSeparateModules():
		RunCompilerTestCase("InternalMacrosInSeparateModules.boo")

	[Test]
	def DuckTyping():
		RunCompilerTestCase("DuckTyping.boo")

	[Test]
	def InternalArrayType():
		RunCompilerTestCase("InternalArrayType.boo")

	[Test]
	def UndeclaredDefaultConstructor():
		RunCompilerTestCase("UndeclaredDefaultConstructor.boo")

	[Test]
	def InternalMacro():
		RunCompilerTestCase("InternalMacro.boo")

	[Test]
	def InternalClassGenerator():
		RunCompilerTestCase("InternalClassGenerator.boo")

	[Test]
	def CodeLiteralInNamespaceWithBooLangPrefix():
		code = """
namespace Boo.Lang.Useful.Attributes

print [| 42 |]"""
		Assert.AreEqual("42\n", RunString(code))

	[Test]
	def TestHello():
		Assert.AreEqual("Hello!\n", RunString("print('Hello!')"))

	[Test]
	def TestHello2():
		stdin = "Test2\n"
		code = "name = prompt(''); print(\"Hello, \${name}!\")"
		Assert.AreEqual("Hello, Test2!\n", RunString(code, stdin))

	[Test]
	def TypeReferenceRepresentsType():
		RunCompilerTestCase("typereference0.boo")

	[Test]
	def TestIfModifier0():
		RunCompilerTestCase("if0.boo")

	[Test]
	def TimeSpanLiteral():
		RunCompilerTestCase("timespan0.boo")

	[Test]
	def TestMatch0():
		RunCompilerTestCase("match0.boo")

	[Test]
	def TestMatch1():
		RunCompilerTestCase("match1.boo")

	[Test]
	def TestNot0():
		RunCompilerTestCase("not0.boo")

	[Test]
	def ArrayMember():
		RunCompilerTestCase("in0.boo")

	[Test]
	def ArrayNotMember():
		RunCompilerTestCase("in1.boo")

	[Test]
	def IsNotIs():
		RunCompilerTestCase("is0.boo")

	[Test]
	def RealType():
		RunCompilerTestCase("double0.boo")

	[Test]
	def PreIncrement():
		RunCompilerTestCase("preincrement0.boo")

	[Test]
	def PreDecrement():
		RunCompilerTestCase("predecrement0.boo")

	[Test]
	def SumLocals():
		RunCompilerTestCase("sum0.boo")

	[Test]
	def MultLocals():
		RunCompilerTestCase("mult0.boo")

	[Test]
	def InPlaceAddLocals():
		RunCompilerTestCase("inplaceadd0.boo")

	[Test]
	def InPlaceField():
		RunCompilerTestCase("inplace1.boo")

	[Test]
	def LongLiterals():
		RunCompilerTestCase("long0.boo")

	[Test]
	def BooleanFromBoxedValueTypes():
		RunCompilerTestCase("bool0.boo")

	[Test]
	def GreaterThanEqualForInts():
		RunCompilerTestCase("gte_int.boo")

	[Test]
	def LessThanEqualForInts():
		RunCompilerTestCase("lte_int.boo")

	[Test]
	def IndexedProperty():
		RunCompilerTestCase("indexed.boo")

	[Test]
	def IndexPropertyWithSyntacticAttribute():
		RunCompilerTestCase("indexed2.boo")

	[Test]
	def ImportInternalNamespace():
		RunMultiFileTestCase("multifile0.boo", "Character.boo")

	[Test]
	def ImportAutomaticallyFromModulesInTheSameNamespace():
		RunMultiFileTestCase("multifile1.boo", "Character.boo")

	[Test]
	def ImportFunctionsFromModulesInTheGlobalNamespace():
		RunMultiFileTestCase("multifile2.boo", "math.boo")

	[Test]
	def ImportFunctionsFromModulesInTheSameNamespace():
		RunMultiFileTestCase("multifile3.boo", "mathwithns.boo")

	[Test]
	def ClassesCanUseModuleMethods():
		RunCompilerTestCase("module_methods0.boo")

	[Test]
	def RangeBuiltin():
		RunCompilerTestCase("range.boo")

	[Test]
	def StringAddition():
		RunCompilerTestCase("stringadd0.boo")

	[Test]
	def StringMultiplyByInt():
		RunCompilerTestCase("stringmultiply0.boo")

	[Test]
	def ListRichOperators():
		RunCompilerTestCase("listoperators.boo")

	[Test]
	def CustomerAddresses():
		RunCompilerTestCase("customer_addresses.boo")

	[Test]
	def NumberExponentiation():
		RunCompilerTestCase("pow0.boo")

	[Test]
	def UnlessModifier():
		RunCompilerTestCase("unless.boo")

	[Test]
	def StringFormattingWithTripleQuotedString():
		RunCompilerTestCase("formatting0.boo")

	[Test]
	def UnpackLocals():
		RunCompilerTestCase("unpack_locals.boo")

	[Test]
	def StatementModifierOnUnpack():
		RunCompilerTestCase("modifiers0.boo")

	[Test]
	def StaticFieldSimple():
		RunCompilerTestCase("static_field0.boo")

	[Test]
	def StaticLiteralField():
		RunCompilerTestCase("static_literalfield0.boo")

	[Test]
	def StaticConstructorIsCalledBeforeFirstStaticFieldAccess():
		RunCompilerTestCase("static_constructor0.boo")

	[Test]
	def IncrementProperty():
		RunCompilerTestCase("increment_property0.boo")

	[Test]
	def IncrementPropertyAndUseValue():
		RunCompilerTestCase("increment_property1.boo")

	[Test]
	def EnumComparisons():
		RunCompilerTestCase("enum_comparisons.boo")

	[Test]
	def TypeIsCallable():
		RunCompilerTestCase("typeiscallable.boo")

	[Test]
	def TypeAsICallable():
		RunCompilerTestCase("typeiscallable1.boo")

	[Test]
	def OverloadedMethodsCanBeDeclaredInAnyOrder():
		RunCompilerTestCase("logservice.boo")

	[Test]
	def ParameterAsLValue():
		RunCompilerTestCase("parameter_as_lvalue.boo")

	[Test]
	def NullIsCompatibleWithInternalClasses():
		RunCompilerTestCase("null0.boo")

	[Test]
	def TextReaderIsEnumerable():
		RunCompilerTestCase("textreaderisenumerable.boo")

	[Test]
	def RegularExpressionLiteralIsRegex():
		RunCompilerTestCase("re0.boo")

	[Test]
	def RegularExpressionMatch():
		RunCompilerTestCase("re1.boo")

	[Test]
	def CaseInsensitiveHash():
		RunCompilerTestCase("caseinsensitivehash.boo")

	[Test]
	def EnumeratorItemTypeForClasses():
		RunCompilerTestCase("enumeratoritemtype0.boo")

	[Test]
	def EnumeratorItemTypeForInternalClasses():
		RunCompilerTestCase("enumeratoritemtype1.boo")

	[Test]
	def EnumeratorItemTypeForMethods():
		RunCompilerTestCase("enumeratoritemtype2.boo")

	[Test]
	def EnumeratorItemTypeForOverloadedGetEnumerator():
		RunCompilerTestCase("enumeratoritemtype3.boo")

	[Test]
	def UnaryMinusWithLocal():
		RunCompilerTestCase("unary0.boo")

	[Test]
	def RedefineBuiltin():
		RunCompilerTestCase("redefine_builtin.boo")

	[Test]
	def ExternalConstants():
		RunCompilerTestCase("const0.boo")

	[Test]
	def ListSort():
		RunCompilerTestCase("sort.boo")

	[Test]
	def CustomCollection():
		RunCompilerTestCase("CustomCollection.boo")

	[Test]
	def UsingNestedType():
		RunCompilerTestCase("UsingNestedType.boo")
