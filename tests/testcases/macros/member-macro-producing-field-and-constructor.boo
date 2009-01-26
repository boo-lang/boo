"""
Pair(first: 42, second: ltuae)
"""

import Boo.Lang.Compiler.Ast
import Boo.Lang.PatternMatching

class IntStringPair:

	constructed_fields first as int, second as string
	
	override def ToString():
		return "Pair(first: ${first}, second: ${second})"

macro constructed_fields:
	ctor = [|
		def constructor():
			pass
	|]
	
	for arg in constructed_fields.Arguments:
		match arg:
			case [| $(ReferenceExpression(Name: fieldName)) as $fieldType |]:
				
				yield Field(Name: fieldName, Type: fieldType)
				
				param = ParameterDeclaration(Name: fieldName, Type: fieldType)
				ctor.Parameters.Add(param)
				ctor.Body.Add([| self.$fieldName = $param |])
	yield ctor
					
print IntStringPair(42, "ltuae")
