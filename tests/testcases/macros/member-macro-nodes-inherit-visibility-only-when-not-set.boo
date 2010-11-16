"""
"""
import System
import System.Reflection
import Boo.Lang.Compiler.Ast

class Foo:
	protected properties:
		ProtectedProperty as string

macro properties:
	for stmt as DeclarationStatement in properties.Body.Statements:
		name = stmt.Declaration.Name
		type = stmt.Declaration.Type
		yield [|
			$name as $type:
				get: raise NotImplementedException()
		|]
		yield [|
			// backing field
			private $("_$name") as $type
		|]
		
def GetProperty(name as string):
	return typeof(Foo).GetProperty(name, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
	
def GetField(name as string):
	return typeof(Foo).GetField(name, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
	
assert GetProperty("ProtectedProperty").GetGetMethod(true).IsFamily
assert GetField("_ProtectedProperty").IsPrivate

