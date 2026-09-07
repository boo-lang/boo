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
import System.Text
import Boo.Lang.Compiler.TypeSystem

class Stub:
"""
Writes a type from a referenced assembly as Boo, using the type system.

Signatures only, bodies as pass. This is the fallback for when the
decompiler cannot read a type, or is not installed.
"""

	static def Of(type as IType) as string:
		return null if type is null
		written = StringBuilder()

		space = NamespaceOf(type)
		written.Append("namespace ").Append(space).Append("\n\n") unless string.IsNullOrEmpty(space)

		written.Append(Signatures.KindOf(type)).Append(" ").Append(NameOf(type)).Append(":\n")
		body = Members(type)
		written.Append(("\n\tpass\n" if string.IsNullOrEmpty(body) else body))
		return written.ToString()

	private static def Members(type as IType) as string:
		written = StringBuilder()
		for entity in type.GetMembers():
			line = MemberOf(entity)
			written.Append("\n").Append(line) unless string.IsNullOrEmpty(line)
		return written.ToString()

	private static def MemberOf(entity as IEntity) as string:
		member = entity as IMember
		return null if member is null or not member.IsPublic or IsSpecial(member.Name)

		method = entity as IMethod
		if method is not null:
			return "\t${Modifier(member)}def ${member.Name}(${Parameters(method)}) as ${method.ReturnType}:\n\t\tpass\n"

		typed = entity as ITypedEntity
		return null if typed is null
		return "\t${Modifier(member)}${member.Name} as ${typed.Type}\n"

	private static def Modifier(member as IMember) as string:
		return ("static " if member.IsStatic else "")

	private static def Parameters(method as IMethod) as string:
		return string.Join(", ", Signatures.ParametersOf(method).ToArray())

	private static def IsSpecial(name as string) as bool:
	"""
	Whether the compiler generated this name rather than the author.

	A property is written as itself, so its get_ and set_ pair would say
	the same thing twice.
	"""
		for prefix in "get_", "set_", "add_", "remove_", "raise_", ".", "<", "op_":
			return true if name.StartsWith(prefix)
		return false

	private static def NamespaceOf(type as IType) as string:
		full = type.FullName
		return "" if string.IsNullOrEmpty(full)
		cut = full.LastIndexOf(char('.'))
		return "" if cut < 0
		return full.Substring(0, cut)

	private static def NameOf(type as IType) as string:
		name = type.Name
		# A generic type carries the count of its parameters in its name.
		tick = name.IndexOf(char('`'))
		return name if tick < 0
		return name.Substring(0, tick)
