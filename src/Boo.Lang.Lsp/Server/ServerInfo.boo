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

import System
import System.IO

class ServerInfo:
"""Identifies this server to a client during initialize."""

	public static final Name = "boo-ls"

	static Version as string:
		get:
			return typeof(ServerInfo).Assembly.GetName().Version.ToString(3)

	static Build as string:
	"""
	Which executable is answering and when its code was built.

	The version alone does not say: several checkouts of the same version
	can serve one editor, and a server goes on running the build it started
	from however many times the tree is rebuilt under it.
	"""
		get:
			running = Environment.ProcessPath
			running = typeof(ServerInfo).Assembly.Location if string.IsNullOrEmpty(running)
			return "location unknown" if string.IsNullOrEmpty(running)
			return "${running}, code built ${Written()}"

	private static def Written() as string:
		try:
			location = typeof(ServerInfo).Assembly.Location
			return "unknown" if string.IsNullOrEmpty(location)
			return File.GetLastWriteTime(location).ToString("yyyy-MM-dd HH:mm:ss")
		except:
			return "unknown"

	static Banner as string:
	"""One line naming this build, for the log a client shows."""
		get:
			return "${Name} ${Version} (${Build})"
