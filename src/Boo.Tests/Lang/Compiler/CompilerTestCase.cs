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

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipeline;
using NUnit.Framework;

namespace Boo.Tests.Lang.Compiler
{
	[Flags]
	public enum TestEnum
	{
		Foo = 1,
		Bar = 2,
		Baz = 4
	}
	
	public class Person
	{
		string _fname;
		string _lname;
		
		public Person()
		{			
		}
		
		public string FirstName
		{
			get
			{
				return _fname;
			}
			
			set
			{
				_fname = value;
			}
		}
		
		public string LastName
		{
			get
			{
				return _lname;
			}
			
			set
			{
				_lname = value;
			}
		}
	}
	
	public class PersonCollection : System.Collections.CollectionBase
	{
		public PersonCollection()
		{
		}
		
		public Person this[int index]
		{
			get
			{
				return (Person)InnerList[index];
			}
			
			set
			{
				InnerList[index] = value;
			}
		}
		
		public Person this[string fname]
		{
			get
			{
				foreach (Person p in InnerList)
				{
					if (p.FirstName == fname)
					{
						return p;
					}
				}
				return null;
			}
			
			set
			{
				int index = 0;
				foreach (Person p in InnerList)
				{
					if (p.FirstName == fname)
					{
						InnerList[index] = value;
						break;						
					}
					++index;
				}
			}
		}
		
		public void Add(Person person)
		{
			InnerList.Add(person);
		}
	}
	
	public class Clickable
	{
		public Clickable()
		{			
		}
		
		public event EventHandler Click;
		
		public void RaiseClick()
		{
			if (null != Click)
			{
				Click(this, EventArgs.Empty);
			}
		}
	}
	
	public class BaseClass
	{
		protected BaseClass()
		{			
		}
		
		protected BaseClass(string message)
		{
			Console.WriteLine("BaseClass.constructor('{0}')", message);
		}
		
		public virtual void Method0()
		{
			Console.WriteLine("BaseClass.Method0");
		}
		
		public virtual void Method0(string text)
		{
			Console.WriteLine("BaseClass.Method0('{0}')", text);
		}
		
		public virtual void Method1()
		{
			Console.WriteLine("BaseClass.Method1");
		}
	}
	
	public class DerivedClass : BaseClass
	{
		protected DerivedClass()
		{
		}
		
		public void Method2()
		{
			Method0();
			Method1();
		}
	}
	
	public class Disposable : System.IDisposable
	{
		public Disposable()
		{
			Console.WriteLine("Disposable.constructor");
		}
		
		public void foo()
		{
			Console.WriteLine("Disposable.foo");
		}
		
		void System.IDisposable.Dispose()
		{
			Console.WriteLine("Disposable.Dispose");
		}
	}
	
	[TestFixture]
	public class CompilerTestCase : AbstractCompilerTestCase
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
					Add(new EmitAssemblyStep()).
					Add(new SaveAssemblyStep()).
					Add(new PEVerifyStep()).
					Add(new RunAssemblyStep());
		}
		
		[Test]
		public void TestDefaultOutputType()
		{
			Assert.AreEqual(CompilerOutputType.ConsoleApplication, _parameters.OutputType,
					"Default compiler output type must be ConsoleApplication."); 
		}
		
		[Test]
		public void TestDefaultAssemblyReferences()
		{
			AssemblyCollection references = _parameters.References;
			Assert.AreEqual(3, references.Count);
			Assert.IsTrue(references.Contains(typeof(string).Assembly), "(ms)corlib.dll must be referenced by default!");
			Assert.IsTrue(references.Contains(Assembly.LoadWithPartialName("System")), "System.dll must be referenced by default!");
			Assert.IsTrue(references.Contains(typeof(Boo.Lang.Builtins).Assembly), "Boo.dll must referenced by default!");
		}
		
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
		public void AttributeConstructorSupport()
		{
			RunCompilerTestCase("attributes0.boo");
		}
		
		[Test]
		public void AttributeNamedArguments()
		{
			RunCompilerTestCase("attributes1.boo");
		}
		
		[Test]
		public void MethodAttributes()
		{
			RunCompilerTestCase("attributes2.boo");
		}
		
		[Test]
		public void TestImportSimpleNamespace()
		{
			RunCompilerTestCase("import0.boo");
		}		
			
		[Test]
		public void TestImportQualifiedType()
		{
			RunCompilerTestCase("import1.boo");
		}
		
		[Test]
		public void TestImportQualifiedNamespace()
		{
			RunCompilerTestCase("import2.boo");
		}
		
		[Test]
		public void TestImportAssemblyQualifiedNamespace()
		{
			RunCompilerTestCase("import3.boo");
		}
		
		[Test]
		public void TestImportAlias()
		{
			RunCompilerTestCase("import4.boo");
		}
		
		[Test]
		public void TestImportAssemblyQualifiedNamespace2()
		{
			RunCompilerTestCase("import5.boo");
		}
		
		[Test]
		public void TestImportSameAssemblyQualifiedNamespaces()
		{
			RunCompilerTestCase("import6.boo");
		}
		
		[Test]
		public void TestSimpleFor()
		{
			RunCompilerTestCase("for0.boo");
		}
		
		[Test]
		public void TestTypedFor()
		{
			RunCompilerTestCase("for1.boo");
		}
		
		[Test]
		public void TestUnpackFor()
		{
			RunCompilerTestCase("for2.boo");
		}
		
		[Test]
		public void TestPrivateForVariables()
		{
			RunCompilerTestCase("for3.boo");
		}
		
		[Test]
		public void TestIfModifier0()
		{
			RunCompilerTestCase("if0.boo");
		}
		
		[Test]
		public void TestList0()
		{
			RunCompilerTestCase("list0.boo");
		}
		
		[Test]
		public void TestList1()
		{
			RunCompilerTestCase("list1.boo");
		}
		
		[Test]
		public void EnumUsage()
		{
			RunCompilerTestCase("enum0.boo");
		}
		
		[Test]
		public void EnumDefinition()
		{
			RunCompilerTestCase("enum1.boo");
		}
		
		[Test]
		public void TestVar0()
		{
			RunCompilerTestCase("var0.boo");
		}
		
		[Test]
		public void TestVar1()
		{
			RunCompilerTestCase("var1.boo");
		}
		
		[Test]
		public void DelegateAddedWithConstructor()
		{
			RunCompilerTestCase("delegate0.boo");
		}
		
		[Test]
		public void DelegateAddedWithInPlaceAdd()
		{
			RunCompilerTestCase("delegate1.boo");
		}
		
		[Test]
		public void DelegateWithInstanceMember()
		{
			RunCompilerTestCase("delegate2.boo");
		}
		
		[Test]
		public void TestProperty0()
		{
			RunCompilerTestCase("property0.boo");
		}
		
		[Test]
		public void TestProperty1()
		{
			RunCompilerTestCase("property1.boo");
		}
		
		[Test]
		public void TimeSpanLiteral()
		{
			RunCompilerTestCase("timespan0.boo");
		}
		
		[Test]
		public void TupleLiteral()
		{
			RunCompilerTestCase("tuple0.boo");
		}
		
		[Test]
		public void TupleUnpacking()
		{
			RunCompilerTestCase("tuple1.boo");
		}
		
		[Test]
		public void TupleAsArgument()
		{
			RunCompilerTestCase("tuple2.boo");
		}
		
		[Test]
		public void TupleTypeReferences()
		{
			RunCompilerTestCase("tuple3.boo");
		}
		
		[Test]
		public void TupleTypeInference()
		{
			RunCompilerTestCase("tuple4.boo");
		}
		
		[Test]
		public void TupleBuiltin()
		{
			RunCompilerTestCase("tuple5.boo");
		}
		
		[Test]
		public void TupleForInternalType()
		{
			RunCompilerTestCase("tuple6.boo");
		}
		
		[Test]
		public void TupleZip()
		{
			RunCompilerTestCase("tuple7.boo");
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
		public void TestTry0()
		{
			RunCompilerTestCase("try0.boo");
		}
		
		[Test]
		public void TestTry1()
		{
			RunCompilerTestCase("try1.boo");
		}
		
		[Test]
		public void TestTry2()
		{
			RunCompilerTestCase("try2.boo");
		}
		
		[Test]
		public void TestTry3()
		{
			RunCompilerTestCase("try3.boo");
		}
		
		[Test]
		public void SimpleInternalClass()
		{
			RunCompilerTestCase("class0.boo");
		}
		
		[Test]
		public void InternalClassWithField()
		{
			RunCompilerTestCase("class1.boo");
		}
		
		[Test]
		public void InternalClassWithProperty()
		{
			RunCompilerTestCase("class2.boo");
		}
		
		[Test]                          
		public void InternalClassWithFieldAndPropertyCreateByAstAttribute()
		{
			RunCompilerTestCase("class3.boo");
		}
		
		[Test]
		public void ToStringOverload()
		{
			RunCompilerTestCase("class4.boo");
		}
		
		[Test]
		public void ForwardInstanceMethodReference()
		{
			RunCompilerTestCase("class5.boo");
		}
		
		[Test]
		public void ForwardDelegateMethodReference()
		{
			RunCompilerTestCase("class6.boo");
		}
		
		[Test]
		public void ClassWithoutConstructor()
		{
			RunCompilerTestCase("class7.boo");
		}
		
		[Test]
		public void ForwardClassReference()
		{
			RunCompilerTestCase("class8.boo");
		}
		
		[Test]
		public void ForwardFieldReference()
		{
			RunCompilerTestCase("class9.boo");
		}
		
		[Test]
		public void ForwardPropertyReference()
		{
			RunCompilerTestCase("class10.boo");
		}
		
		[Test]
		public void TestSimpleCast()
		{
			RunCompilerTestCase("cast0.boo");
		}
		
		[Test]
		public void TestTypeTest()
		{
			RunCompilerTestCase("cast1.boo");
		}
		
		[Test]
		public void TestSimpleBaseClass()
		{
			RunCompilerTestCase("baseclass0.boo");
		}
		
		[Test]
		public void TestOverrideBaseClassMethod()
		{
			RunCompilerTestCase("baseclass1.boo");
		}
		
		[Test]
		public void CallSuperMethod()
		{
			RunCompilerTestCase("baseclass2.boo");
		}
		
		[Test]
		public void CallSuperConstructor()
		{
			RunCompilerTestCase("baseclass3.boo");
		}
		
		[Test]
		public void OverrideMethodHigherInTheHierarchy()
		{
			RunCompilerTestCase("baseclass4.boo");
		}
		
		[Test]
		public void TestMethod0()
		{
			RunCompilerTestCase("method0.boo");
		}
		
		[Test]
		public void TestMethod1()
		{
			RunCompilerTestCase("method1.boo");
		}
		
		[Test]
		public void TestMethod3()
		{
			RunCompilerTestCase("method3.boo");
		}
		
		[Test]
		public void TestMethod4()
		{
			RunCompilerTestCase("method4.boo");
		}
		
		[Test]
		public void TestMethod5()
		{
			RunCompilerTestCase("method5.boo");
		}
		
		[Test]
		public void ConditionalReturnBranches()
		{
			RunCompilerTestCase("method7.boo");
		}
		
		[Test]
		public void ExceptionalReturnBranches()
		{
			RunCompilerTestCase("method8.boo");
		}
		
		[Test]
		public void TupleMember()
		{
			RunCompilerTestCase("in0.boo");
		}
		
		[Test]
		public void TupleNotMember()
		{
			RunCompilerTestCase("in1.boo");
		}
		
		[Test]
		public void Typeof()
		{
			RunCompilerTestCase("typeof0.boo");
		}
		
		[Test]
		public void ExplicitSignatureOverride()
		{
			RunCompilerTestCase("override1.boo");
		}
		
		[Test]
		public void InferedSignatureOverride()
		{
			RunCompilerTestCase("override2.boo");
		}
		
		[Test]
		public void Using()
		{
			RunCompilerTestCase("using0.boo");
		}
		
		[Test]
		public void IsNotIs()
		{
			RunCompilerTestCase("is0.boo");
		}
		
		[Test]
		public void IsNotIsType()
		{
			RunCompilerTestCase("isa0.boo");
		}
		
		[Test]
		public void IsBranch()
		{
			RunCompilerTestCase("isa1.boo");
		}
		
		[Test]
		public void SliceArraySimple()
		{
			RunCompilerTestCase("slicing0.boo");
		}
		
		[Test]
		public void SliceArrayArray()
		{
			RunCompilerTestCase("slice_array.boo");
		}
		
		[Test]
		public void SliceIndexedArrayProperty()
		{
			RunCompilerTestCase("slice_indexed_array_property.boo");
		}
		
		[Test]
		public void SliceListSimple()
		{
			RunCompilerTestCase("slicing1.boo");
		}
		
		[Test]
		public void SliceArrayNegative()
		{
			RunCompilerTestCase("slicing2.boo");
		}
		
		[Test]
		public void SliceArrayNegativeOrPositive()
		{
			RunCompilerTestCase("slicing3.boo");
		}
		
		[Test]
		public void SliceListNegative()
		{
			RunCompilerTestCase("slicing4.boo");
		}
		
		[Test]
		public void SliceOverload()
		{
			RunCompilerTestCase("slicing5.boo");
		}
		
		[Test]
		public void SliceOverloadAssignment()
		{
			RunCompilerTestCase("slicing6.boo");
		}
		
		[Test]
		public void SliceOverTuple()
		{
			RunCompilerTestCase("slicing7.boo");
		}
		
		[Test]
		public void SliceOverIndexedProperty()
		{
			RunCompilerTestCase("slicing8.boo");
		}
		
		[Test]
		public void SliceArrayComplex()
		{
			RunCompilerTestCase("slicing9.boo");
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
		public void Or()
		{
			RunCompilerTestCase("or0.boo");
		}
		
		[Test]
		public void OrShortCircuit()
		{
			RunCompilerTestCase("or1.boo");
		}
		
		[Test]
		public void OrWithObjectReferences()
		{
			RunCompilerTestCase("or2.boo");
		}
		
		[Test]
		public void And()
		{
			RunCompilerTestCase("and0.boo");
		}
		
		[Test]
		public void AndBranches()
		{
			RunCompilerTestCase("and1.boo");
		}
		
		[Test]
		public void BooleanFromBoxedValueTypes()
		{
			RunCompilerTestCase("bool0.boo");
		}
		
		[Test]
		public void BitwiseOrForEnums()
		{
			RunCompilerTestCase("bitwise_enum.boo");
		}
		
		[Test]
		public void BitwiseOrForInt()
		{
			RunCompilerTestCase("bitwise_int.boo");			
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
		public void HashLiteral()
		{
			RunCompilerTestCase("hash0.boo");
		}
		
		[Test]
		public void IndexedProperty()
		{
			RunCompilerTestCase("indexed.boo");
		}
		
		[Test]
		public void ImportInternalNamespace()
		{
			RunMultiFileTestCase("multifile0.boo", "character.boo");
		}
		
		[Test]
		public void ImportAutomaticallyFromModulesInTheSameNamespace()
		{
			RunMultiFileTestCase("multifile1.boo", "character.boo");
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
		public void SimpleWhile()
		{
			RunCompilerTestCase("while0.boo");
		}

		[Test]
		public void WhileFalse()
		{
			RunCompilerTestCase("while1.boo");
		}
		
		[Test]
		public void InterfaceContainsObjectMethods()
		{
			RunCompilerTestCase("interface0.boo");
		}
		
		[Test]
		public void ClassImplementsInterface()
		{
			RunCompilerTestCase("interface1.boo");
		}
		
		[Test]
		public void ArrayEquality()
		{
			RunCompilerTestCase("array_equality.boo");
		}
		
		[Test]
		public void UnlessModifier()
		{
			RunCompilerTestCase("unless.boo");
		}
		
		[Test]
		public void BreakForWhile()
		{
			RunCompilerTestCase("break0.boo");
		}
		
		[Test]
		public void BreakOutOfTry()
		{
			RunCompilerTestCase("break1.boo");
		}
		
		[Test]
		public void ContinueFor()
		{
			RunCompilerTestCase("continue0.boo");
		}
		
		[Test]
		public void ContinueWhile()
		{
			RunCompilerTestCase("continue1.boo");
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
		public void ICallableCanBeInvoked()
		{
			RunCompilerTestCase("callable0.boo");
		}
		
		[Test]
		public void MethodReferenceCanBeAssignedToLocalAndLaterInvoked()
		{
			RunCompilerTestCase("callable1.boo");
		}
		
		[Test]
		public void MethodReferenceCanBeSelected()
		{
			RunCompilerTestCase("callable2.boo");
		}
		
		[Test]
		public void StaticFieldSimple()
		{
			RunCompilerTestCase("static_field0.boo");
		}
		
		[Test]
		public void StaticConstructorIsCalledBeforeFirstStaticFieldAccess()
		{
			RunCompilerTestCase("static_constructor0.boo");
		}
		
		[Test]
		public void InnerClassSimple()
		{
			RunCompilerTestCase("innerclass0.boo");
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
		public void Overloading()
		{
			RunCompilerTestCase("overloading0.boo");
		}
		
		[Test]
		public void TypeIsCallable()
		{
			RunCompilerTestCase("typeiscallable.boo");
		}
	}
}
