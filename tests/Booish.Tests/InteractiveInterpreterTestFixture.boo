namespace Booish.Tests

import System
import System.IO
import NUnit.Framework
import booish

[TestFixture]
class InteractiveInterpreterTestFixture:
	
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
