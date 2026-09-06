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

import System.Collections.Generic
import Boo.Lang.Compiler.TypeSystem

class Signatures:
"""How an entity is written for a person to read."""

	static def Of(name as string, entity as IEntity) as string:
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
