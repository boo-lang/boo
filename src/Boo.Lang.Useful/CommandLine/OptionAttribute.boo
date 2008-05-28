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

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
class OptionAttribute(Attribute):
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
	
	[getter(Occurred)]
	_occurred = 0
	
	[property(Description)]
	_description as string
	
	def constructor():
		pass
		
	def constructor(description as string):
		_description = description
		
	override def ToString():
		return _long or _short
	
	def PreValidate():
		assert LongForm is not null
		
	def PostValidate():
		if _occurred < _minOccurs:
			raise CommandLineException("${_long or _short} must be specified at least ${_minOccurs} time(s)") 
	
	def Validate(value as string):
		++_occurred
		if _occurred > _maxOccurs:
			raise CommandLineException("${_long or _short} cannot be used more than ${_maxOccurs} time(s)")

