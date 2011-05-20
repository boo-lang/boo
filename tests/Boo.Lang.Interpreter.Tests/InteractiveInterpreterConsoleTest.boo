
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
import NUnit.Framework
import Boo.Lang.Interpreter

[TestFixture]
class InteractiveInterpreterConsoleTest:
	
	_interpreter as InteractiveInterpreter
	_console as InteractiveInterpreterConsole
	
	[SetUp]
	def SetUp():
		_interpreter = InteractiveInterpreter()
		_console = InteractiveInterpreterConsole(_interpreter)
		
	[Test]
	def EvalRemembersLastValueAsDash():
		
		_console.Eval("3+3")
		assert 6 == _interpreter.GetValue("_")
		
		_console.Eval("'42'*3")
		assert "424242" == _interpreter.GetValue("_")
	
	[Test]
	def MethodReturningDynamicallyDefinedClassInstance():
		
		code = """class Foo:
	[getter(Value)] _value = null
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
		ConsoleLoopEval(code)
		assert AValueType().Equals(_interpreter.GetValue("v"))
		
	[Test]
	def ConversionError():
		output = ConsoleLoopEval("1 << 0x22")
		assert output.Contains("Constant value `17179869184L' cannot be converted to a `int'"), output
		
	[Test]
	def DontPrintArrayLengthInForLoop():
		output = ConsoleLoopEval("for i in (-1, 42): print i")
		Assert.AreEqual("-1\n42\n", output.Replace("\r\n", "\n"))
		
	def ConsoleLoopEval(code as string):
		using console=ConsoleCapture():
			_console.Eval(code)
		return console.ToString()
