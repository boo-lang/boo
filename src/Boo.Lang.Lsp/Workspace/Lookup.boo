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

namespace Boo.Lang.Lsp.Workspace

import System
import System.Collections.Generic
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Environments

class Lookup:
"""
Says what the cursor is on, in plain values.

An entity can only be read inside the environment the compile ran in, so
everything the caller needs is turned into strings and positions here rather
than handed out as entities.
"""

	class Result:
		public Name as string
		public Signature as string
		public Documentation as string
		public Start as Position
		public End as Position
		public Declaration as Position
		public DeclarationUri as string

		HasDeclaration as bool:
			get: return Declaration is not null

	static def At(document as TextDocument, context as CompilerContext, position as Position) as Result:
		return null if context is null

		found as Result
		# Reading a type or a declaration off an entity needs the environment
		# the compile ran in, and resolves lazily through the same shared state
		# a compile uses, so it waits its turn like one.
		lock CompilerLock.Gate:
			ActiveEnvironment.With(context.Environment) do:
				finder = Finder(document, position)
				finder.Visit(context.CompileUnit)
				found = Describe(document, finder.Found, finder.FoundName)
		return found

	private static def ValueTypeMade(node as MethodInvocationExpression) as IType:
	"""
	The value type an eval invocation stands for, or null.

	A value type built with no arguments is replaced by an eval over a temp
	local, so the invocation carries the type but names nothing.
	"""
		target = node.Target as ReferenceExpression
		return null if target is null
		builtin = target.Entity as BuiltinFunction
		return null if builtin is null or builtin.FunctionType != BuiltinFunctionType.Eval
		return node.ExpressionType

	private static def NameOnLine(document as TextDocument, node as Node, name as string) as Position:
	"""
	Where a name is written on a node's line, or null if it is not on it.

	The eval a value type construction leaves behind sits past the name it
	was made from, so the column it carries is no place to start looking.
	"""
		return null if string.IsNullOrEmpty(name)
		location = node.LexicalInfo
		return null unless location.Line > 0
		start = Positions.FromLexicalInfo(document, location)
		line = document.LineText(start.Line)
		at = line.IndexOf(name, StringComparison.Ordinal)
		return null if at < 0
		return Position(start.Line, at)

	private static def Describe(document as TextDocument, node as Node, name as string) as Result:
		return null if node is null

		entity = node.Entity
		if entity is null:
			invocation = node as MethodInvocationExpression
			entity = ValueTypeMade(invocation) if invocation is not null
		return null if entity is null

		start = Positions.FromLexicalInfo(document, node.LexicalInfo)

		result = Result(
			Name: name,
			Signature: Signatures.Of(name, entity),
			Start: start,
			End: Position(start.Line, start.Character + name.Length))

		declared = entity as IInternalEntity
		if declared is not null and declared.Node is not null:
			result.Documentation = DocumentationOf(declared.Node)
			location = declared.Node.LexicalInfo
			if location.Line > 0:
				result.DeclarationUri = Project.UriOf(location.FileName)
				result.Declaration = Positions.FromSourceLocation(location)
		else:
			# Nothing in the project declares it, so what an assembly holds
			# is the only source there is to point at.
			source = Decompiler.Of(entity)
			if source is not null:
				result.DeclarationUri = source.Uri
				result.Declaration = Position(source.Line, 0)

		return result

	private static def DocumentationOf(node as Node) as string:
	"""
	What a declaration documents, or null if it documents nothing.

	Writing a type's name to construct it binds to the constructor rather
	than to the type, and the write up is almost always on the type, so an
	undocumented constructor answers with what encloses it.
	"""
		return null if node is null
		text = node.Documentation
		return Dedent(text) if not string.IsNullOrEmpty(text)
		return DocumentationOf(node.ParentNode) if node isa Constructor
		return null

	private static def Dedent(text as string) as string:
	"""
	Documentation without the indentation it was written at.

	A doc string is indented to match the declaration it belongs to, and
	markdown reads an indented line as preformatted text, so what the author
	indented for the file has to come off before anyone renders it. Only the
	shared margin goes: indentation past it is the author's own layout.
	"""
		lines = text.Replace("\r\n", "\n").Split(char('\n'))
		margin = -1
		for line in lines:
			continue if line.Trim().Length == 0
			indent = line.Length - line.TrimStart().Length
			margin = indent if margin < 0 or indent < margin
		return text.Trim() if margin <= 0
		stripped = List[of string]()
		for line in lines:
			stripped.Add(("" if line.Trim().Length == 0 else line.Substring(margin)))
		return string.Join("\n", stripped.ToArray()).Trim()

	private class Finder(DepthFirstVisitor):
	"""
	Picks the innermost name written at the position.

	The compiler moves and synthesises nodes, and those carry the position of
	whatever statement they came from, so a candidate only counts when the
	name is really in the text where the node claims to be.

	A name is worth stopping on where it is declared as much as where it is
	used, and a declaration is not an expression, so both kinds are visited.
	"""

		_document as TextDocument
		_position as Position

		[getter(Found)]
		_found as Node

		[getter(FoundName)]
		_foundName as string

		def constructor(document as TextDocument, position as Position):
			_document = document
			_position = position

		override def OnReferenceExpression(node as ReferenceExpression):
			Consider(node, node.Name)

		override def OnMemberReferenceExpression(node as MemberReferenceExpression):
			super(node)
			Consider(node, node.Name)

		override def OnMethodInvocationExpression(node as MethodInvocationExpression):
		"""
		A value type built with no arguments leaves nothing where it was written.

		The invocation is replaced by an eval over a temp local, which carries
		neither the type's entity nor its column, so the name is looked for
		along the line instead.
		"""
			super(node)
			made = ValueTypeMade(node)
			return if made is null
			ConsiderOnLine(node, made.Name)

		private def ConsiderOnLine(node as Node, name as string):
			start = NameOnLine(_document, node, name)
			return if start is null
			return unless start.Line == _position.Line
			return unless _position.Character >= start.Character and _position.Character <= start.Character + name.Length
			_found = node
			_foundName = name

		override def OnAttribute(node as Boo.Lang.Compiler.Ast.Attribute):
		"""
		An attribute names a type with no reference expression to carry it,
		and the binder rewrites the node to the full name of the type it
		resolved to, so the name to look for is whichever form was written.
		"""
			super(node)
			ConsiderNamed(node, node.Name)

		override def OnSimpleTypeReference(node as SimpleTypeReference):
		"""A type written as an annotation is named the same way."""
			super(node)
			ConsiderNamed(node, node.Name)

		private def ConsiderNamed(node as Node, name as string):
		"""Take whichever form of a rewritten name is the one in the text."""
			for candidate in NamesFor(name):
				break if Consider(node, candidate)

		private static def NamesFor(name as string) as List[of string]:
		"""
		The forms a rewritten name may be written in, most qualified first.

		System.IO.Path is written Path or in full, and an attribute drops
		the Attribute suffix as well. Most qualified first, so the longest
		form that is really in the text is the one taken.
		"""
			names = List[of string]()
			return names if string.IsNullOrEmpty(name)
			names.Add(name)
			cut = name.LastIndexOf(char('.'))
			bare = (name if cut < 0 else name.Substring(cut + 1))
			names.Add(bare) unless bare == name
			suffix = "Attribute"
			names.Add(bare.Substring(0, bare.Length - suffix.Length)) if bare.EndsWith(suffix)
			return names

		override def OnDeclaration(node as Declaration):
			super(node)
			Consider(node, node.Name)

		override def OnParameterDeclaration(node as ParameterDeclaration):
			super(node)
			Consider(node, node.Name)

		private def Consider(node as Node, name as string) as bool:
			return false unless Covers(node, name)
			# A later start is a more specific name: g.Hello beats g.
			if _found is null or node.LexicalInfo.Column >= _found.LexicalInfo.Column:
				_found = node
				# The name as written, which an attribute's node no longer holds.
				_foundName = name
			return true

		private def Covers(node as Node, name as string) as bool:
			location = node.LexicalInfo
			return false unless location.Line > 0 and location.Column > 0
			start = Positions.FromLexicalInfo(_document, location)
			return false unless start.Line == _position.Line
			return false unless WrittenHere(name, start)
			# The end counts: clicking a name leaves the caret after it, and
			# for a name of one character that is all the editor ever asks.
			return _position.Character >= start.Character and _position.Character <= start.Character + name.Length

		private def WrittenHere(name as string, start as Position) as bool:
			line = _document.LineText(start.Line)
			return false if string.IsNullOrEmpty(name)
			return false if start.Character + name.Length > line.Length
			return line.Substring(start.Character, name.Length) == name
