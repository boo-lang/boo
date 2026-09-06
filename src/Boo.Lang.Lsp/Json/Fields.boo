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

namespace Boo.Lang.Lsp.Json

import System
import System.Collections.Generic

class Fields:
"""
Reads values out of a parsed message.

A client may leave out anything the protocol marks optional, so every reader
answers with a default rather than raising when a field is missing or is not
of the type asked for.
"""

	static def Map(value as object, name as string) as Dictionary[of string, object]:
		return Of(value, name) as Dictionary[of string, object]

	static def Items(value as object, name as string) as List[of object]:
		found = Of(value, name) as List[of object]
		return List[of object]() if found is null
		return found

	static def Text(value as object, name as string) as string:
		return Of(value, name) as string

	static def Number(value as object, name as string, fallback as int) as int:
		found = Of(value, name)
		return fallback if found is null
		return System.Convert.ToInt32(found)

	static def Of(value as object, name as string) as object:
		map = value as IDictionary[of string, object]
		return null if map is null
		found as object
		return null unless map.TryGetValue(name, found)
		return found
