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

import System
import Boo.Lang.Lsp.Protocol
import Boo.Lang.Lsp.Server

def Usage():
	print "usage: boo-ls [--stdio] [--version] [--help]"
	print ""
	print "Speaks the Language Server Protocol over stdin and stdout."
	print "Started by an editor, not usually by hand."

def Serve() as int:
	# Named on the way in, so a log says which build answered it.
	Console.Error.WriteLine("boo-ls: serving as ${ServerInfo.Banner}")
	# Console.In and Console.Out would decode; the protocol is framed in bytes.
	stream = MessageStream(Console.OpenStandardInput(), Console.OpenStandardOutput())
	return LanguageServer(stream).Run()

def Run(args as (string)) as int:
	parsed = CommandLineOptions.Parse(args)

	if parsed.Action == CommandLineOptions.ShowVersion:
		print "${ServerInfo.Name} ${ServerInfo.Version}"
		return 0

	if parsed.Action == CommandLineOptions.ShowHelp:
		Usage()
		return 0

	if parsed.Action == CommandLineOptions.Unknown:
		Console.Error.WriteLine("boo-ls: unknown option ${parsed.UnknownOption}")
		return 2

	return Serve()

Environment.ExitCode = Run(argv)
