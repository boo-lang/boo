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
	def UseInterpreterValues():
		using console=ConsoleCapture():
			Eval("print(name);print(age)")
		newLine = Environment.NewLine
		Assert.AreEqual("boo${newLine}3${newLine}", console.ToString())
			
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
	def AssignmentPreservesType():
		Eval("value = (a = 3).ToString()")
		assert 3 == _interpreter.GetValue("a")
		assert "3" == _interpreter.GetValue("value")
		
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
		
	/*
	[Test]
	def UnderscoreHoldsLastEvaluatedExpression():
		Eval("a = 42")
		Eval("b = _/2")
		assert 21 == _interpreter.GetValue("b")
		assert 21 == _interpreter.GetValue("_")
		
	[Test]
	def EvaluateSimpleExpression():
		Eval("2+2")
		assert 4 == _interpreter.GetValue("_")
	*/
		
	[Test]
	def Closures():
	
		Eval("x2 = { v as int | return v*2 }")

		x2 as callable = _interpreter.GetValue("x2")
		assert x2 is not null
		assert 4 == x2(2)
		
	[Test]
	def GeneratorExpressions():
	
		_interpreter.SetValue("value", 3)
		Eval("e = i*2 for i in range(value)")
		assert array(_interpreter.GetValue("e")) == (0, 2, 4)
		
	[Test]
	def Loop():

		# let's loop
		Eval("""
l = []
for i in range(3):
	l.Add(i*2)
""")
		Assert.AreEqual([0, 2, 4], _interpreter.GetValue("l"))
		
	def Eval(code as string):
		result = _interpreter.Eval(code)
		assert 0 == len(result.Errors), result.Errors.ToString(true)
