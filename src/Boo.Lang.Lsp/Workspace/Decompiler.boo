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
import System.IO
import System.Reflection
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Reflection
import Boo.Lang.Decompiler

class Decompiler:
"""
Where something that lives in an assembly was written.

A member of a referenced assembly has no syntax tree to point at, so the
type it belongs to is decompiled to C# once, written under the cache
directory, and go to definition answers with a place in that file.

Decompiling a type costs enough that it is worth doing once. The file on
disk is the cache: it is named after the assembly and the type, so a second
request for anything in the same type reads it back instead.
"""

	class Source:
		public Uri as string
		public Line as int

	# Stamped with this build: what a server writes out is only as good as
	# the version that wrote it, and a stale file would outlive the fix.
	public static final Boo = "boo"
	public static final CSharp = "csharp"

	# Which language go to definition shows a type an assembly owns in.
	# C# is what the decompiler reads back most faithfully.
	[property(Language)]
	static _language = CSharp

	static Cache = Path.Combine(Path.GetTempPath(), "boo-ls", "metadata", Build())

	private static def Build() as string:
		try:
			location = typeof(Decompiler).Assembly.Location
			return "0" if string.IsNullOrEmpty(location)
			return File.GetLastWriteTimeUtc(location).Ticks.ToString()
		except:
			return "0"

	static def Of(entity as IEntity) as Source:
	"""Where the entity is written, or null if it is not in an assembly."""
		member = MemberOf(entity)
		return null if member is null

		declaring = member as Type
		declaring = member.DeclaringType if declaring is null
		return null if declaring is null

		path = SourceFile(declaring, DeclaringTypeOf(entity))
		return null if path is null
		return Source(Uri: Project.UriOf(path), Line: LineOf(path, member))

	private static def DeclaringTypeOf(entity as IEntity) as IType:
	"""The type this entity belongs to, for the fallback to write."""
		type = entity as IType
		return type if type is not null
		owned = entity as IMember
		return (owned.DeclaringType if owned is not null else null)

	private static def MemberOf(entity as IEntity) as MemberInfo:
	"""
	The reflection handle behind an entity, or null if it has none.

	A type keeps its handle under its own name; everything else reaches it
	through the interface the external entities share.
	"""
		return null if entity is null
		type = entity as ExternalType
		return type.ActualType if type is not null
		external = entity as IExternalEntity
		return external.MemberInfo if external is not null
		return null

	private static def SourceFile(type as Type, declared as IType) as string:
	"""The cached file for a type, writing it the first time."""
		definition = type
		definition = type.GetGenericTypeDefinition() if type.IsConstructedGenericType
		assembly = definition.Assembly.Location
		return null if string.IsNullOrEmpty(assembly)

		# Kept apart by language, so a change of mind does not read a file
		# written for the other one.
		target = Path.Combine(Cache, _language, Path.GetFileNameWithoutExtension(assembly), FileName(definition))
		return target if File.Exists(target)

		# Fall back to the type system if the decompiler fails or is absent.
		text as string
		try:
			text = (Decompiled.AsCSharp(assembly, definition.FullName) if _language == CSharp else Decompiled.Of(assembly, definition.FullName))
		except e as Exception:
			Console.Error.WriteLine("boo-ls: reading ${definition.FullName} failed: ${e.Message}")
		text = Stub.Of(declared) if string.IsNullOrEmpty(text) and _language != CSharp
		return null if string.IsNullOrEmpty(text)
		Directory.CreateDirectory(Path.GetDirectoryName(target))
		File.WriteAllText(target, text)
		return target

	private static def FileName(type as Type) as string:
	"""
	A file name for a type, close enough to its own to read well in a tab.

	A nested type is written with a plus and a generic one with a backtick,
	and neither belongs in a file name.
	"""
		name = type.FullName.Replace("+", ".")
		tick = name.IndexOf(char('`'))
		name = name.Substring(0, tick) if tick >= 0
		return name + (".cs" if _language == CSharp else ".boo")

	private static def LineOf(path as string, member as MemberInfo) as int:
	"""
	The line the member is written on, or the first line of the file.

	The decompiler does not report where it put anything, so the name is
	looked for in the text. A member is declared before it is used, so the
	first line naming it is the declaration.
	"""
		name = member.Name
		lines = File.ReadAllLines(path)
		for i in range(lines.Length):
			return i if Declares(lines[i], name)
		return 0

	private static def Declares(line as string, name as string) as bool:
		at = line.IndexOf(name)
		while at >= 0:
			return true if Whole(line, at, name.Length)
			at = line.IndexOf(name, at + 1)
		return false

	private static def Whole(line as string, at as int, length as int) as bool:
		return false if at > 0 and (char.IsLetterOrDigit(line[at - 1]) or line[at - 1] == char('_'))
		after = at + length
		return true if after >= line.Length
		return not (char.IsLetterOrDigit(line[after]) or line[after] == char('_'))
