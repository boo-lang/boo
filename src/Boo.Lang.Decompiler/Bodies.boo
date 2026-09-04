#region license
// Copyright (c) 2026 the Boo contributors
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Decompiler

import System
import System.Text
import ICSharpCode.Decompiler.CSharp.Syntax

internal class Bodies:
"""
Converts a decompiled method body to Boo.

C# with no Boo equivalent becomes a "# TODO" comment holding the original,
so nothing is silently dropped. The output is for reading, not compiling.
"""

	static def Of(block as BlockStatement, indent as string) as string:
		return "${indent}pass\n" if block is null or block.IsNull
		written = Statements(block, indent)
		return "${indent}pass\n" if string.IsNullOrEmpty(written)
		return written

	private static def Statements(block as BlockStatement, indent as string) as string:
		written = StringBuilder()
		for statement in block.Statements:
			written.Append(Statement(statement, indent))
		return written.ToString()

	private static def Statement(node as Statement, indent as string) as string:
		return "" if node is null or node isa EmptyStatement

		block = node as BlockStatement
		return Statements(block, indent) if block is not null

		returned = node as ReturnStatement
		if returned is not null:
			return "${indent}return\n" if returned.Expression.IsNull
			return "${indent}return ${Value(returned.Expression)}\n"

		expressed = node as ExpressionStatement
		return "${indent}${Value(expressed.Expression)}\n" if expressed is not null

		declared = node as VariableDeclarationStatement
		return Declaration(declared, indent) if declared is not null

		branch = node as IfElseStatement
		return Branch(branch, indent) if branch is not null

		loop = node as WhileStatement
		return "${indent}while ${Value(loop.Condition)}:\n${Nested(loop.EmbeddedStatement, indent)}" if loop is not null

		each = node as ForeachStatement
		if each is not null:
			return "${indent}for ${each.VariableDesignation} in ${Value(each.InExpression)}:\n${Nested(each.EmbeddedStatement, indent)}"

		thrown = node as ThrowStatement
		if thrown is not null:
			return "${indent}raise\n" if thrown.Expression.IsNull
			return "${indent}raise ${Value(thrown.Expression)}\n"

		guarded = node as TryCatchStatement
		return Guarded(guarded, indent) if guarded is not null

		held = node as UsingStatement
		if held is not null:
			return "${indent}using ${Value(held.ResourceAcquisition as Expression)}:\n${Nested(held.EmbeddedStatement, indent)}" if held.ResourceAcquisition isa Expression
			return Unwritten(node, indent)

		counted = node as ForStatement
		return Counted(counted, indent) if counted is not null

		chosen = node as SwitchStatement
		return Chosen(chosen, indent) if chosen is not null

		repeated = node as DoWhileStatement
		if repeated is not null:
			# Boo has no do while, so the test moves to the end of the body.
			body = Nested(repeated.EmbeddedStatement, indent)
			return "${indent}while true:\n${body}${indent}\tbreak unless ${Value(repeated.Condition)}\n"

		return "${indent}break\n" if node isa BreakStatement
		return "${indent}continue\n" if node isa ContinueStatement

		return Unwritten(node, indent)

	private static def Chosen(chosen as SwitchStatement, indent as string) as string:
	"""
	A switch as an if/elif chain, since Boo has no switch.

	The break ending each section only exits the switch, which the chain
	does anyway, so it is dropped.
	"""
		written = StringBuilder()
		subject = Value(chosen.Expression)
		first = true
		fallen = StringBuilder()
		for section as SwitchSection in chosen.SwitchSections:
			test = Test(subject, section)
			body = Section(section, indent)
			if test is null:
				fallen.Append(indent).Append("else:\n").Append(body)
				continue
			written.Append(indent).Append(("if " if first else "elif ")).Append(test).Append(":\n")
			written.Append(body)
			first = false
		written.Append(fallen.ToString())
		return written.ToString()

	private static def Test(subject as string, section as SwitchSection) as string:
	"""The condition for a section, or null for the default one."""
		tests = System.Collections.Generic.List[of string]()
		for label as CaseLabel in section.CaseLabels:
			return null if label.Expression.IsNull
			tests.Add("${subject} == ${Value(label.Expression)}")
		return null if tests.Count == 0
		return string.Join(" or ", tests.ToArray())

	private static def Section(section as SwitchSection, indent as string) as string:
		written = StringBuilder()
		for statement as Statement in section.Statements:
			continue if statement isa BreakStatement
			written.Append(Statement(statement, indent + "\t"))
		return "${indent}\tpass\n" if written.Length == 0
		return written.ToString()

	private static def Guarded(guarded as TryCatchStatement, indent as string) as string:
		written = StringBuilder()
		written.Append(indent).Append("try:\n").Append(Nested(guarded.TryBlock, indent))
		for caught as CatchClause in guarded.CatchClauses:
			written.Append(indent).Append("except")
			unless caught.Type.IsNull:
				name = (caught.VariableName if not string.IsNullOrEmpty(caught.VariableName) else "e")
				written.Append(" ").Append(name).Append(" as ").Append(Decompiled.BooType(caught.Type))
			written.Append(":\n").Append(Nested(caught.Body, indent))
		unless guarded.FinallyBlock.IsNull:
			written.Append(indent).Append("ensure:\n").Append(Nested(guarded.FinallyBlock, indent))
		return written.ToString()

	private static def Counted(counted as ForStatement, indent as string) as string:
	"""
	A C# for loop as a while, since Boo has no three part for.

	The setup runs before the loop and the step at the end of the body. A
	continue would skip that step, so those loops are left unconverted.
	"""
		return Unwritten(counted, indent) if Continues(counted.EmbeddedStatement)
		written = StringBuilder()
		for setup as Statement in counted.Initializers:
			written.Append(Statement(setup, indent))
		written.Append(indent).Append("while ").Append(Value(counted.Condition)).Append(":\n")
		written.Append(Nested(counted.EmbeddedStatement, indent))
		for step as Statement in counted.Iterators:
			written.Append(Statement(step, indent + "\t"))
		return written.ToString()

	private static def Continues(node as AstNode) as bool:
		return true if node isa ContinueStatement
		for child in node.Children:
			return true if Continues(child)
		return false

	private static def Declaration(declared as VariableDeclarationStatement, indent as string) as string:
		written = StringBuilder()
		for variable as VariableInitializer in declared.Variables:
			written.Append(indent).Append(variable.Name)
			if variable.Initializer.IsNull:
				written.Append(" as ").Append(Decompiled.BooType(declared.Type)).Append("\n")
			else:
				written.Append(" = ").Append(Value(variable.Initializer)).Append("\n")
		return written.ToString()

	private static def Branch(branch as IfElseStatement, indent as string) as string:
		written = StringBuilder()
		written.Append(indent).Append("if ").Append(Value(branch.Condition)).Append(":\n")
		written.Append(Nested(branch.TrueStatement, indent))
		unless branch.FalseStatement.IsNull:
			written.Append(indent).Append("else:\n")
			written.Append(Nested(branch.FalseStatement, indent))
		return written.ToString()

	private static def Nested(node as Statement, indent as string) as string:
		written = Statement(node, indent + "\t")
		return "${indent}\tpass\n" if string.IsNullOrEmpty(written)
		return written

	private static def Value(node as Expression) as string:
	"""One expression, or a "# TODO" comment holding the C# it came from."""
		return "null" if node is null or node.IsNull

		literal = node as PrimitiveExpression
		return Literal(literal) if literal is not null

		named = node as IdentifierExpression
		return named.Identifier if named is not null

		member = node as MemberReferenceExpression
		return "${Value(member.Target)}.${member.MemberName}" if member is not null

		call = node as InvocationExpression
		return "${Value(call.Target)}(${Arguments(call.Arguments)})" if call is not null

		made = node as ObjectCreateExpression
		return "${Decompiled.BooType(made.Type)}(${Arguments(made.Arguments)})" if made is not null

		binary = node as BinaryOperatorExpression
		if binary is not null:
			operator = Operator(binary.Operator)
			return Unwritten(node) if operator is null
			return "${Value(binary.Left)} ${operator} ${Value(binary.Right)}"

		unary = node as UnaryOperatorExpression
		if unary is not null:
			after = Suffix(unary.Operator)
			return "${Value(unary.Expression)}${after}" if after is not null
			operator = Prefix(unary.Operator)
			return Unwritten(node) if operator is null
			return "${operator}${Value(unary.Expression)}"

		assigned = node as AssignmentExpression
		if assigned is not null:
			operator = Assignment(assigned.Operator)
			return Unwritten(node) if operator is null
			return "${Value(assigned.Left)} ${operator} ${Value(assigned.Right)}"

		converted = node as CastExpression
		return "cast(${Decompiled.BooType(converted.Type)}, ${Value(converted.Expression)})" if converted is not null

		indexed = node as IndexerExpression
		return "${Value(indexed.Target)}[${Arguments(indexed.Arguments)}]" if indexed is not null

		wrapped = node as ParenthesizedExpression
		return "(${Value(wrapped.Expression)})" if wrapped is not null

		chosen = node as ConditionalExpression
		if chosen is not null:
			return "(${Value(chosen.TrueExpression)} if ${Value(chosen.Condition)} else ${Value(chosen.FalseExpression)})"

		named_type = node as TypeReferenceExpression
		return Decompiled.BooType(named_type.Type) if named_type is not null

		return "self" if node isa ThisReferenceExpression
		return "super" if node isa BaseReferenceExpression
		return "null" if node isa NullReferenceExpression

		return Unwritten(node)

	private static def Literal(literal as PrimitiveExpression) as string:
		value = literal.Value
		return "null" if value is null
		return ("true" if cast(bool, value) else "false") if value isa bool
		return Quoted(value as string) if value isa string
		return "char('${value}')" if value isa char
		return value.ToString()

	private static def Quoted(text as string) as string:
	"""A Boo string literal, with quotes and backslashes escaped."""
		quote = char('"').ToString()
		slash = char('\\').ToString()
		written = text.Replace(slash, slash + slash).Replace(quote, slash + quote)
		return quote + written + quote

	private static def Arguments(arguments as AstNodeCollection[of Expression]) as string:
		written = System.Collections.Generic.List[of string]()
		for argument in arguments:
			written.Add(Value(argument))
		return string.Join(", ", written.ToArray())

	private static def Assignment(operator as AssignmentOperatorType) as string:
		return "=" if operator == AssignmentOperatorType.Assign
		return "+=" if operator == AssignmentOperatorType.Add
		return "-=" if operator == AssignmentOperatorType.Subtract
		return "*=" if operator == AssignmentOperatorType.Multiply
		return "/=" if operator == AssignmentOperatorType.Divide
		return "%=" if operator == AssignmentOperatorType.Modulus
		return "<<=" if operator == AssignmentOperatorType.ShiftLeft
		return ">>=" if operator == AssignmentOperatorType.ShiftRight
		return "&=" if operator == AssignmentOperatorType.BitwiseAnd
		return "|=" if operator == AssignmentOperatorType.BitwiseOr
		return "^=" if operator == AssignmentOperatorType.ExclusiveOr
		return null

	private static def Operator(operator as BinaryOperatorType) as string:
		return "and" if operator == BinaryOperatorType.ConditionalAnd
		return "or" if operator == BinaryOperatorType.ConditionalOr
		return "==" if operator == BinaryOperatorType.Equality
		return "!=" if operator == BinaryOperatorType.InEquality
		return "<" if operator == BinaryOperatorType.LessThan
		return ">" if operator == BinaryOperatorType.GreaterThan
		return "<=" if operator == BinaryOperatorType.LessThanOrEqual
		return ">=" if operator == BinaryOperatorType.GreaterThanOrEqual
		return "+" if operator == BinaryOperatorType.Add
		return "-" if operator == BinaryOperatorType.Subtract
		return "*" if operator == BinaryOperatorType.Multiply
		return "/" if operator == BinaryOperatorType.Divide
		return "%" if operator == BinaryOperatorType.Modulus
		return "&" if operator == BinaryOperatorType.BitwiseAnd
		return "|" if operator == BinaryOperatorType.BitwiseOr
		return "^" if operator == BinaryOperatorType.ExclusiveOr
		return "<<" if operator == BinaryOperatorType.ShiftLeft
		return ">>" if operator == BinaryOperatorType.ShiftRight
		return null

	private static def Suffix(operator as UnaryOperatorType) as string:
	"""Increment and decrement, which Boo writes the same as C#."""
		return "++" if operator == UnaryOperatorType.PostIncrement or operator == UnaryOperatorType.Increment
		return "--" if operator == UnaryOperatorType.PostDecrement or operator == UnaryOperatorType.Decrement
		return null

	private static def Prefix(operator as UnaryOperatorType) as string:
		return "not " if operator == UnaryOperatorType.Not
		return "-" if operator == UnaryOperatorType.Minus
		return "" if operator == UnaryOperatorType.Plus
		return "~" if operator == UnaryOperatorType.BitNot
		return null

	private static def Unwritten(node as AstNode, indent as string) as string:
		return "${indent}${Unwritten(node)}\n"

	private static def Unwritten(node as AstNode) as string:
	"""A "# TODO" comment holding the C# that has no Boo equivalent."""
		text = node.ToString().Replace("\r", " ").Replace("\n", " ").Trim()
		text = text.Substring(0, 90) + "..." if text.Length > 90
		return "# TODO: ${text}"
