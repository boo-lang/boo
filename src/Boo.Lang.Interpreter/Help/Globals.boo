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

import System.Reflection
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.PatternMatching

def DescribeEntity(entity as IEntity):
	match entity:
		case method = ExternalMethod(MethodInfo: MethodInfo()):
			return DescribeMethod(method.MethodInfo)
		case field = ExternalField():
			return DescribeField(field.FieldInfo)
		case property = ExternalProperty():
			return DescribeProperty(property.PropertyInfo)
		case e = ExternalEvent():
			return DescribeEvent(e.EventInfo)
		otherwise:
			entity.ToString()
		
def DescribeEvent(e as EventInfo):
	return "${DescribeModifiers(e)}event ${e.Name} as ${e.EventHandlerType}"
		
def DescribeProperty(p as PropertyInfo):
	modifiers = DescribeModifiers(p)
	params = DescribePropertyParameters(p.GetIndexParameters())
	return "${modifiers}${p.Name}${params} as ${GetBooTypeName(p.PropertyType)}"
		
def DescribeField(f as FieldInfo):
	return "${DescribeModifiers(f)}${f.Name} as ${GetBooTypeName(f.FieldType)}"
		
def DescribeMethod(m as MethodInfo):
	returnType = GetBooTypeName(m.ReturnType)
	modifiers = DescribeModifiers(m)
	return "${modifiers}def ${m.Name}(${DescribeParameters(m.GetParameters())}) as ${returnType}"
		
def DescribeModifiers(f as FieldInfo):
	return "static " if f.IsStatic
	return ""
		
def DescribeModifiers(m as MethodBase):
	return "static " if m.IsStatic
	return ""
	
def DescribeModifiers(e as EventInfo):
	return DescribeModifiers(e.GetAddMethod(true) or e.GetRemoveMethod(true))
	
def DescribeModifiers(p as PropertyInfo):
	accessor = p.GetGetMethod(true) or p.GetSetMethod(true)
	return DescribeModifiers(accessor)
		
def DescribePropertyParameters(parameters as (ParameterInfo)):
	return "" if 0 == len(parameters)
	return "(${DescribeParameters(parameters)})"
		
def DescribeParameters(parameters as (ParameterInfo)):
	return join(DescribeParameter(p) for p in parameters, ", ")
	
def DescribeParameter(p as ParameterInfo):
	return "${p.Name} as ${GetBooTypeName(p.ParameterType)}"
	
def GetBooTypeName(type as System.Type) as string:
	return "(${GetBooTypeName(type.GetElementType())})" if type.IsArray
	return "object" if object is type
	return "string" if string is type
	return "void" if void is type
	return "bool" if bool is type		
	return "byte" if byte is type
	return "char" if char is type
	return "sbyte" if sbyte is type
	return "short" if short is type
	return "ushort" if ushort is type
	return "int" if int is type
	return "uint" if uint is type
	return "long" if long is type
	return "ulong" if ulong is type
	return "single" if single is type
	return "double" if double is type
	return "date" if date is type
	return "timespan" if timespan is type
	return "regex" if regex is type
	return type.FullName