#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors
import System

def print(node as Node):
	BooPrinterVisitor(Console.Out).Switch(node)
	
def CreateNotExpression(e as Expression):
	return UnaryExpression(Operand: e, Operator: UnaryOperatorType.Not)

e = ExpressionStatement(
			Expression: be = BinaryExpression(BinaryOperatorType.Assign,
											ReferenceExpression("a"),
											IntegerLiteralExpression(3)
											)
					)
print(e)

be.ParentNode.Replace(be, MethodInvocationExpression(Target: ReferenceExpression("a")))
print(e)


i = IfStatement(Condition: be = BinaryExpression(BinaryOperatorType.NotMatch,
										StringLiteralExpression("foo"),
										StringLiteralExpression("bar")))
i.TrueBlock = Block()
//be.ReplaceBy(CreateNotExpression(be))
//i.Expression = CreateNotExpression(be)
i.Replace(be, CreateNotExpression(be))

be.Operator = BinaryOperatorType.Match
print(i)

