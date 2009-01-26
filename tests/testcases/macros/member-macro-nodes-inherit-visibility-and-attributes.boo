"""
"""
import System
import System.Reflection
import Boo.Lang.Compiler.Ast

class Foo:

	public properties:
		PublicProperty as string
		
	protected properties:
		ProtectedProperty as string
	
	[Obsolete] # attributes are applied to 
	private properties:
		PrivateProperty1 as int
		PrivateProperty2 as int

macro properties:
	for stmt as DeclarationStatement in properties.Body.Statements:
		name = stmt.Declaration.Name
		type = stmt.Declaration.Type
		yield [|
			$name as $type:
				get: raise NotImplementedException()
		|]
		
def GetProperty(name as string) as PropertyInfo:
	return typeof(Foo).GetProperty(name, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
	
assert GetProperty("PublicProperty").GetGetMethod().IsPublic
assert GetProperty("ProtectedProperty").GetGetMethod(true).IsFamily

private1 = GetProperty("PrivateProperty1")
assert private1.GetGetMethod(true).IsPrivate
assert private1.IsDefined(ObsoleteAttribute, false)

private2 = GetProperty("PrivateProperty2")
assert private2.GetGetMethod(true).IsPrivate
assert private2.IsDefined(ObsoleteAttribute, false)

