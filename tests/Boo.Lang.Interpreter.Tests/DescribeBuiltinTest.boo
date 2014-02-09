namespace Boo.Lang.Interpreter.Tests

import System
import System.Text
import Boo.Lang.Interpreter
import NUnit.Framework

[TestFixture]
class DescribeBuiltinTest:
	
	[SetUp]
	def SetUp():
		pass
	
	[Test]
	def ClassDescription():
		
		expected = """
class Person(object):

    def constructor(name as string)

    def constructor()

    public LastName as string

    FirstName as string:
        get
        set

    def Equals(obj as object) as bool
    ${'"""'}
    ${'"""'}

    def GetHashCode() as int
    ${'"""'}
    ${'"""'}

    def GetType() as System.Type
    ${'"""'}
    ${'"""'}

    def ToString() as string
    ${'"""'}
    ${'"""'}

    event Changed as System.EventHandler

"""
		AssertDescription(expected, Person)
		
	[Test]
	def InterfaceDescription():
			
		expected = """
interface AnInterface():

    AProperty as int:
        get
        set

    def AMethod() as void

"""
		AssertDescription(expected, AnInterface)
		
	def AssertDescription(expected as string, type as System.Type):
		using console=ConsoleCapture():
			Boo.Lang.Interpreter.describe(type)
		
		# mono compatibility fix
		# object.Equals arg on mono is called o
		actual = console.ToString().Replace("o as object", "obj as object")
		
		Console.WriteLine(actual)
		
		Assert.AreEqual(ns(expected), ns(actual))
		
	static def ns(s as string):
		s = s.Trim().Replace("\r\n", "\n").Replace("\t", "    ")
		lines = s.Split("\n"[0])
		sb = StringBuilder()
		#region Skip Documentation
		lineNo = 0
		while lineNo < lines.Length:
			line = lines[lineNo]
			sb.AppendLine(line)
			if "\"\"\"".Equals(line.Trim()):
				lineNo+=1
				while lineNo < lines.Length:
					line = lines[lineNo]
					if "\"\"\"".Equals(line.Trim()):
						break
					lineNo += 1
				sb.AppendLine(line)
			lineNo += 1
		return sb.ToString()
		
class Person:
	
	_fname as string
	
	public LastName as string
	
	FirstName:
		get: return _fname
		set: _fname = value
	
	event Changed as System.EventHandler
	
	def constructor(name as string):
		_fname = name
	
	def constructor():
		pass
	
interface AnInterface:
	def AMethod()
	AProperty as int:
		get
		set

