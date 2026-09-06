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
		# the compile ran in.
		ActiveEnvironment.With(context.Environment) do:
			found = Describe(document, Find(document, context, position))
		return found

	private static def Find(document as TextDocument, context as CompilerContext, position as Position) as Expression:
		finder = Finder(document, position)
		finder.Visit(context.CompileUnit)
		return finder.Found

	private static def Describe(document as TextDocument, node as Expression) as Result:
		return null if node is null

		entity = node.Entity
		return null if entity is null

		name = NameOf(node)
		start = Positions.FromLexicalInfo(document, node.LexicalInfo)

		result = Result(
			Name: name,
			Signature: SignatureOf(name, entity),
			Start: start,
			End: Position(start.Line, start.Character + name.Length))

		declared = entity as IInternalEntity
		if declared is not null and declared.Node is not null:
			location = declared.Node.LexicalInfo
			if location.Line > 0:
				result.DeclarationUri = location.FileName
				result.Declaration = Positions.FromSourceLocation(location)

		return result

	private static def SignatureOf(name as string, entity as IEntity) as string:
		method = entity as IMethod
		return "def ${name}(${Parameters(method)}) as ${method.ReturnType}" if method is not null

		type = entity as IType
		return "${KindOf(type)} ${type}" if type is not null

		typed = entity as ITypedEntity
		return "${name} as ${typed.Type}" if typed is not null

		return "namespace ${name}" if entity.EntityType == EntityType.Namespace
		return name

	private static def Parameters(method as IMethod) as string:
		written = List[of string]()
		for parameter in method.GetParameters():
			written.Add("${parameter.Name} as ${parameter.Type}")
		return string.Join(", ", written.ToArray())

	private static def KindOf(type as IType) as string:
		return "interface" if type.IsInterface
		return "enum" if type.IsEnum
		return "struct" if type.IsValueType
		return "class"

	private static def NameOf(node as Expression) as string:
		reference = node as ReferenceExpression
		return reference.Name if reference is not null
		return ""

	private class Finder(DepthFirstVisitor):
	"""
	Picks the innermost name written at the position.

	The compiler moves and synthesises nodes, and those carry the position of
	whatever statement they came from, so a candidate only counts when the
	name is really in the text where the node claims to be.
	"""

		_document as TextDocument
		_position as Position

		[getter(Found)]
		_found as Expression

		def constructor(document as TextDocument, position as Position):
			_document = document
			_position = position

		override def OnReferenceExpression(node as ReferenceExpression):
			Consider(node)

		override def OnMemberReferenceExpression(node as MemberReferenceExpression):
			super(node)
			Consider(node)

		private def Consider(node as ReferenceExpression):
			return unless Covers(node)
			# A later start is a more specific name: g.Hello beats g.
			_found = node if _found is null or node.LexicalInfo.Column >= _found.LexicalInfo.Column

		private def Covers(node as ReferenceExpression) as bool:
			location = node.LexicalInfo
			return false unless location.Line > 0 and location.Column > 0
			start = Positions.FromLexicalInfo(_document, location)
			return false unless start.Line == _position.Line
			return false unless WrittenHere(node.Name, start)
			return _position.Character >= start.Character and _position.Character < start.Character + node.Name.Length

		private def WrittenHere(name as string, start as Position) as bool:
			line = _document.LineText(start.Line)
			return false if string.IsNullOrEmpty(name)
			return false if start.Character + name.Length > line.Length
			return line.Substring(start.Character, name.Length) == name
