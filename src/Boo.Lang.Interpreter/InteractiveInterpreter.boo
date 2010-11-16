#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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
import System.Collections.Generic
import Boo.Lang.Compiler

class InteractiveInterpreter(AbstractInterpreter):

	_values = Dictionary[of string, object]()
	
	_declarations = {}
	
	[getter(LastValue)]
	_lastValue = null

	def constructor():
		super()
		InitializeStandardReferences()
		
	def constructor(parser as ICompilerStep):
		super(parser)
		InitializeStandardReferences()
		
	Values as KeyValuePair[of string, object]*:
		get: return _values
		
	def Reset():
		_values.Clear()
		_declarations.Clear()
		_lastValue = null
		InitializeStandardReferences()
	
	override def Declare([required] name as string, [required] type as System.Type):
		_declarations.Add(name, type)
		
	override def SetLastValue(value):
		_lastValue = value
		
	override def SetValue(name as string, value):
		_values[name] = value
		return value

	override def GetValue(name as string):
		value as object
		if _values.TryGetValue(name, value):
			return value
		return null
		
	override def Lookup([required] name as string):
		type as System.Type = _declarations[name]
		return type if type is not null
		
		value = GetValue(name)
		return value.GetType() if value is not null
	
	private def InitializeStandardReferences():
		SetValue("interpreter", self)
		SetValue("globals", { array(key for key in _values.Keys) })
					
