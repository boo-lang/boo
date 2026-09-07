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

import System.CommandLine

class CommandLineOptions:
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

	static final VersionNames = ("--version", "-version")
	static final HelpNames = ("--help", "-help", "-h")

	public final Action as int
	public final UnknownOption as string

	def constructor(action as int, unknownOption as string):
		Action = action
		UnknownOption = unknownOption

	private static def Parser() as Command:
	"""A RootCommand would answer for --help and --version itself."""
		command = Command("boo-ls", "Language server for Boo, spoken over stdio.")
		command.Add(Option[of bool]("--stdio", "-stdio"))
		command.Add(Option[of bool]("--version", "-version"))
		command.Add(Option[of bool]("--help", "-help", "-h"))
		return command

	static def Parse(args as (string)) as CommandLineOptions:
		parsed = Parser().Parse(args)
		for error in parsed.Errors:
			return CommandLineOptions(Unknown, Offending(parsed, args))

		# Whichever was written first is what was asked for.
		for arg in args:
			return CommandLineOptions(ShowVersion, null) if arg in VersionNames
			return CommandLineOptions(ShowHelp, null) if arg in HelpNames
		return CommandLineOptions(Serve, null)

	private static def Offending(parsed as ParseResult, args as (string)) as string:
	"""The argument the parser could not place, as the caller wrote it."""
		for token in parsed.UnmatchedTokens:
			return token
		for arg in args:
			return arg unless arg in VersionNames or arg in HelpNames or arg == "--stdio" or arg == "-stdio"
		return null
