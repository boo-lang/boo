namespace Boo.Lang.Extensions

import Compiler
import Compiler.Ast

macro var:
	if len(var.Arguments) == 0:
		raise "var macro requires at least one variable declaration."
	for a in var.Arguments:
		yield DeclarationFromExpression(a)

internal def DeclarationFromExpression(e as Expression):
	identifier = e as ReferenceExpression
	if identifier is not null:
		return DeclarationStatement(Declaration(identifier.LexicalInfo, identifier.Name), null)

	assignment = e as BinaryExpression
	if (assignment is null
		or assignment.Operator != BinaryOperatorType.Assign
		or not assignment.Left isa ReferenceExpression):
		raise CompilerError(e, "var macro argument must be of the form `<identifier> = <expression>' or `<identifier>'.")

	name = assignment.Left cast ReferenceExpression
	initializer = assignment.Right
	return DeclarationStatement(Declaration(name.LexicalInfo, name.Name), initializer)
