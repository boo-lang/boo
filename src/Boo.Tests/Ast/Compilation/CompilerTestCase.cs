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

namespace Boo.Tests.Ast.Compiler
{
	public enum TestEnum
	{
		Foo = 5,
		Bar = 10,
		Baz = 11
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
		
		public virtual void Method0()
		{
			Console.WriteLine("BaseClass.Method0");
		}
		
		public virtual void Method1()
		{
			Console.WriteLine("BaseClass.Method1");
		}
	}
	
	[TestFixture]
	public class CompilerTestCase : AbstractCompilerTestCase
	{
		protected override void SetUpCompilerPipeline(CompilerPipeline pipeline)
		{
			pipeline.
					Add(new Boo.Antlr.BooParsingStep()).
					Add(new UsingResolutionStep()).
					Add(new AstAttributesStep()).
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
		public void TestAttributeConstructorSupport()
		{
			RunCompilerTestCase("attributes0.boo");
		}
		
		[Test]
		public void TestAttributeNamedArguments()
		{
			RunCompilerTestCase("attributes1.boo");
		}
		
		[Test]
		public void TestUsingSimpleNamespace()
		{
			RunCompilerTestCase("using0.boo", "using System");
		}		
			
		[Test]
		public void TestUsingQualifiedType()
		{
			RunCompilerTestCase("using1.boo", "using System.Console");
		}
		
		[Test]
		public void TestUsingQualifiedNamespace()
		{
			RunCompilerTestCase("using2.boo", "using System.Text");
		}
		
		[Test]
		public void TestUsingAssemblyQualifiedNamespace()
		{
			RunCompilerTestCase("using3.boo", "using System.Drawing from System.Drawing");
		}
		
		[Test]
		public void TestUsingAlias()
		{
			RunCompilerTestCase("using4.boo", "using System as S");
		}
		
		[Test]
		public void TestUsingAssemblyQualifiedNamespace2()
		{
			RunCompilerTestCase("using5.boo", "using System.Drawing from different assembly");
		}
		
		[Test]
		public void TestUsingSameAssemblyQualifiedNamespaces()
		{
			RunCompilerTestCase("using6.boo", "using System.Drawing from two assemblies");
		}
		
		[Test]
		public void TestSimpleFor()
		{
			RunCompilerTestCase("for0.boo", "for item in list");
		}
		
		[Test]
		public void TestTypedFor()
		{
			RunCompilerTestCase("for1.boo", "for item as string in list");
		}
		
		[Test]
		public void TestUnpackFor()
		{
			RunCompilerTestCase("for2.boo", "for first, second in list");
		}
		
		[Test]
		public void TestPrivateForVariables()
		{
			RunCompilerTestCase("for3.boo", "for message in message:");
		}
		
		[Test]
		public void TestIfModifier0()
		{
			RunCompilerTestCase("if0.boo", "write() if true");
		}
		
		[Test]
		public void TestList0()
		{
			RunCompilerTestCase("list0.boo", "[]");
		}
		
		[Test]
		public void TestList1()
		{
			RunCompilerTestCase("list1.boo", "[1, 2, 3]");
		}
		
		[Test]
		public void TestEnum0()
		{
			RunCompilerTestCase("enum0.boo", "TestEnum.Foo");
		}
		
		[Test]
		public void TestVar0()
		{
			RunCompilerTestCase("var0.boo", "var as string");
		}
		
		[Test]
		public void TestVar1()
		{
			RunCompilerTestCase("var1.boo", "var as string = expression");
		}
		
		[Test]
		public void DelegateAddedWithConstructor()
		{
			RunCompilerTestCase("delegate0.boo", "basic delegate support");
		}
		
		[Test]
		public void DelegateAddedWithInPlaceAdd()
		{
			RunCompilerTestCase("delegate1.boo", "delegate += method");
		}
		
		[Test]
		public void DelegateWithInstanceMember()
		{
			RunCompilerTestCase("delegate2.boo", "delegate += object.method");
		}
		
		[Test]
		public void TestProperty0()
		{
			RunCompilerTestCase("property0.boo", "basic property support");
		}
		
		[Test]
		public void TestProperty1()
		{
			RunCompilerTestCase("property1.boo", "unpack property");
		}
		
		[Test]
		public void TestTuple0()
		{
			RunCompilerTestCase("tuple0.boo", "simple tuple creation");
		}
		
		[Test]
		public void TestTuple1()
		{
			RunCompilerTestCase("tuple1.boo", "tuple unpacking");
		}
		
		[Test]
		public void TestTuple3()
		{
			RunCompilerTestCase("tuple2.boo", "string.Format(template, tuple)");
		}
		
		[Test]
		public void TestMatch0()
		{
			RunCompilerTestCase("match0.boo", "string =~ string");
		}
		
		[Test]
		public void TestMatch1()
		{
			RunCompilerTestCase("match1.boo", "string !~ string");
		}
		
		[Test]
		public void TestNot0()
		{
			RunCompilerTestCase("not0.boo", "not true; not false");
		}
		
		[Test]
		public void TestTry0()
		{
			RunCompilerTestCase("try0.boo", "try/catch");
		}
		
		[Test]
		public void TestTry1()
		{
			RunCompilerTestCase("try1.boo", "try/catch/ensure");
		}
		
		[Test]
		public void TestTry2()
		{
			RunCompilerTestCase("try2.boo", "nested try/catch/ensure");
		}
		
		[Test]
		public void TestTry3()
		{
			RunCompilerTestCase("try3.boo", "raise with string argument");
		}
		
		[Test]
		public void SimpleInternalClass()
		{
			RunCompilerTestCase("class0.boo", "simple internal class");
		}
		
		[Test]
		public void InternalClassWithField()
		{
			RunCompilerTestCase("class1.boo", "internal class with field");
		}
		
		[Test]
		public void InternalClassWithProperty()
		{
			RunCompilerTestCase("class2.boo",
				"internal class with property");
		}
		
		[Test]                          
		public void InternalClassWithFieldAndPropertyCreateByAstAttribute()
		{
			RunCompilerTestCase("class3.boo",
				"internal class with field and property created by AstAttribute");
		}
		
		[Test]
		public void ToStringOverload()
		{
			RunCompilerTestCase("class4.boo", "ToString override");
		}
		
		[Test]
		public void ForwardInstanceMethodReference()
		{
			RunCompilerTestCase("class5.boo", "def initialize");
		}
		
		[Test]
		public void ForwardDelegateMethodReference()
		{
			RunCompilerTestCase("class6.boo", "App().Run()");
		}
		
		[Test]
		public void TestSimpleCast()
		{
			RunCompilerTestCase("cast0.boo", "(a as string).Substring()");
		}
		
		[Test]
		public void TestTypeTest()
		{
			RunCompilerTestCase("cast1.boo", "reference as string");
		}
		
		[Test]
		public void TestSimpleBaseClass()
		{
			RunCompilerTestCase("baseclass0.boo", "simplebaseclass");
		}
		
		[Test]
		public void TestOverrideBaseClassMethod()
		{
			RunCompilerTestCase("baseclass1.boo", "override base class method");
		}
		
		[Test]
		public void CallSuperMethod()
		{
			RunCompilerTestCase("baseclass2.boo", "call overriden method");
		}
		
		[Test]
		public void TestMethod0()
		{
			RunCompilerTestCase("method0.boo", "simple methods");
		}
		
		[Test]
		public void TestMethod1()
		{
			RunCompilerTestCase("method1.boo", "builtin redefinition");
		}
		
		[Test]
		public void TestMethod3()
		{
			RunCompilerTestCase("method3.boo", "simple return type decision");
		}
		
		[Test]
		public void TestMethod4()
		{
			RunCompilerTestCase("method4.boo", "method locals");
		}
		
		[Test]
		public void TestMethod5()
		{
			RunCompilerTestCase("method5.boo", "simple recursive method");
		}
		
		[Test]
		public void ConditionalReturnBranches()
		{
			RunCompilerTestCase("method7.boo", "conditional return branches");
		}
		
		[Test]
		public void ExceptionalReturnBranches()
		{
			RunCompilerTestCase("method8.boo", "exceptional return branches");
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
	}
}
