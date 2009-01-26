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

[System.Reflection.DefaultMember("Item")]
class Namespace:
"""
Namespace introspection helper.

>>> root = Boo.Lang.Interpreter.Namespace.GetRootNamespace()
>>> types = root["System"]["Collections"].Types
>>> print join(types, "\n")
"""
		
	static def GetRootNamespace():
				
		root = Namespace("")
		
		GetNamespace = def(ns as string):
			return root if ns is null or len(ns) == 0
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
				
	_name as string
	_types = []
	_children = {}
	
	def constructor([required] name as string):
		_name = name
	
	internal def GetOrCreateNamespace([required] ns as string):
		found as Namespace = _children[ns]
		if found is null:
			found = Namespace(ns) 
			_children.Add(ns, found)
		return found
		
	internal def AddType([required] type as System.Type):
		assert not _types.Contains(type)
		_types.Add(type)
		
	Types as (System.Type):
		get:
			return array(System.Type, _types)
			
	def GetType(name as string):
		for type as System.Type in _types:
			return type if name == type.Name
			
	Namespaces as (Namespace):
		get:
			return array(Namespace, _children.Values)
			
	Item[name as string] as Namespace:
		get:
			return _children[name]
			
	override def ToString():
		return "${_name} - ${len(_types)} type(s), ${len(_children)} namespace(s)"
