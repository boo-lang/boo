namespace Boo.Lang.Extensions

import Compiler.Ast

macro var:
	case [| var $(name=ReferenceExpression()) = $initializer |]:
		yield DeclarationStatement(
			Declaration(name.LexicalInfo, name.Name),
			initializer)

