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
		assert node isa ClassDefinition
		
		classDef as ClassDefinition = node
		assert ExtendsObject(classDef), "cannot introduce AbstractCollection base class"		
		RemoveObjectBaseType(classDef)
		
		template = CollectionTemplate.CloneNode()
		template.ReplaceNodes(SimpleTypeReference("T"), _itemType)
		classDef.Merge(template)
		
	static final CollectionTemplate = ast:
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
			Item(index as int) as T:
				get:
					return self.InnerList[index]
						
	def ExtendsObject(classDef as ClassDefinition):
		return cast(IType, classDef.Entity).BaseType is TypeSystemServices.ObjectType
		
	def RemoveObjectBaseType(classDef as ClassDefinition):
		for item in classDef.BaseTypes:
			if item.Entity is TypeSystemServices.ObjectType:
				classDef.BaseTypes.Remove(item)
				return
		assert false
