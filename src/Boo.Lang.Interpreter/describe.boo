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


namespace Boo.Lang.Interpreter

import System
import System.Text
import System.Reflection
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.PatternMatching
import Boo.Lang.Useful.Doc.DocDB

import Boo.Lang.Interpreter.ColorScheme

def describe(obj):
	WithColor EmphasizeColor:
		miCol = obj as System.Collections.Generic.ICollection[of System.Reflection.MethodInfo]
		if miCol == null:
			mi = obj as System.Reflection.MethodInfo
			mcd = obj as System.MulticastDelegate
			mi = mcd.Method if mcd != null
			miCol.Add(mi) if mi != null
		if miCol == null:
			type = (obj as Type) or obj.GetType()
			for line in DescriptionFormatter("    ").GenerateFormattedLinesFor(type):
				Console.WriteLine(line)
		else:
			for mi in miCol:
				if mi.DeclaringType.IsClass:
					Console.Write("class ")
				elif mi.DeclaringType.IsInterface:
					Console.Write("interface ")
				elif mi.DeclaringType.IsValueType:
					Console.Write("struct ")
				Console.Write(GetBooTypeName(mi.DeclaringType))
				Console.WriteLine(":")
				Console.WriteLine(DescribeMethod(mi))
				Console.WriteLine(DescribeDoc("   ", Find(mi), true))
				Console.WriteLine()

class DescriptionFormatter:
	
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
		doc = DescribeDoc(string.Empty, Find(type), true)
		if not string.IsNullOrEmpty(doc):
			yield doc 
		yield ""
		
		for ctor in type.GetConstructors():
			yield "${indent}def constructor(${DescribeParameters(ctor.GetParameters())})"
			doc = DescribeDoc(indent, Find(ctor), false)
			if not string.IsNullOrEmpty(doc):
				yield doc 
			yield ""
			
		sortByName = def (lhs as MemberInfo, rhs as MemberInfo):
			return lhs.Name.CompareTo(rhs.Name)
			
		for f as FieldInfo in List(type.GetFields()).Sort(sortByName):
			yield "${indent}public ${DescribeField(f)}"
			doc = DescribeDoc(indent, Find(f), false)
			if not string.IsNullOrEmpty(doc):
				yield doc 
			yield ""
			
		for p as PropertyInfo in List(type.GetProperties()).Sort(sortByName):
			yield "${indent}${DescribeProperty(p)}:"
			doc = DescribeDoc(indent, Find(p), false)
			if not string.IsNullOrEmpty(doc):
				yield doc 
			yield "${indent}${indent}get" if p.GetGetMethod() is not null
			yield "${indent}${indent}set" if p.GetSetMethod() is not null
			yield ""
		
		for m as MethodInfo in List(type.GetMethods()).Sort(sortByName):
			continue if m.IsSpecialName
			yield "${indent}${DescribeMethod(m)}"
			doc = DescribeDoc(indent, Find(m), false)
			if not string.IsNullOrEmpty(doc):
				yield doc 
			yield ""
			
		for e as EventInfo in List(type.GetEvents()).Sort(sortByName):
			yield "${indent}${DescribeEvent(e)}"
			doc = DescribeDoc(indent, Find(e), false)
			if not string.IsNullOrEmpty(doc):
				yield doc 
			yield ""
			
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
			return entity.ToString()

def DescribeDoc(indent as string, doc as Xml.XmlNodeList, detailed as bool):
"""
	Param doc: XML nodes containing the documentation of a code entity.
	Param indent:A string that is used to introduce new lines. 
	Returns: A string
	
	Return a string showing the documentation on a particular
	objects. Use [indent] to indent lines.
	[detailed] requests with true an output containing all details.
	If this is false, only the summary and parameters will be returned.
	
	Remarks: I could provide some more details here but I don't know
	anything that remains to be explained.
"""
	return string.Empty if doc == null
	sb=StringBuilder()
	sb.AppendLine("${indent}\"\"\"")
	for i in range(doc.Count):
		subnode=doc.Item(i)
		subnodeText = subnode.InnerXml.Trim().Replace("\r\n", " ").Replace("\n", " ").Replace("  ", " ")
		if "summary".Equals(subnode.Name, StringComparison.InvariantCultureIgnoreCase):
			sb.AppendLine("${indent}${subnodeText}")
		elif detailed and "remarks".Equals(subnode.Name, StringComparison.InvariantCultureIgnoreCase):
			sb.AppendLine("${indent}Remarks: ${subnodeText}")
		elif "param".Equals(subnode.Name, StringComparison.InvariantCultureIgnoreCase):
			name = subnode.Attributes.GetNamedItem('name') or string.Empty
			sb.AppendLine("${indent}Param ${name}: ${subnodeText}")
		elif "returns".Equals(subnode.Name, StringComparison.InvariantCultureIgnoreCase):
			sb.AppendLine("${indent}Returns: ${subnodeText}")
	sb.Append("${indent}\"\"\"")
	return sb.ToString()

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
	return GenericTypeNameFor(type) if type.IsGenericType
	return FullNameOf(type)
	
def FullNameOf(type as System.Type):
	fullName = type.FullName
	if string.IsNullOrEmpty(fullName):
		return type.Name
	return fullName
	
def GenericTypeNameFor(type as System.Type):
	parameterList = join(GetBooTypeName(t) for t in type.GetGenericArguments(), ', ')
	definition = type.GetGenericTypeDefinition()
	if definition == typeof(System.Collections.Generic.IEnumerable of *):
		return "$parameterList*"
	fullName = FullNameOf(definition)
	simpleName = fullName[:fullName.IndexOf('`')]
	return "$simpleName[of $parameterList]"