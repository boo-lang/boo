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

namespace Boo.Lang.Lsp.Server

class CommandLine:
"""
What the arguments asked for.

--stdio is not an option so much as a fact: it is what every client built on
vscode-languageclient appends, and stdio is the only transport this server
speaks, so it is accepted and ignored. Refusing it means exiting before the
client has been answered, which the client reports as a crash.
"""

	public static final Serve = 0
	public static final ShowVersion = 1
	public static final ShowHelp = 2
	public static final Unknown = 3

	public final Action as int
	public final UnknownOption as string

	def constructor(action as int, unknownOption as string):
		Action = action
		UnknownOption = unknownOption

	static def Parse(args as (string)) as CommandLine:
		for arg in args:
			continue if arg == "--stdio" or arg == "-stdio"
			return CommandLine(ShowVersion, null) if arg == "--version" or arg == "-version"
			return CommandLine(ShowHelp, null) if arg == "--help" or arg == "-help" or arg == "-h"
			return CommandLine(Unknown, arg)
		return CommandLine(Serve, null)
