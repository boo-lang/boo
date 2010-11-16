namespace Boo.Lang.Interpreter.Tests

import System
import Boo.Lang.Interpreter
import NUnit.Framework

[TestFixture]
class DescribeBuiltinTest:
	
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

    def GetHashCode() as int

    def GetType() as System.Type

    def ToString() as string

    event Changed as System.EventHandler

"""
		AssertDescription(expected, Person)
		
	[Test]
	def InterfaceDescription():
			
		expected = """
interface IDisposable():

    def Dispose() as void

"""
		AssertDescription(expected, System.IDisposable)
		
	def AssertDescription(expected as string, type as System.Type):
		using console=ConsoleCapture():
			Boo.Lang.Interpreter.Builtins.describe(type)
		
		# mono compatibility fix
		# object.Equals arg on mono is called o
		actual = console.ToString().Replace("o as object", "obj as object")
		
		Assert.AreEqual(ns(expected), ns(actual))
		
	static def ns(s as string):
		return s.Trim().Replace("\r\n", "\n")
		
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
	
		
