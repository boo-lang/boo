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

class Parser(AbstractParser):
"""
Parses a command line based on a description of acceptable options.

@see Option
"""

	_options = {}
	
	event ArgumentFound as ArgumentHandler

	def AddOption([required] option as OptionAttribute, [required] handler as ArgumentHandler):
		option.PreValidate()
		TryToAddOption(option.ShortForm, option, handler)
		TryToAddOption(option.LongForm, option, handler)
		
	override def Parse(args as (string)):
		super(args)
		PostValidate()
		
	private def TryToAddOption(key as string, option as OptionAttribute, handler as ArgumentHandler):
		return if key is null
		raise ArgumentException("'${key}' was specified more than once") if key in _options
		_options[key] = (option, handler)
		
	private def PostValidate():
		for option as OptionAttribute, handler in _options.Values:
			option.PostValidate()

	override protected def OnOption(name as string, value as string):
		option = _options[name]
		InvalidOption(name) if option is null
			
		attribute as OptionAttribute, handler as ArgumentHandler = option
		attribute.Validate(value)
		handler(value)
		
	protected def InvalidOption(value as string):
		raise CommandLineException("'${value}' is not a valid option")
		
	override protected def OnArgument(value as string):
		ArgumentFound(value)
