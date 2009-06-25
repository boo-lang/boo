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

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import System.Runtime.CompilerServices
import System.Runtime.Serialization

class SingletonAttribute(AbstractAstAttribute):
"""
Implements the singleton design pattern for a class.

Guarantees that accessing the singleton instance is thread-safe.
Guarantees that singleton instance is not initialized until Instance is called (lazy instantiation).
Guarantees that all constructors are private.
Guarantees that a private default constructor exists. (One need not be provided.)
Guarantees that serializable singletons remain singletons through serialization/deserialization.

Enforces constraint that no parameterized constructors exist on the singleton class.
Enforces constraint that no static members exist on the singleton class (as a recommendation at the Warning level).

@author Marcus Griep (neoeinstein+boo@gmail.com)
@author Sorin Ionescu (sorin.ionescu@gmail.com)
@author Rodrigo B. de Oliveira
"""
	_suppressStaticWarning as bool
	_classDef as ClassDefinition
	
	[property(AccessorName)]
	_accessorName = ReferenceExpression(Name: DEFAULT_INSTANCE_PROPERTY_NAME)
	
	public static final NESTED_CLASS_NAME = "$Nested"
	public static final INSTANCE_FIELD_NAME = "instance"
	public static final SERIALIZATION_OBJECT_CLASS_NAME = "$ObjectForSerialization"
	public static final DEFAULT_INSTANCE_PROPERTY_NAME = "Instance"
	
	public static final ISERIALIZABLE_FULL_NAME = typeof(ISerializable).FullName
	public static final GETOBJECTDATA_METHOD_NAME = typeof(ISerializable).GetMethods()[0].Name
	public static final IOBJECTREFERENCE_FULL_NAME = typeof(IObjectReference).FullName
	public static final STREAMINGCONTEXT_FULL_NAME = typeof(StreamingContext).FullName
	public static final SERIALIZATIONINFO_FULL_NAME = typeof(SerializationInfo).FullName
	public static final COMPILERGENERATEDATTRIBUTE_FULL_NAME = typeof(CompilerGeneratedAttribute).FullName
	
	override def Apply(node as Node):
		if not node isa ClassDefinition:
			InvalidNodeForAttribute("Class");
			return
		
		_classDef = node as ClassDefinition

		# A singleton class cannot be inherited from, so we make it final here.		
		_classDef.Modifiers |= TypeMemberModifiers.Final
		EnforceMemberConstraints()
		
		CreateInstanceProperty()
		CreateNestedClass()
		
		if _classDef.Modifiers & TypeMemberModifiers.Transient == TypeMemberModifiers.None:
			CreateSerializationObjectReference()
			CreateDeserializer()
			_classDef.BaseTypes.Add(SimpleTypeReference(ISERIALIZABLE_FULL_NAME))			

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
		forRemoval = []
		for member as TypeMember in _classDef.Members:
			if member.IsStatic and not _suppressStaticWarning:
				Context.Warnings.Add(
					CompilerWarningFactory.CustomWarning(
						member.LexicalInfo, 
						"Static members should not be mixed with the SingletonAttribute pattern (${_classDef.Name}.${member.Name})."))
				_suppressStaticWarning = true

			
			// Because we only warn about static members,
			// we must take care not to override a possible
			// static constructor.  If a static constructor
			// is found, it doesn't count toward an instance
			// constructor, so we don't modify 'foundConstructor'.
			ctor = member as Constructor

			if ctor is not null and not member.IsStatic:
				if ctor.Parameters.Count == 0:
					ctor.Modifiers |= TypeMemberModifiers.Private
					ctor.Modifiers &= ~(TypeMemberModifiers.Public)
					foundDefaultConstructor = true
				else:
					Context.Warnings.Add(
						CompilerWarningFactory.CustomWarning(
							ctor.LexicalInfo,
							"SingletonAttribute pattern will never instantiate parameterized constructors.  '${ctor}' has been removed from '${_classDef.Name}'."))
					forRemoval.Add(ctor)
					foundParameterizedConstructor = true
		
		unless foundDefaultConstructor:
			defaultConstructor = [|
				private def constructor():
					pass
			|]
			AddCompilerGeneratedAttribute(defaultConstructor)
			_classDef.Members.Add(defaultConstructor)
			if foundParameterizedConstructor:
				Context.Warnings.Add(
					CompilerWarningFactory.CustomWarning(
						_classDef.LexicalInfo,
						"Default private constructor added to '${_classDef.Name}'."))
		
		for member in forRemoval:
			_classDef.Members.Remove(member)
		
	private def CreateInstanceProperty():
	"""
	Creates the public, static, read-only property 'Instance' and 
	adds it to the singleton.  This property accesses
	'$Nested.instance' to return the actual singleton instance.
	"""
		classRef = SimpleTypeReference(_classDef.Name)
		
		instanceProperty = [|
			static Accessor as $classRef:
				get:
					return self.$NESTED_CLASS_NAME.$INSTANCE_FIELD_NAME
		|]
		
		instanceProperty.Name = AccessorName.Name
		
		if AccessorName.LexicalInfo == LexicalInfo.Empty:
			instanceProperty.LexicalInfo = _classDef.LexicalInfo
		else:
			instanceProperty.LexicalInfo = AccessorName.LexicalInfo
		
		AddCompilerGeneratedAttribute(instanceProperty)

		_classDef.Members.Add(instanceProperty)

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
		classRef = ReferenceExpression(_classDef.Name)
		
		nestedInstanceClass = [| 
			private transient final class $NESTED_CLASS_NAME:
				static def constructor():
					self.$INSTANCE_FIELD_NAME = $classRef()
				
				private def constructor():
					pass
				
				public static final $INSTANCE_FIELD_NAME as $(_classDef.Name)
		|]
		
		AddCompilerGeneratedAttribute(nestedInstanceClass)
		
		_classDef.Members.Add(nestedInstanceClass)
	
	private def CreateSerializationObjectReference():
		classRef = ReferenceExpression(_classDef.Name)
		
		nestedReferenceClass = [|
			private final class $SERIALIZATION_OBJECT_CLASS_NAME($IOBJECTREFERENCE_FULL_NAME):
				public def GetRealObject(context as $STREAMINGCONTEXT_FULL_NAME):
					return $(classRef).$(AccessorName.Name)
		|]
		
		AddCompilerGeneratedAttribute(nestedReferenceClass)
		
		_classDef.Members.Add(nestedReferenceClass)

	private def CreateDeserializer():
		objRef = ReferenceExpression(SERIALIZATION_OBJECT_CLASS_NAME)
		
		deserializer = [|
			def $GETOBJECTDATA_METHOD_NAME(info as $SERIALIZATIONINFO_FULL_NAME, context as $STREAMINGCONTEXT_FULL_NAME):
				info.SetType($objRef)
		|]
		
		MakeExplicit(deserializer, ISERIALIZABLE_FULL_NAME)
		
		AddCompilerGeneratedAttribute(deserializer)
		
		_classDef.Members.Add(deserializer)
		
	private def AddCompilerGeneratedAttribute(node as TypeMember):
		node.Attributes.Add(
			Attribute(COMPILERGENERATEDATTRIBUTE_FULL_NAME))

	private def MakeExplicit(method as Method, interfaceName as string):
		method.ExplicitInfo = ExplicitMemberInfo(InterfaceType: SimpleTypeReference(interfaceName))
		
