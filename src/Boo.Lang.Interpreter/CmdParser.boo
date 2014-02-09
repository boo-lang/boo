#region license
// Copyright (c) 2013 by Harald Meyer auf'm Hofe
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
import System.Collections

class CmdParser:
"""
Parses a line that might be a builtin command (without
preceeding slash).
This class will isolate the command and try its best to create
an array of arguments, each represented by a string.
You may use quotes or double quotes to compose arguments of more
than one word. You may also use brackets, square brackets, or curly
brackets to embrace an argument. The difference between to approaches:
"an "arg" will be parsed into two arguments "an " and "arg\"".
"this is exactly "" one arg" will be parsed into "this is exactly \" one arg". 
(an (expr) containing [additional] expressions) will be parsed into
exactly one argument "an (expr) containing [additional] expressions".
Brackets and quotes will be removed from the resulting argument string,
if the parser suspects that these characters have been used to quote
the argument.
"""
	public def constructor(line as string):
		self._line = line.Trim()
		if self._line.StartsWith("/"):
			self._line=self._line[1:] // skip slash that has been used to identify this line as a builtin tool
		pos = self._line.IndexOf(' ')
		if pos > 0:
			self._Cmd = self._line[0:pos].TrimStart()
			self._Arguments = self._line[pos:].Trim()
			args=Generic.List[of string]()
			arg=ScanArg(pos)
			while not arg.IsEmpty:
				args.Add(arg.Arg)
				arg=ScanArg(arg.EndPos+1)
			self._Args=args.ToArray()
		else:
			self._Cmd = self._line
			self._Args=array(string,0)
	
	static def ScanCmd(line as string):
		line = line.TrimStart()
		if line.StartsWith("/"):
			line=line[1:] // skip slash that has been used to identify this line as a builtin tool
		pos = line.IndexOf(' ')
		if pos > 0:
			return line[0:pos].TrimStart()
		return null
	
	[Property(Line)]
	_line as string
	
	[Property(Arguments)]
	_Arguments as string
	
	def SetOnlyOneArgument():
	"""
	The caller recognized after analysing [_Cmd] that
	this method only has one argument. Thus, all arguments
	shall be joined into one.
	"""
		self._Args = (self._Arguments,) if self._Args.Length > 0
	
	struct ArgDescr:
		public Arg as string
		public EndPos as int 
		public IsEmpty:
			get: return string.IsNullOrEmpty(self.Arg)
	
	def ScanArg(pos as int):
		# skip introducing blanks
		while pos < self._line.Length and self._line[pos] == ' '[0]:
			pos+=1
		result = ArgDescr()
		if pos >= self._line.Length:
			return result
		result.Arg=string.Empty;
		result.EndPos = pos
		bracketCounter=1
		argDelimiter = " "
		openingBracket=string.Empty
		useQuote = false
		currentChar=self._line[result.EndPos:result.EndPos+1]
		if "(".Equals(currentChar):
			openingBracket=currentChar
			argDelimiter = ")"
		elif "{".Equals(currentChar):
			openingBracket=currentChar
			argDelimiter = "}"
		elif "[".Equals(currentChar):
			openingBracket=currentChar
			argDelimiter = "]"
		elif "\"".Equals(currentChar) or "'".Equals(currentChar):
			useQuote = true
			argDelimiter = currentChar
		else:
			result.Arg+=currentChar
		result.EndPos += 1
		currentChar=self._line[result.EndPos:result.EndPos+1]
		while result.EndPos < self._line.Length:
			if openingBracket.Equals(currentChar):
				bracketCounter+=1
			elif argDelimiter.Equals(currentChar):
				bracketCounter -= 1 if bracketCounter > 0
			break if bracketCounter == 0			
			result.Arg+=currentChar
			result.EndPos += 1
			currentChar=self._line[result.EndPos:result.EndPos+1]
		return result
	
	[Getter(Cmd)]
	_Cmd as string
	"""Name of the parsed builtin command."""
	
	[Getter(Args)]
	_Args as (string)
	"""The arguments that have been found."""


