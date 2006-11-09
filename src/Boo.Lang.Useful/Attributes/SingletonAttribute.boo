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

namespace Boo.Lang.Useful.Attributes

import System.Threading
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem

class SingletonAttribute(AbstractAstAttribute):
"""
Implements the singleton design pattern for a class.

Ensures that the singleton is thread-safe.
Ensures that singleton instance is not initialized until Instance is called (lazy instantiation).
Ensures that all constructors are private.
Ensures that if no constructor exists, a private default constructor is provided.
Enforces constraint that no static members exist on the singleton class (as a recommendation at the Warning level).

@example
Before:
	[Singleton]
	class OnlyOne:
		pass

After:
	final class OnlyOne:
		private def constructor():
			pass
			
		Instance as OnlyOne:
			get:
				return __Nested.instance
			
		private class __Nested:
			internal static final instance = OnlyOne()

@author Marcus Griep (neoeinstein+boo@gmail.com)
@author Sorin Ionescu (sorin.ionescu@gmail.com)
@author Rodrigo B. de Oliveira
"""
	_singletonType as ClassDefinition
	_nestedInstanceClass as ClassDefinition
	_suppressStaticWarning as bool
	
	override def Apply(node as Node):
		if not node isa ClassDefinition:
			InvalidNodeForAttribute("Class");
			return
		
		_singletonType = node
		
		MakeClassFinal()
		EnforceMemberConstraints()
		CreateInstanceProperty()
		CreateNestedClass()

	private def MakeClassFinal():
	"""
	A singleton class cannot be inherited from, so we make it final here.
	"""
		_singletonType.Modifiers |= TypeMemberModifiers.Final
		
	private def EnforceMemberConstraints():
	"""
	Checks all type members to ensure compliance with singleton specification.
	In this case, we discourage the use of static members on a singleton class
	but we do not make disallow it.  The attribute will funtion properly 
	regardless of whether or not a class strictly follows the pattern.  Also,
	we ensure that every instance constructor is private, and if the user
	never provided an instance constructor, we provide the default one as
	private so that Boo doesn't automatically create the default public 
	constructor.  There may be a static constructor, but we do not factor this
	in because we warn about it as aforementioned.
	"""
		for member as TypeMember in _singletonType.Members:
			if member.IsStatic and not _suppressStaticWarning:
				Context.Warnings.Add(CompilerWarningFactory.CustomWarning(member.LexicalInfo, "Static members should not be mixed with the SingletonAttribute pattern (${_singletonType.Name}.${member.Name})."))
				_suppressStaticWarning = true
			ctor = member as Constructor
			
			// Because we only warn about static members,
			// we must take care not to override a possible
			// static constructor.  If a static constructor
			// is found, it doesn't count toward an instance
			// constructor, so we don't modify 'foundConstructor'.
			if ctor is not null and not member.IsStatic:
				EnforcePrivateInstanceConstructorConstraint(ctor)
				foundConstructor = true
				
		CreatePrivateDefaultConstructor() unless foundConstructor			

	private def EnforcePrivateInstanceConstructorConstraint([required] ctor as Constructor):
	"""
	Marks a constructor as private.  ctor cannot be null.
	"""
		ctor.Modifiers |= TypeMemberModifiers.Private
		
	private def CreatePrivateDefaultConstructor():
	"""
	This function creates the default constructor for a class but
	marks its visibility as private.  The function then adds the
	constructor to the singleton.
	"""
		_singletonType.Members.Add(
			Constructor(
				LexicalInfo: self.LexicalInfo,
				Modifiers: TypeMemberModifiers.Private))

	private def CreateInstanceProperty():
	"""
	Creates the public, static, read-only property 'Instance' and 
	adds it to the singleton.  This property accesses
	'__Nested.instance' to return the actual singleton instance.
	"""
		instanceProperty = Property(LexicalInfo: self.LexicalInfo,
								Name: "Instance",
								Modifiers: TypeMemberModifiers.Public | TypeMemberModifiers.Static)
		getter = instanceProperty.Getter = Method(LexicalInfo: self.LexicalInfo, Name: "get")
		
		getter.Body.Add(ReturnStatement(MemberReferenceExpression(ReferenceExpression("__Nested"),"instance")))
			 
		_singletonType.Members.Add(instanceProperty)

	private def CreateNestedClass():
	"""
	Creates the nested class and calls helper functions to generate
	the nested members.  This class is marked private to ensure that
	only the singleton class will be able to access it.  Note that
	accessing this nested class will cause the lazy initialization
	to trigger.  The reason for this internal class is to allow accesses
	to other static methods without triggering initialization of the
	instance until it is called for.  This is the most elegant solution
	I know of for the .Net framework to allow for this functionality.
	"""
		_nestedInstanceClass = ClassDefinition(
							LexicalInfo: self.LexicalInfo,
							Name: "__Nested",
							Modifiers: TypeMemberModifiers.Private)
		
		//See AddEmptyStaticConstructorToNested() method doc for the 
		// reason the following line is commented out.
		//
		//AddEmptyStaticConstructorToNested() 
		
		AddInstanceFieldToNested()
		_singletonType.Members.Add(_nestedInstanceClass)
	
	private def AddEmptyStaticConstructorToNested():
	"""
	This function is only needed to evoke functionality similar to C# wherein
	any class with a static constructor is not emitted to IL with the beforefieldinit
	modifier.  Boo does not emit this modifier, so this method is unnecessary,
	but may be needed if Boo ever changes that.  Note that its call is commented
	out above for this reason.
	"""
		emptyStaticConstructor = Constructor(
								LexicalInfo: self.LexicalInfo,
								Modifiers: TypeMemberModifiers.Public | TypeMemberModifiers.Static)
		_nestedInstanceClass.Members.Add(emptyStaticConstructor)
		
	private def AddInstanceFieldToNested():
	"""
	Creates the 'instance' field which is the actual singleton instance.
	It is created as a final field with an initializer, meaning that it
	will be initialized whenever '__Nested's static constructor is called
	(which happens only on the singleton's 'Instance' property being called.
	Using final instead of checking for 'instance == null' ensures thread-safety
	and ensures that the singleton is only initialized with the express
	consent of the singleton's consumer (the consumer must call 'OnlyOne.Instance'
	to initialize the singleton)
	"""
		instance = Field(
					LexicalInfo: self.LexicalInfo,
					Name: "instance",
					Modifiers: TypeMemberModifiers.Internal | TypeMemberModifiers.Static | TypeMemberModifiers.Final,
					Type: SimpleTypeReference(_singletonType.Name),
					Initializer: MethodInvocationExpression(
						ReferenceExpression(_singletonType.Name)))
		_nestedInstanceClass.Members.Add(instance)
