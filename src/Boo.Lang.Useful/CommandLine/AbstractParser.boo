#region license
// Copyright (c) 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Useful.CommandLine

import System

callable ArgumentHandler(argument as string)

callable OptionHandler(name as string, value as string)

class CommandLineException(ApplicationException):
	def constructor(message as string):
		super(message)
		
class MalformedOptionException(CommandLineException):
	def constructor(message as string):
		super(message)

class AbstractParser:

	[Getter(Empty)]
	private _empty as bool

	virtual def Parse(args as (string)):
		if args.Length == 0:
			_empty = true
		else: 
			for arg in args:
				ParseArg arg
			
	def ParseArg(arg as string):
		if arg.StartsWith("@"):
			ParseResponseFile(arg[1:])
		elif IsOption(arg):
			ParseOption(arg)
		else:
			OnArgument(arg)
			
	def ParseResponseFile(responseFile as string):
		for line in System.IO.File.ReadAllLines(responseFile):
			arg = line.Trim()
			if len(arg) == 0 or IsComment(arg):
				continue
			ParseArg(arg)
			
	def IsComment(arg as string):
		return arg.StartsWith("#")
				
	abstract protected def OnArgument(argument as string):
		pass
		
	abstract protected def OnOption(name as string, value as string):
		pass
				
	virtual def ParseOption(arg as string):
		name, value = SplitOption(arg)
		OnOption(name, value)
				
	def IsOption(arg as string):
		return /^-{1,2}\w/.IsMatch(arg)
		
	def SplitOption(arg as string):
		m = /^(-{1,2})(?<name>(\w|-)*\w)(?<value>((-|\+)|([:=].+)))?$/.Match(arg)
		raise MalformedOptionException(arg) unless m.Success
		name = m.Groups["name"].Value
		value as string
		if m.Groups["value"].Success:
			value = m.Groups["value"].Value
			if value.StartsWith(":") or value.StartsWith('='): 
				value = value[1:]
		return name, value
