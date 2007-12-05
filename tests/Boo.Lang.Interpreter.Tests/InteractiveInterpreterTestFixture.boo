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
namespace Boo.Lang.Interpreter.Tests

import System
import System.IO
import NUnit.Framework
import Boo.Lang.Interpreter
import Boo.Lang.Compiler.TypeSystem

struct AValueType:
	pass

[TestFixture]
class InteractiveInterpreterTestFixture:
	
	public static LifeTheUniverseAndEverything = 42
	
	_interpreter as InteractiveInterpreter
	
	[SetUp]
	def SetUp():
		_interpreter = InteractiveInterpreter()
		_interpreter.SetValue("name", "boo")
		_interpreter.SetValue("age", 3)	
		
	[Test]
	def DefaultValues():
		assert false == _interpreter.RememberLastValue
		assert _interpreter.LastValue is null
		
	[Test]
	def EvalEmptyString():
		Eval("")
		assert _interpreter.LastValue is null
		
	[Test]
	def UseInterpreterValues():
		using console=ConsoleCapture():
			Eval("print(name);print(age)")
		newLine = Environment.NewLine
		Assert.AreEqual("boo${newLine}3${newLine}", console.ToString())
		
	[Test]
	def LoopEvalCapturesConsoleOut():
		lines = []
		_interpreter.Print = { line | lines.Add(line) }
		_interpreter.LoopEval("System.Console.WriteLine('Hello, world')")
		Assert.AreEqual(1, len(lines))
		Assert.AreEqual("Hello, world", lines[0])
		
	[Test]
	def Unpacking():
		Eval("a, b = 1, 2")
		assert 1 == _interpreter.GetValue("a")
		assert 2 == _interpreter.GetValue("b")
		
		Eval("a, b = b, a")
		assert 2 == _interpreter.GetValue("a")
		assert 1 == _interpreter.GetValue("b")
		
	[Test]
	def ArraySlicing():
		Eval("a = 1, 2, 3")
		Eval("a1 = a[0]")
		Eval("a12 = a[0:-1]")
		Eval("a3 = a[-1]")
		
		assert _interpreter.GetValue("a") == (1, 2, 3)
		assert _interpreter.GetValue("a1") == 1
		assert _interpreter.GetValue("a12") == (1, 2)
		assert _interpreter.GetValue("a3") == 3
		
	[Test]
	def AssignmentToSlice():
		Eval("a = 1, 2, 3")
		Eval("a[-1] = 1")
		Eval("a[0] = 3")
		
		assert _interpreter.GetValue("a") == (3, 2, 1)
			
	[Test]
	def ChangeInterpreterValues():
		Eval("age = 4")
		assert 4 == _interpreter.GetValue("age")

		Eval("age = 42")
		assert 42 == _interpreter.GetValue("age")
		
	[Test]
	def Import():
		Eval("import Boo.Lang.Interpreter.Tests from Boo.Lang.Interpreter.Tests")
		Eval("value = InteractiveInterpreterTestFixture.LifeTheUniverseAndEverything")
		assert 42 == _interpreter.GetValue("value")
		
	[Test]
	def ImportingTwiceIsNoProblem():
		Eval("import Boo.Lang.Interpreter.Tests from Boo.Lang.Interpreter.Tests")
		Eval("import Boo.Lang.Interpreter.Tests from Boo.Lang.Interpreter.Tests")
		
		Eval("value = InteractiveInterpreterTestFixture.LifeTheUniverseAndEverything")
		assert 42 == _interpreter.GetValue("value")		
		
	[Test]
	def AssignmentPreservesType():
		Eval("value = (a = 3).ToString()")
		Eval("a2 = a*2")
		Eval("value2 = value*2")
		assert 3 == _interpreter.GetValue("a")
		assert "3" == _interpreter.GetValue("value")
		assert 6 == _interpreter.GetValue("a2")
		assert "33" == _interpreter.GetValue("value2")
		
	[Test]
	[ExpectedException(System.Reflection.TargetInvocationException)]
	def RaiseInsideEval():
		Eval("raise System.ApplicationException()")
		
	[Test]
	def TreatObjectsAsDucks():
		Eval("""
class Person:
	[property(Name)] _name = ''
	
p as object = Person(Name: 'John')
""")
		Eval("name = p.Name")
		assert "John" == _interpreter.GetValue("name")
		
	[Test]
	def TreatObjectsAsDucksCanBeDisabled():
		Eval("""
class Person:
	[property(Name)] _name = ''
	
p as object = Person(Name: 'John')
""")
		_interpreter.Ducky = false
		result = _interpreter.Eval("name = p.Name")
		assert 1 == len(result.Errors)
		
	[Test]
	def InterpreterRememberDeclaredTypes():
		Eval("i as object = 3")
		assert 3 == _interpreter.GetValue("i")
		Eval("i = '3' # is is typed object")
		assert "3" == _interpreter.GetValue("i")
		
	[Test]
	def MethodDef():
		
		_interpreter.SetValue("eggs", "eggs")
		
		Eval("""
def spam():
	return eggs
""")
		
		Eval("value = spam()")
		assert 'eggs' == _interpreter.GetValue("value")
		
	[Test]
	def InterpreterReference():
		
		assert _interpreter.GetValue("interpreter") is _interpreter 
		
	[Test]
	def Reset():
		
		_interpreter.RememberLastValue = true
		
		Eval("a as int = 3")
		Eval("a")		
		
		assert 3 == _interpreter.LastValue
		assert 3 == _interpreter.GetValue("a")
		assert int is _interpreter.Lookup("a")
		
		_interpreter.Reset()
		
		assert _interpreter.LastValue is null
		assert _interpreter.GetValue("a") is null
		assert _interpreter.Lookup("a") is null
		
		
	[Test]
	def TypeDef():
		
		_interpreter.SetValue("DefaultName", "boo")
		
		Eval("""
class Language:
	[property(Name)] _name = DefaultName
""")
	
		Eval("language = Language()")
		language as duck = _interpreter.GetValue("language")
		assert language is not null
		assert 'boo' == language.Name
		
		Eval("language = Language(Name: 'portuguese')")
		language = _interpreter.GetValue("language")
		assert language is not null
		assert 'portuguese' == language.Name

	[Test]
	def ExpressionWithoutSideEffectsNotAllowedInDefaultMode():
		_interpreter.RememberLastValue = false
		errors = _interpreter.Eval("2+2").Errors
		
		assert 1 == len(errors)
		assert "BCE0034" == errors[0].Code
		

	[Test]
	def BeginInvokeEndInvoke():
		Eval("""
def foo():
	return 42

handle = foo.BeginInvoke(null, null)
result = foo.EndInvoke(handle)
""")

		handle as duck = _interpreter.GetValue("handle")
		assert handle is not null
		assert handle.IsCompleted
		assert handle.EndInvokeCalled
		
		Assert.AreEqual(42, cast(int, _interpreter.GetValue("result")))		

	[Test]
	def BeginInvokeEndInvokeInSequentialEvals():
		Eval("""
def foo():
	return 42

handle = foo.BeginInvoke(null, null)
""")
		handle as duck = _interpreter.GetValue("handle")
		assert handle is not null
		assert not handle.EndInvokeCalled

		Eval("result = foo.EndInvoke(handle)")

		Assert.AreEqual(42, cast(int, _interpreter.GetValue("result")))
		
		assert handle.IsCompleted
		assert handle.EndInvokeCalled
		
	[Test]
	def MethodReferences():
		Eval("""
def foo():
	return 42""")
	
		Eval("f1 = foo")
		Eval("f2 = foo")
		
		f1 as callable = _interpreter.GetValue("f1")
		f2 as callable = _interpreter.GetValue("f2")
		assert f1 is not null
		assert 42 == f1()
		
		assert f2 is not null
		assert 42 == f2()
		
	[Test]
	def EvalSimpleLiterals():
		
		_interpreter.RememberLastValue = true
		Eval("true")
		assert true == _interpreter.LastValue
		
		Eval("false")
		assert false == _interpreter.LastValue
		
		Eval("null")
		assert _interpreter.LastValue is null
	
	[Test]
	def RememberLastValue():
		_interpreter.RememberLastValue = true
		
		Eval("2+2")
		assert 4 == _interpreter.LastValue
		
		Eval("3")
		assert 3 == _interpreter.LastValue
		
	[Test]
	def DisableRememberLastValue():
		_interpreter.RememberLastValue = false
		Eval("a=2+2")
		assert _interpreter.LastValue is null
		
	[Test]
	def MethodVariablesAreNotGlobalToTheInterpreter():
		Eval("""
def foo():
	a = 3
	
b = 4""")
		assert int is _interpreter.Lookup("b")
		assert _interpreter.Lookup("a") is null
		
	[Test]
	def EvaluateClosure():
		_interpreter.RememberLastValue = true
		Eval("{ return 42 }")
		assert 42 == cast(callable, _interpreter.LastValue)()
		
	[Test]
	def EvaluateVoidFunctionSetsValueToNull():
		
		_interpreter.RememberLastValue = true
		Eval("""
def dummy():
	pass
42
dummy()""")
		assert _interpreter.LastValue is null
		
	[Test]
	def Closures():
	
		Eval("x2 = { v as int | return v*2 }")

		x2 as callable = _interpreter.GetValue("x2")
		assert x2 is not null
		assert 4 == x2(2)
		
	[Test]
	def EvalSimpleReferenceGetsItsValue():
		
		_interpreter.RememberLastValue = true
		
		Eval("name")
		assert "boo" == _interpreter.LastValue
		
		Eval("age")
		assert 3 == _interpreter.LastValue
		
	[Test]
	def GeneratorExpressions():
	
		_interpreter.SetValue("value", 3)
		Eval("e = i*2 for i in range(value)")
		assert array(_interpreter.GetValue("e")) == (0, 2, 4)
		
	[Test]
	def OverflowsFailFast():
		_interpreter.SetValue("value", int.MaxValue)
		Eval("""
try:
	value += 1
except x as System.OverflowException:
	overflow = x
""")
		assert _interpreter.GetValue("overflow") is not null
		
		
	[Test]
	def LoopEval():
		
		value = ""
		_interpreter.RememberLastValue = true
		_interpreter.Print = { item | value = item }		
		_interpreter.LoopEval("3+3")
		assert "6" == value
		assert 6 == _interpreter.GetValue("_")
		
		_interpreter.LoopEval("'42'*3")
		assert "'424242'" == value
		assert "424242" == _interpreter.GetValue("_")
		
	class Customer:
		
		[Property(FirstName)]
		_fname as string
		
		public LastName as string
		
		event Changed as System.EventHandler
		
		def constructor(name as string):
			_fname = name
		
		def constructor():
			pass
		
	[Test]
	def HelpOnClass():
		
		expected = """
class Customer(object):

    def constructor(name as string)

    def constructor()

    public LastName as string

    FirstName as string:
        get
        set

    def Equals(obj as object) as bool

    def GetHashCode() as int

    def GetType() as System.Type

    def ToString() as string

    event Changed as System.EventHandler

"""
		assertHelp(expected, Customer)
		
	[Test]
	def HelpOnInterface():
			
		expected = """
interface IDisposable():

    def Dispose() as void

"""
		assertHelp(expected, System.IDisposable)
		
	def assertHelp(expected as string, type as System.Type):
		buffer = System.IO.StringWriter()
		_interpreter.Print = { item | buffer.WriteLine(item) }		
		_interpreter.help(type)
		
		# mono compatibility fix
		# object.Equals arg on mono is called o
		actual = buffer.ToString().Replace("o as object", "obj as object")
		
		Assert.AreEqual(ns(expected), ns(actual))
		
	static def ns(s as string):
		return s.Trim().Replace("\r\n", Environment.NewLine)
	
		
	[Test]
	def Loop():

		# let's loop
		Eval("""
l = []
for i in range(3):
	l.Add(i*2)
""")
		Assert.AreEqual([0, 2, 4], _interpreter.GetValue("l"))
		
	[Test]
	def UnpackingLoop():
		Eval("""
l = []
for i, j in ((1, 2), (3, 4)):
	l.Add((j, i))
""")

		Assert.AreEqual([(2, 1), (4, 3)], _interpreter.GetValue("l"))
		
	[Test]
	def ResolveEntity():
		Eval("a = 3")
		suggestion = _interpreter.ResolveEntity("a.__codecomplete__")
		assert suggestion isa ExternalType
		assert int is (suggestion as ExternalType).ActualType
		
		suggestion = _interpreter.ResolveEntity("'foo'.ToUpper().__codecomplete__")
		assert suggestion isa ExternalType
		assert string is (suggestion as ExternalType).ActualType
		
	[Test]
	def ResolveNamespaceEntity():
		suggestion = _interpreter.ResolveEntity("System.__codecomplete__")
		assert suggestion is not null
		assert suggestion.EntityType == EntityType.Namespace
		
	[Test]
	def ResolveEntityReferencesAreInSync():
		_interpreter.References.Add(System.Reflection.Assembly.GetExecutingAssembly())
		suggestion = _interpreter.ResolveEntity("Boo.Lang.Interpreter.Tests.__codecomplete__")
		assert suggestion is not null
		assert suggestion.EntityType == EntityType.Namespace
		
	[Test]
	def ResolveEntityFromImportedNamespaces():
		_interpreter.Eval("import System")
		suggestion = _interpreter.ResolveEntity("Console.__codecomplete__")
		assert suggestion is not null
		assert System.Console is (suggestion as ExternalType).ActualType
		
	[Test]
	def ResolveEntityForImportStatement():
		suggestion = _interpreter.ResolveEntity("import    System.__codecomplete__")
		assert suggestion is not null
		assert suggestion.EntityType == EntityType.Namespace
		
	[Test]
	def DeclarationInitializesRefType():
		Eval("""
for i in range(3):
	item as string
	assert item is null
	item = i.ToString()""")
		assert "2" == _interpreter.GetValue("item")
		
		
	[Test]
	def DeclarationInitializesValueType():
		Eval("""
for i in range(3):
	item as int
	assert 0 == item
	item = i""")
		assert 2 == _interpreter.GetValue("item")
		
	[Test]
	def DeclarationInitializersInsideMethod():
		Eval("""
def foo():
	for i in range(3):
		item as string
		assert item is null
		item = i.ToString()
	return item
		
value = foo()""")
		assert "2" == _interpreter.GetValue("value")

	[Test]
	def MethodReturningDynamicallyDefinedClassInstance():
		
		code = """class Foo:
	[getter(Value)] _value
	def constructor(value):
		_value = value

def foo():
	return Foo(42)	

value = foo().Value
"""
		ConsoleLoopEval(code)			
		assert 42 == _interpreter.GetValue("value")
			
	[Test]
	def ValueType():
		code = """
import Boo.Lang.Interpreter.Tests from Boo.Lang.Interpreter.Tests

v = AValueType()
"""
		_interpreter.RememberLastValue = true
		ConsoleLoopEval(code)
		assert AValueType().Equals(_interpreter.GetValue("v"))
		
	def Eval(code as string):
		result = _interpreter.Eval(code)
		assert 0 == len(result.Errors), result.Errors.ToString(true)
		
	def ConsoleLoopEval(code as string):
		oldIn = Console.In
		oldOut = Console.Out
		try:
			Console.SetIn(StringReader(code))
			Console.SetOut(writer = StringWriter())
			_interpreter.ConsoleLoopEval()			
		ensure:
			Console.SetIn(oldIn)
			Console.SetOut(oldOut)
		return writer.ToString()
		

