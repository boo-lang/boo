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
import ICSharpCode.Decompiler
import ICSharpCode.Decompiler.CSharp
import ICSharpCode.Decompiler.CSharp.Syntax
import ICSharpCode.Decompiler.TypeSystem as Metadata

class Decompiled:
"""
Converts a type from a compiled assembly to Boo.

DecompileType returns a C# syntax tree rather than text, so the
declarations can be walked and rewritten in another language.
"""

	static def Of(assembly as string, fullName as string) as string:
		tree = TreeOf(assembly, fullName)
		return null if tree is null

		written = StringBuilder()
		for child in tree.Children:
			space = child as NamespaceDeclaration
			continue if space is null
			written.Append("namespace ").Append(space.Name).Append("\n")
			for member in space.Members:
				declared = member as TypeDeclaration
				written.Append("\n").Append(TypeOf(declared, "")) if declared is not null
		return null if written.Length == 0
		return written.ToString()

	static def AsCSharp(assembly as string, fullName as string) as string:
	"""The type as the decompiler writes it, for a reader who wants C#."""
		try:
			decompiler = CSharpDecompiler(assembly, DecompilerSettings())
			return decompiler.DecompileTypeAsString(Metadata.FullTypeName(fullName))
		except e as Exception:
			# A caller gets null and decides what to say about it.
			return null

	private static def TreeOf(assembly as string, fullName as string) as SyntaxTree:
		try:
			decompiler = CSharpDecompiler(assembly, DecompilerSettings())
			return decompiler.DecompileType(Metadata.FullTypeName(fullName))
		except e as Exception:
			# A caller gets null and decides what to say about it.
			return null

	private static def TypeOf(declared as TypeDeclaration, indent as string) as string:
		written = StringBuilder()
		written.Append(indent).Append(KindOf(declared)).Append(" ").Append(declared.Name)
		written.Append(TypeParametersOf(declared)).Append(":\n")

		body = StringBuilder()
		for member in declared.Members:
			line = MemberOf(member, indent + "\t")
			body.Append("\n").Append(line) unless string.IsNullOrEmpty(line)

		written.Append(("\n${indent}\tpass\n" if body.Length == 0 else body.ToString()))
		return written.ToString()

	private static def TypeParametersOf(declared as TypeDeclaration) as string:
	"""The [of T] a generic type needs, so its members can refer to T."""
		return "" if declared.TypeParameters.Count == 0
		names = System.Collections.Generic.List[of string]()
		for parameter as TypeParameterDeclaration in declared.TypeParameters:
			names.Add(parameter.Name)
		return "[of ${string.Join(', ', names.ToArray())}]"

	private static def KindOf(declared as TypeDeclaration) as string:
		return "interface" if declared.ClassType == ClassType.Interface
		return "enum" if declared.ClassType == ClassType.Enum
		return "struct" if declared.ClassType == ClassType.Struct
		return "class"

	private static def MemberOf(member as AstNode, indent as string) as string:
		return null unless IsPublic(member)

		nested = member as TypeDeclaration
		return TypeOf(nested, indent) if nested is not null

		made = member as ConstructorDeclaration
		if made is not null:
			return "${indent}def constructor(${ParametersOf(made.Parameters)}):\n${Bodies.Of(made.Body, indent + "\t")}"

		method = member as MethodDeclaration
		if method is not null:
			signature = "${indent}${Modifier(method)}def ${method.Name}(${ParametersOf(method.Parameters)})"
			return "${signature} as ${BooType(method.ReturnType)}:\n${Bodies.Of(method.Body, indent + "\t")}"

		property = member as PropertyDeclaration
		if property is not null:
			return "${indent}${Modifier(property)}${property.Name} as ${BooType(property.ReturnType)}:\n${indent}\tget:\n${indent}\t\tpass\n"

		field = member as FieldDeclaration
		return FieldOf(field, indent) if field is not null

		enumerated = member as EnumMemberDeclaration
		return "${indent}${enumerated.Name}\n" if enumerated is not null

		return null

	private static def FieldOf(field as FieldDeclaration, indent as string) as string:
	"""One line per field, whose names live on the declaration's variables."""
		written = StringBuilder()
		for variable as VariableInitializer in field.Variables:
			written.Append(indent).Append(Modifier(field)).Append(variable.Name)
			written.Append(" as ").Append(BooType(field.ReturnType)).Append("\n")
		return written.ToString()

	private static def IsPublic(member as AstNode) as bool:
		declared = member as EntityDeclaration
		return false if declared is null
		return (declared.Modifiers & Modifiers.Public) == Modifiers.Public

	private static def Modifier(declared as EntityDeclaration) as string:
		return ("static " if (declared.Modifiers & Modifiers.Static) == Modifiers.Static else "")

	private static def ParametersOf(parameters as AstNodeCollection[of ParameterDeclaration]) as string:
		written = System.Collections.Generic.List[of string]()
		for parameter in parameters:
			written.Add("${parameter.Name} as ${BooType(parameter.Type)}")
		return string.Join(", ", written.ToArray())

	internal static def BooType(type as AstType) as string:
	"""
	A C# type name as Boo writes it.

	The primitives are spelled the same; arrays and generics are not.
	"""
		return "object" if type is null or type.IsNull
		return BooName(type.ToString())

	private static def BooName(name as string) as string:
		return "object" if string.IsNullOrEmpty(name)

		if name.EndsWith("[]"):
			return "(${BooName(name.Substring(0, name.Length - 2))})"

		open = name.IndexOf(char('<'))
		if open > 0 and name.EndsWith(">"):
			inner = name.Substring(open + 1, name.Length - open - 2)
			return "${name.Substring(0, open)}[of ${BooName(inner)}]"

		return name.Replace("?", "")
