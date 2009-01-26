"""
yes
Person.Set violated class invariant '_name is not null'.
Person.Set violated class invariant '_age >= 0'.
""" 
import Boo.Lang.Compiler.Ast
import Boo.Lang.PatternMatching

class Person:

	invariants:
		_name is not null
		_age >= 0
	
	_name = ""
	_age = 0
	
	def Set(name as string, age as int):
		_name = name
		_age = age

macro invariants:
	
	// member macros are members of their first TypeDefinition ancestor
	typeDefinition = invariants.GetAncestor[of TypeDefinition]()
	
	for member in typeDefinition.Members:
		method = member as Method
		continue if method is null
		
		for stmt as ExpressionStatement in invariants.Body.Statements:
			invariant = stmt.Expression
			message = "${member.FullName} violated class invariant '${invariant}'."
			method.Body = [|
				try:
					$(method.Body)
				ensure:
					assert $invariant, $message
			|].ToBlock()

def printingErrors(code as callable()):
	try:
		code()
	except x:
		print x.Message
		
printingErrors:
	Person().Set("", 0)
	print "yes"
	
printingErrors:
	Person().Set(null, 0)
	print "no"
	
printingErrors:
	Person().Set("", -1)
	print "no"

