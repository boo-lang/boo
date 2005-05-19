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

class Option:
"""
Describes a command line option.
@see Parser
"""
	[property(ShortForm, value is not null and len(value) == 1)]
	_short as string
	
	[property(LongForm, value is not null and len(value) > 0)]
	_long as string
	
	[property(MinOccurs, value >= 0)]
	_minOccurs = 0
	
	[property(MaxOccurs, value > 0)]
	_maxOccurs = 1
	
	[property(Handler, value is not null)]
	_handler as ArgumentHandler
	
	[getter(Occurred)]
	_occurred = 0
	
	def PreValidate():
		assert ShortForm is not null or LongForm is not null
		assert Handler is not null
		
	def PostValidate():
		if _occurred < _minOccurs:
			raise CommandLineException("${_long or _short} must be specified at least ${_minOccurs} time(s)") 
	
	def Handle(value as string):
		++_occurred
		if _occurred > _maxOccurs:
			raise CommandLineException("${_long or _short} cannot be used more than ${_maxOccurs} time(s)")
		_handler(value)

class Parser(AbstractParser):
"""
Parses a command line based on a description of acceptable options.

@see Option
"""

	_options = {}
	_arguments = []
	
	Arguments:
		get:
			return array(string, _arguments)

	def AddOption([required] option as Option):
		option.PreValidate()
		TryToAddOption(option.ShortForm, option)
		TryToAddOption(option.LongForm, option)
		
	def GetOption([required] key as string) as Option:
		return _options[key]
		
	override def Parse(args as (string)):
		super(args)
		for option as Option in _options.Values:
			option.PostValidate()
		
	private def TryToAddOption(key as string, option as Option):
		return if key is null
		raise ArgumentException("'${key}' was specified more than once") if key in _options
		_options[key] = option
		
	override protected def OnOption(name as string, value as string):
		option = GetOption(name)
		if option is null:
			raise CommandLineException("'${name}' is not a valid option")
		option.Handle(value)
		
	override protected def OnArgument(value as string):
		_arguments.Add(value)