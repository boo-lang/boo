"""
"""
import Boo.Lang.PatternMatching
import System.Reflection

class Song:
	property Name as string

macro property:
	case [| property $propertyName as $propertyType |]:
		fieldName = "_" + propertyName
		yield [|
			private $fieldName as $propertyType
		|]
		fieldRef = Boo.Lang.Compiler.Ast.ReferenceExpression(fieldName)
		yield [|
			$propertyName as $propertyType:
				get: return $fieldRef
				set: $fieldRef = value
		|]
		
assert string is typeof(Song).GetProperty("Name", BindingFlags.Public|BindingFlags.Instance).PropertyType
assert string is typeof(Song).GetField("_Name", BindingFlags.NonPublic|BindingFlags.Instance).FieldType
