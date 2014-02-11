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
import System.Collections

[System.Reflection.DefaultMember("Item")]
[CmdClass("Namespaces")]
class Namespace:
"""
Namespace introspection helper.

>>> root = Boo.Lang.Interpreter.Namespace.GetRootNamespace()
>>> types = root["System"]["Collections"].Types
>>> print join(types, "\n")
"""
	[CmdDeclaration("namespaces ns", Description:"A list of all namespaces.")]
	static public def ListNamespaces([CmdArgument(CmdArgumentCompletion.None)] part as string):
		root = GetRootNamespace()
		ListNamespace(root, part)
	
	public static def Find(nsName):
		root = GetRootNamespace()
		return root.FindChild(nsName)
	
	public static def ListTypes(ns as Namespace):
		for t in ns.Types:
			if t.IsClass:
				Console.WriteLine( " class "+repr(t))
			elif t.IsEnum:
				Console.WriteLine( " enum "+repr(t))
			elif t.IsValueType:
				Console.WriteLine( " struct "+repr(t))
			elif t.IsInterface:
				Console.WriteLine(" interface "+repr(t))
			else:
				Console.WriteLine(" "+repr(t))
	
	public static def ListNamespace(ns as Namespace, part as string):
		if string.IsNullOrEmpty(part):
			Console.WriteLine( ns.FullName )
		elif ns.FullName.ToLower().Contains(part.ToLower()):
			Console.WriteLine( ns.FullName )
		for subns in ns.Namespaces:
			ListNamespace(subns, part)
	
	static def GetRootNamespace() as Namespace:			
		root = Namespace("", null)
		GetNamespace = def(ns as string):
			return root if string.IsNullOrEmpty(ns)
			parts = /\./.Split(ns)
			found = root
			for part in parts:
				found = found.GetOrCreateNamespace(part)
			return found
		
		for asm in System.AppDomain.CurrentDomain.GetAssemblies():
			try:
				types = asm.GetTypes()
			except:
				continue
			for type in types:
				ns = GetNamespace(type.Namespace)
				ns.AddType(type)	
		return root
	
	[Getter(Name)]
	_name as string
	[Getter(Qualifier)]
	_qualifier as Namespace
	
	_types = List[of System.Type]()
	_children = Generic.SortedList[of string, Namespace]()
	
	def constructor([required] name as string, qualifier):
		_name = name
		_qualifier = qualifier
	
	public FullName as string:
		get:
			result=self.Name
			if self._qualifier != null and not string.IsNullOrEmpty(self._qualifier.Name):
				result = self._qualifier.FullName+"."+result
			return result
	
	internal def GetOrCreateNamespace([required] ns as string):
		found as Namespace
		_children.TryGetValue(ns, found)
		if found is null:
			found = Namespace(ns, self) 
			_children.Add(ns, found)
		return found
	
	public def FindChild(name as string) as Namespace:
		if string.IsNullOrEmpty(name):
			return null
		fullQualifier = string.Empty
		if self._qualifier != null:
			fullQualifier=self._qualifier.FullName+"."
		if name.StartsWith(fullQualifier):
			name=name.Substring(fullQualifier.Length)
		if string.Equals(self.Name, name):
			return self
		elif name.StartsWith(self.Name+"."):
			name=name.Substring(self.Name.Length+1)
			for ns in self.Namespaces:
				result=ns.FindChild(name)
				if result != null:
					return result
			return null
		elif string.IsNullOrEmpty(self.Name):
			for ns in self.Namespaces:
				result=ns.FindChild(name)
				if result != null:
					return result
			return null			
		else:
			return null			
	
	internal def AddType([required] type as System.Type):
		assert not _types.Contains(type)
		_types.Add(type)
		
	Types as (System.Type):
		get: return _types.ToArray()
			
	def GetType(name as string):
		for type as System.Type in _types:
			return type if name == type.Name
			
	Namespaces:
		get: return array(value as Namespace for value in _children.Values)
			
	Item[name as string] as Namespace:
		get: return _children[name]
	
	NamespacesNames:
		get: return self._children.Keys
			
	override def ToString():
		return "${_name} - ${len(_types)} type(s), ${len(_children)} namespace(s)"
