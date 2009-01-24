#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


namespace Boo.Lang.Interpreter.Help

import System
import System.Reflection

class HelpFormatter:
	
	indent as string
	
	def constructor(indent as string):
		self.indent = indent
		
	def GenerateFormattedLinesFor(type as Type):
		if type.IsInterface:
			typeDef = "interface"
			baseTypes = array(GetBooTypeName(t) for t in type.GetInterfaces())
		else:
			typeDef = "class"
			baseTypes = (GetBooTypeName(type.BaseType),) + array(GetBooTypeName(t) for t in type.GetInterfaces())
			
		yield "${typeDef} ${type.Name}(${join(baseTypes, ', ')}):"
		yield ""
		
		for ctor in type.GetConstructors():
			yield "${indent}def constructor(${DescribeParameters(ctor.GetParameters())})"
			yield ""
			
		sortByName = def (lhs as MemberInfo, rhs as MemberInfo):
			return lhs.Name.CompareTo(rhs.Name)
			
		for f as FieldInfo in List(type.GetFields()).Sort(sortByName):
			yield "${indent}public ${DescribeField(f)}"
			yield ""
			
		for p as PropertyInfo in List(type.GetProperties()).Sort(sortByName):
			yield "${indent}${DescribeProperty(p)}:"
			yield "${indent}${indent}get" if p.GetGetMethod() is not null
			yield "${indent}${indent}set" if p.GetSetMethod() is not null
			yield ""
		
		for m as MethodInfo in List(type.GetMethods()).Sort(sortByName):
			continue if m.IsSpecialName
			yield "${indent}${DescribeMethod(m)}"
			yield ""
			
		for e as EventInfo in List(type.GetEvents()).Sort(sortByName):
			yield "${indent}${DescribeEvent(e)}"
			yield ""