"""
"""
import System.Reflection

class Song:
	property Name as string

macro property:
	case [| property $propertyName as $propertyType |]:
		backingField = Boo.Lang.Compiler.Ast.ReferenceExpression("_" + propertyName)
		yield [|
			private $backingField as $propertyType
		|] 
		yield [|
			$propertyName as $propertyType:
				get: return $backingField
				set: $backingField = value
		|]
		
assert string is typeof(Song).GetProperty("Name", BindingFlags.Public|BindingFlags.Instance).PropertyType
assert string is typeof(Song).GetField("_Name", BindingFlags.NonPublic|BindingFlags.Instance).FieldType
