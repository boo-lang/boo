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
namespace Booish.Tests

import System
import System.IO
import NUnit.Framework
import booish

[TestFixture]
class InteractiveInterpreterTestFixture:
	
	public static LifeTheUniverseAndEverything = 42
	
	class ConsoleCapture(IDisposable):	
		_console = StringWriter()
		_old
		
		def constructor():
			_old = Console.Out
			Console.SetOut(_console)
			
		override def ToString():
			return _console.ToString()
		
		def Dispose():
			Console.SetOut(_old)

	_interpreter as InteractiveInterpreter
	
	[SetUp]
	def SetUp():
		_interpreter = InteractiveInterpreter()
		_interpreter.SetValue("name", "boo")
		_interpreter.SetValue("age", 3)
		
	[Test]
	def DefaultValues():
		assert false == _interpreter.RememberLastValue
		
	[Test]
	def UseInterpreterValues():
		using console=ConsoleCapture():
			Eval("print(name);print(age)")
		newLine = Environment.NewLine
		Assert.AreEqual("boo${newLine}3${newLine}", console.ToString())
		
	[Test]
	def Unpacking():
		Eval("a, b = 1, 2")
		assert 1 == _interpreter.GetValue("a")
		assert 2 == _interpreter.GetValue("b")
			
	[Test]
	def ChangeInterpreterValues():
		Eval("age = 4")
		assert 4 == _interpreter.GetValue("age")

		Eval("age = 42")
		assert 42 == _interpreter.GetValue("age")
		
	[Test]
	def Import():
		Eval("import Booish.Tests from Booish.Tests")
		Eval("value = InteractiveInterpreterTestFixture.LifeTheUniverseAndEverything")
		assert 42 == _interpreter.GetValue("value")
		
	[Test]
	def ImportingTwiceIsNoProblem():
		Eval("import Booish.Tests from Booish.Tests")
		Eval("import Booish.Tests from Booish.Tests")
		
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
	def TypeDef():
		
		_interpreter.SetValue("DefaultName", "boo")
		
		Eval("""
class Language:
	[property(Name)] _name = DefaultName
""")
	
		Eval("language = Language()")
		language as duck = _interpreter.GetValue("language")
		assert 'boo' == language.Name
		
		Eval("language = Language(Name: 'portuguese')")
		language = _interpreter.GetValue("language")
		assert 'portuguese' == language.Name

	[Test]
	def ExpressionWithoutSideEffectsNotAllowedInDefaultMode():
		_interpreter.RememberLastValue = false
		assert 1 == len(_interpreter.Eval("2+2").Errors)
	
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
		assert _interpreter.Lookup("b") is int
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
		
	def Eval(code as string):
		result = _interpreter.Eval(code)
		assert 0 == len(result.Errors), result.Errors.ToString(true)
