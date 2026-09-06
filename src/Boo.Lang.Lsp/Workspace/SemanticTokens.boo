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

class SemanticTokens:
"""
Classifies names in a bound document for LSP semantic highlighting.

The compiler AST is not lossless, so this intentionally classifies only names
that can be tied back to text in the document. Compiler-generated and moved
nodes are ignored by checking that the name is written where the node says it
is.
"""

	public static final Namespace = 0
	public static final Type = 1
	public static final Class = 2
	public static final Enum = 3
	public static final Interface = 4
	public static final Struct = 5
	public static final TypeParameter = 6
	public static final Parameter = 7
	public static final Variable = 8
	public static final Property = 9
	public static final EnumMember = 10
	public static final Event = 11
	public static final Function = 12
	public static final Method = 13
	public static final Macro = 14
	public static final Keyword = 15
	public static final Modifier = 16
	public static final Comment = 17
	public static final String = 18
	public static final Number = 19
	public static final Regexp = 20
	public static final Operator = 21

	static def Legend() as Dictionary[of string, object]:
		legend = Dictionary[of string, object]()
		legend["tokenTypes"] = (
			"namespace", "type", "class", "enum", "interface", "struct",
			"typeParameter", "parameter", "variable", "property", "enumMember",
			"event", "function", "method", "macro", "keyword", "modifier",
			"comment", "string", "number", "regexp", "operator")
		legend["tokenModifiers"] = (,)
		return legend

	static def Capability() as Dictionary[of string, object]:
		capability = Dictionary[of string, object]()
		capability["legend"] = Legend()
		capability["full"] = true
		capability["range"] = false
		return capability

	static def Of(document as TextDocument, context as CompilerContext) as Dictionary[of string, object]:
		result = Dictionary[of string, object]()
		result["data"] = Encoded(document, context)
		return result

	private static def Encoded(document as TextDocument, context as CompilerContext) as List[of long]:
		tokens = List[of Token]()
		return List[of long]() if context is null

		lock CompilerLock.Gate:
			ActiveEnvironment.With(context.Environment) do:
				collector = Collector(document)
				collector.Visit(context.CompileUnit)
				tokens = collector.Tokens

		tokens.Sort({ left as Token, right as Token | left.CompareTo(right) })
		return Encode(tokens)

	private static def Encode(tokens as List[of Token]) as List[of long]:
		data = List[of long]()
		line = 0
		character = 0
		for token in tokens:
			deltaLine = token.Line - line
			deltaStart = token.Character
			deltaStart = token.Character - character if deltaLine == 0
			data.Add(deltaLine)
			data.Add(deltaStart)
			data.Add(token.Length)
			data.Add(token.Kind)
			data.Add(0)
			line = token.Line
			character = token.Character
		return data

	private class Token(IComparable[of Token]):
		public final Line as int
		public final Character as int
		public final Length as int
		public final Kind as int

		def constructor(line as int, character as int, length as int, kind as int):
			Line = line
			Character = character
			Length = length
			Kind = kind

		def CompareTo(other as Token) as int:
			result = Line.CompareTo(other.Line)
			return result if result != 0
			result = Character.CompareTo(other.Character)
			return result if result != 0
			return Length.CompareTo(other.Length)

	private class Collector(DepthFirstVisitor):
		_document as TextDocument
		_tokens = List[of Token]()
		_seen = HashSet[of string]()

		def constructor(document as TextDocument):
			_document = document

		Tokens as List[of Token]:
			get: return _tokens

		override def OnReferenceExpression(node as ReferenceExpression):
			Add(node, node.Name, KindOf(node.Entity))

		override def OnMemberReferenceExpression(node as MemberReferenceExpression):
			super(node)
			Add(node, node.Name, KindOf(node.Entity))

		override def OnSimpleTypeReference(node as SimpleTypeReference):
			Add(node, node.Name, KindOf(node.Entity))

		override def OnGenericParameterDeclaration(node as GenericParameterDeclaration):
			Add(node, node.Name, TypeParameter)

		override def OnDeclaration(node as Declaration):
			super(node)
			Add(node, node.Name, KindOf(node.Entity))

		override def OnParameterDeclaration(node as ParameterDeclaration):
			super(node)
			Add(node, node.Name, Parameter)

		override def OnClassDefinition(node as ClassDefinition):
			super(node)
			AddDeclarationName(node, node.Name, Class)

		override def OnStructDefinition(node as StructDefinition):
			super(node)
			AddDeclarationName(node, node.Name, Struct)

		override def OnInterfaceDefinition(node as InterfaceDefinition):
			super(node)
			AddDeclarationName(node, node.Name, Interface)

		override def OnEnumDefinition(node as EnumDefinition):
			super(node)
			AddDeclarationName(node, node.Name, Enum)

		override def OnEnumMember(node as EnumMember):
			super(node)
			AddDeclarationName(node, node.Name, EnumMember)

		override def OnMethod(node as Method):
			super(node)
			AddDeclarationName(node, node.Name, Method)

		override def OnField(node as Field):
			super(node)
			AddDeclarationName(node, node.Name, Variable)

		override def OnProperty(node as Property):
			super(node)
			AddDeclarationName(node, node.Name, Property)

		override def OnEvent(node as Event):
			super(node)
			AddDeclarationName(node, node.Name, Event)

		private def Add(node as Node, name as string, kind as int):
			return if kind < 0
			return if string.IsNullOrEmpty(name)
			location = node.LexicalInfo
			return unless location.Line > 0 and location.Column > 0
			start = Positions.FromLexicalInfo(_document, location)
			return unless WrittenHere(name, start)
			Add(start.Line, start.Character, name.Length, kind)

		private def AddDeclarationName(node as Node, name as string, kind as int):
			return if string.IsNullOrEmpty(name)
			location = node.LexicalInfo
			return unless location.Line > 0
			start = Positions.FromLexicalInfo(_document, location)
			line = _document.LineText(start.Line)
			at = line.IndexOf(name, Math.Min(start.Character, line.Length), StringComparison.Ordinal)
			return if at < 0
			Add(start.Line, at, name.Length, kind)

		private def Add(line as int, character as int, length as int, kind as int):
			key = "${line}:${character}:${length}"
			return unless _seen.Add(key)
			_tokens.Add(Token(line, character, length, kind))

		private def WrittenHere(name as string, start as Position) as bool:
			line = _document.LineText(start.Line)
			return false if start.Character < 0
			return false if start.Character + name.Length > line.Length
			return line.Substring(start.Character, name.Length) == name

		private static def KindOf(entity as IEntity) as int:
			return -1 if entity is null
			kind = entity.EntityType
			return Namespace if kind == EntityType.Namespace
			return Method if kind == EntityType.Method or kind == EntityType.Constructor
			return Property if kind == EntityType.Property
			return Event if kind == EntityType.Event
			return Parameter if kind == EntityType.Parameter
			return Variable if kind == EntityType.Local or kind == EntityType.Field
			return TypeParameter if kind == EntityType.GenericParameter
			return TypeKindOf(entity) if kind == EntityType.Type or kind == EntityType.Array
			return Function if kind == EntityType.BuiltinFunction
			return -1

		private static def TypeKindOf(entity as IEntity) as int:
			type = entity as IType
			return Type if type is null
			return Interface if type.IsInterface
			return Enum if type.IsEnum
			return Struct if type.IsValueType
			return Class
