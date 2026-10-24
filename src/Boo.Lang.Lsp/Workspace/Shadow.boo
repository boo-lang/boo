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
import System.Diagnostics
import System.IO
import System.Security.Cryptography
import System.Text

class Shadow:
"""
Copies of build outputs, to load in place of the outputs themselves.

An assembly that has been loaded cannot be overwritten on Windows, and the
analyzer loads every reference a project has so that the compiler can read
its types. Loaded from where a build writes it, the reference would hold the
build's own output hostage until the editor was closed. A copy keeps the file
free; the copy is named for the file's size and time, so a rebuilt output is
picked up as a new one.

Only build outputs are copied. Packages and the installed frameworks are not
rewritten under anyone, and some of them are large.

The copies live in a directory of this process, and the directories of
processes that have gone are removed the next time one starts.
"""

	private static final Gate = object()
	private static _root as string

	static def For(path as string) as string:
	"""The path to load: a copy if the file is a build output, otherwise the file itself."""
		return path unless IsBuildOutput(path)
		try:
			info = FileInfo(path)
			return path unless info.Exists
			key = "${path.ToLowerInvariant()}|${info.LastWriteTimeUtc.Ticks}|${info.Length}"
			hash = System.Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(key))).Substring(0, 16)
			folder = Path.Combine(Root(), hash)
			copy = Path.Combine(folder, info.Name)
			return copy if File.Exists(copy)
			Directory.CreateDirectory(folder)
			# Whole or not at all: a copy cut short would load as a bad image.
			cut = copy + ".partial"
			File.Copy(path, cut, true)
			File.Move(cut, copy, true)
			return copy
		except e as Exception:
			# A build may be writing it, or the disk may be full. Loading it in
			# place is no worse than before.
			return path

	static def IsBuildOutput(path as string) as bool:
	"""Whether the file is under a bin or obj directory."""
		directory = Path.GetDirectoryName(path)
		while not string.IsNullOrEmpty(directory):
			name = Path.GetFileName(directory)
			return true if string.Equals(name, "bin", StringComparison.OrdinalIgnoreCase) or string.Equals(name, "obj", StringComparison.OrdinalIgnoreCase)
			directory = Path.GetDirectoryName(directory)
		return false

	private static def Root() as string:
		lock Gate:
			if _root is null:
				parent = Path.Combine(Path.GetTempPath(), "boo-ls")
				Directory.CreateDirectory(parent)
				Sweep(parent)
				_root = Path.Combine(parent, "p${Process.GetCurrentProcess().Id}")
				Directory.CreateDirectory(_root)
			return _root

	private static def Sweep(parent as string):
	"""Remove what processes that are no longer running left behind."""
		for directory in Directory.GetDirectories(parent, "p*"):
			id as int
			continue unless int.TryParse(Path.GetFileName(directory).Substring(1), id)
			continue if Running(id)
			try:
				Directory.Delete(directory, true)
			except:
				pass

	private static def Running(id as int) as bool:
		try:
			Process.GetProcessById(id).Dispose()
			return true
		except:
			return false
