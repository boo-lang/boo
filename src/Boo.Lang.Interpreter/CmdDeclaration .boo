#region license
// Copyright (c) 2013 Harald Meyer auf'm Hofe (harald_meyer@users.sourceforge.net)
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

namespace Boo.Lang.Interpreter

import System

public class CmdClassAttribute(Attribute):
"""
A class containing builtin commands. blocks of commands
can be activated and deactivated. Classes with this attribute
shall either be static or the activator shall be able to create
instances. Within these classes, the shell will scan for
<BuiltinDeclarationAttribute>.
"""
	public def constructor(name as string):
		self._name = name
	
	[Getter(Name)]
	_name as string
	"""The name of this block of commands."""
	

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class CmdDeclarationAttribute(Attribute):
"""Declares builtin shell commands.
Builtins have a name and an optional description that shall be
displayed to the user whenever appropriate.
"""	
	public def constructor(nameAndShortcuts as string):
		self._shortcuts = nameAndShortcuts.Split(' '.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
		self._name = self._shortcuts[0]
		self._shortcuts = self._shortcuts[1:]
	
	[Getter(Name)]
	_name as string
	"""Primary name of the command."""
	
	[Getter(Shortcuts)]
	_shortcuts as (string)
	"""The name of the command that shall be used to start it in a shell."""
	
	[Property(Description)]
	_description as string
	"""The description on the command that shall be displayed to the user whenever appropriate."""

public enum CmdArgumentCompletion:
"""
Options of the <CommandArgumentAttribute> defining a kind of
argument completion.
"""
	None = 0x000
	"""
	Argument completion is not supported.
	"""
		
	MaskPathName = 0x8000
	"""
	If this bit is set, the argument represents a file or a path name.
	"""
		
	Directory = 0x0a000
	"""
	The argument is the name of a directory  as string.
	"""
		
	File = 0x0C000
	"""
	The argument is the name of a file as string. The <Method> property
	may contain a suffix (like .boo).
	"""
	
	ExistingOrNotExistingFileOrExistingDirectory = 0x0e001
	"""
	A target destination for copy and move (and maybe others). The <Method> property
	may contain a suffix (like .boo). This suffix will however only applied to existing
	files.
	"""
	
	Type = 0x002
	"""
	the argument is a type name.
	"""
	
	TypeOrMember = 0x004
	"""
	The argument is a type or a method (used by "help").
	"""

[AttributeUsage(AttributeTargets.Parameter)]
public class CmdArgumentAttribute(Attribute):
"""
An optional 
"""
	[Getter(Type)]
	_type as CmdArgumentCompletion

	public property DefaultValue as string
	"""
	This is an optional property and will be passed to the 
	CompletionMethod.
	"""
	
	public def constructor(type as CmdArgumentCompletion):
		self._type = type
		
