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


namespace Boo.Lang.Useful.Collections

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem

class CollectionAttribute(AbstractAstAttribute):
"""
Generates a simple strongly typed collection
which extends Boo.Lang.Useful.Collections.AbstractCollection.

Example:
	[collection(CompilerError)]
	class CompilerErrorCollection:
		pass
"""
	_itemType as SimpleTypeReference
	
	def constructor(itemType as ReferenceExpression):
		_itemType = SimpleTypeReference(itemType.ToString())	
		
	override def Apply(node as Node):
		if not node isa ClassDefinition:
			InvalidNodeForAttribute("Class")
			return
		
		classDef as ClassDefinition = node
		if not ExtendsObject(classDef):
			Errors.Add(CompilerErrorFactory.CustomError(node.LexicalInfo,
				"Cannot introduce AbstractCollection base class."))
			return
		RemoveObjectBaseType(classDef)
		
		template = CollectionTemplate.CloneNode()
		template.ReplaceNodes(SimpleTypeReference("T"), _itemType)
		classDef.Merge(template)
		
	static final CollectionTemplate = [|
		[EnumeratorItemType(typeof(T))]
		[System.Reflection.DefaultMember("Item")]
		class Collection(Boo.Lang.Useful.Collections.AbstractCollection):
			def constructor():
				pass
			def constructor([required] enumerable):
				for item as T in enumerable:
					self.Add(item)
			def Add([required] item as T):
				self.InnerList.Add(item)
			Item[index as int] as T:
				get:
					return self.InnerList[index]
	|]
						
	def ExtendsObject(classDef as ClassDefinition):
		return IsObject(cast(IType, TypeSystemServices.GetEntity(classDef)).BaseType)
		
	def RemoveObjectBaseType(classDef as ClassDefinition):
		for item in classDef.BaseTypes:
			if IsObject(TypeSystemServices.GetEntity(item)):
				classDef.BaseTypes.Remove(item)
				return
		raise System.InvalidOperationException("no base type removed")

	def IsObject(entity as IEntity):
		 return entity is TypeSystemServices.ObjectType
