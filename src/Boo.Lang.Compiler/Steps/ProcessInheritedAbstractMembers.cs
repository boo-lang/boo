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

using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.Steps
{
	public class ProcessInheritedAbstractMembers : AbstractVisitorCompilerStep, ITypeMemberReifier
	{
		private List<TypeDefinition> _newAbstractClasses;
		private Hash _classDefinitionList;
		private int _depth;
		private Set<IEntity> _explicitMembers;

		override public void Run()
		{	
			_newAbstractClasses = new List<TypeDefinition>();
			_classDefinitionList = new Hash();

			Visit(CompileUnit.Modules);

			_classDefinitionList.Clear();
			ProcessNewAbstractClasses();
		}

		override public void Dispose()
		{
			_newAbstractClasses = null;
			base.Dispose();
		}

		override public void OnProperty(Property node)
		{
			if (node.IsAbstract && null == node.Type)
				node.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType);

			ExplicitMemberInfo explicitInfo = node.ExplicitInfo;
			if (null != explicitInfo)
			{
				Visit(explicitInfo);
				if (null != explicitInfo.Entity)
					ProcessPropertyImplementation(node, (IProperty) explicitInfo.Entity);
			}
		}

		override public void OnMethod(Method node)
		{
			if (node.IsAbstract && null == node.ReturnType)
				node.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);

			ExplicitMemberInfo explicitInfo = node.ExplicitInfo;
			if (null != explicitInfo)
			{
				Visit(explicitInfo);
				if (null != explicitInfo.Entity)
					ProcessMethodImplementation(node, (IMethod) explicitInfo.Entity);
			}
		}

		override public void OnExplicitMemberInfo(ExplicitMemberInfo node)
		{
			TypeMember member = (TypeMember)node.ParentNode;
			if (!CheckExplicitMemberValidity((IExplicitMember)member))
				return;

			IType interfaceType = GetEntity(node.InterfaceType);
			IEntity baseMember = FindImplementationFor((IMember) member.Entity, interfaceType);
			if (null == baseMember)
			{
				Error(CompilerErrorFactory.NotAMemberOfExplicitInterface(member, interfaceType));
				return;
			}

			TraceImplements(member, baseMember);
			node.Entity = baseMember;
		}

		bool CheckExplicitMemberValidity(IExplicitMember member)
		{
			Node node = (Node) member;
			IMember explicitMember = (IMember)GetEntity(node);
			IType declaringType = explicitMember.DeclaringType;
			if (!declaringType.IsClass)
			{
				Error(CompilerErrorFactory.InvalidTypeForExplicitMember(node, declaringType));
				return false;
			}

			IType targetInterface = GetType(member.ExplicitInfo.InterfaceType);
			if (!targetInterface.IsInterface)
			{
				Error(CompilerErrorFactory.InvalidInterfaceForInterfaceMember(node, member.ExplicitInfo.InterfaceType.Name));
				return false;
			}

			if (!declaringType.IsSubclassOf(targetInterface))
			{
				Error(CompilerErrorFactory.InterfaceImplForInvalidInterface(node, targetInterface.Name, ((TypeMember)node).Name));
				return false;
			}

			return true;
		}

		override public void LeaveInterfaceDefinition(InterfaceDefinition node)
		{
			MarkVisited(node);
		}

		override public void LeaveClassDefinition(ClassDefinition node)
		{
			MarkVisited(node);
			
			if (!_classDefinitionList.Contains(node.Name))
				_classDefinitionList.Add(node.Name, node);

			_explicitMembers = ExplicitlyImplementedMembersOn(node);
			foreach (TypeReference baseTypeRef in node.BaseTypes)
			{	
				IType baseType = GetType(baseTypeRef);
				EnsureRelatedNodeWasVisited(node, baseType);
				
				if (baseType.IsInterface)
					ResolveInterfaceMembers(node, baseTypeRef, baseType);
				else if (IsAbstract(baseType))
					ResolveAbstractMembers(node, baseTypeRef, baseType);
			}
		}

		private Set<IEntity> ExplicitlyImplementedMembersOn(ClassDefinition definition)
		{
			var explicitMembers = new Set<IEntity>();
			foreach (TypeMember member in definition.Members)
			{
				var explicitMember = member as IExplicitMember;
				if (null == explicitMember)
					continue;

				var memberInfo = explicitMember.ExplicitInfo;
				if (null == memberInfo || null == memberInfo.Entity)
					continue;

				explicitMembers.Add(memberInfo.Entity);
			}
			return explicitMembers;
		}

		/// <summary>
		/// This function checks for inheriting implementations from EXTERNAL classes only.
		/// </summary>
		bool CheckInheritsInterfaceImplementation(ClassDefinition node, IMember abstractMember)
		{
			foreach (TypeReference baseTypeRef in node.BaseTypes)
			{
				IType type = GetType(baseTypeRef);
				if (type.IsInterface)
					continue;

				IEntity implementation = FindImplementationFor(abstractMember, type);
				if (null != implementation)
					return true;
			}
			return false;
		}

		private IEntity FindImplementationFor(IMember member, IType inType)
		{
			foreach (IEntity candidate in ImplementationCandidatesFor(member, inType))
			{
				if (candidate == member)
					continue;

				if (IsValidImplementationFor(member, candidate))
					return candidate;
			}
			return null;
		}

		private bool IsValidImplementationFor(IEntity member, IEntity candidate)
		{
			switch (member.EntityType)
			{
				case EntityType.Method:
					if (CheckInheritedMethodImpl(candidate as IMethod, member as IMethod))
						return true;
					break;
				case EntityType.Event:
					if (CheckInheritedEventImpl(candidate as IEvent, member as IEvent))
						return true;
					break;
				case EntityType.Property:
					if (CheckInheritedPropertyImpl(candidate as IProperty, member as IProperty))
						return true;
					break;
			}
			return false;
		}

		private IEnumerable<IEntity> ImplementationCandidatesFor(IMember abstractMember, IType inBaseType)
		{
			foreach (IEntity candidate in inBaseType.GetMembers())
			{
				if (candidate.EntityType != abstractMember.EntityType)
					continue;

				if (candidate.EntityType == EntityType.Field)
					continue;

				string candidateName = abstractMember.DeclaringType.IsInterface
				                       	? SimpleNameOf(candidate)
				                       	: candidate.Name;
				if (candidateName == abstractMember.Name)
					yield return candidate;
			}
		}

		private string SimpleNameOf(IEntity candidate)
		{
			//candidate.Name == "CopyTo"
			//vs
			//candidate.Name == "System.ICollection.CopyTo"
			string[] temp = candidate.FullName.Split('.');
			return temp[temp.Length - 1];
		}

		bool CheckInheritedMethodImpl(IMethod impl, IMethod baseMethod)
		{
			return TypeSystemServices.CheckOverrideSignature(impl, baseMethod);
		}

		bool CheckInheritedEventImpl(IEvent impl, IEvent target)
		{
			return impl.Type == target.Type;
		}

		bool CheckInheritedPropertyImpl(IProperty impl, IProperty target)
		{
			if (!TypeSystemServices.CheckOverrideSignature(impl.GetParameters(), target.GetParameters()))
				return false;
	
			if (HasGetter(target))
			{
				if (!HasGetter(impl))
					return false;
			}
			if (HasSetter(target))
			{
				if (!HasSetter(impl))
					return false;
			}
			/* Unnecessary?
			  if(impl.IsPublic != target.IsPublic || 
			   impl.IsProtected != target.IsProtected ||
			   impl.IsPrivate != target.IsPrivate)
			{
				return false;
			}*/
			return true;
		}

		private static bool HasGetter(IProperty property)
		{
			return property.GetGetMethod() != null;
		}

		private static bool HasSetter(IProperty property)
		{
			return property.GetSetMethod() != null;
		}

		private bool IsAbstract(IType type)
		{
			if (type.IsAbstract)
				return true;

			AbstractInternalType internalType = type as AbstractInternalType;
			if (null != internalType)
				return _newAbstractClasses.Contains(internalType.TypeDefinition);
			return false;
		}

		void ResolveAbstractProperty(ClassDefinition node,
			TypeReference baseTypeRef,
			IProperty baseProperty)
		{			
			foreach (Property p in GetAbstractPropertyImplementationCandidates(node, baseProperty))
			{
				if (!ResolveAsImplementationOf(baseProperty, p))
					continue;

				//fully-implemented?
				if (!HasGetter(baseProperty) || (HasGetter(baseProperty) && null != p.Getter))
					if (!HasSetter(baseProperty) || (HasSetter(baseProperty) && null != p.Setter))
						return;
			}

			foreach(SimpleTypeReference parent in node.BaseTypes)
			{
				if(_classDefinitionList.Contains(parent.Name))
				{
					_depth++;
					ResolveAbstractProperty(_classDefinitionList[parent.Name] as ClassDefinition, baseTypeRef, baseProperty);
					_depth--;
				}
			}

			if (CheckInheritsInterfaceImplementation(node, baseProperty))
				return;

			if (_depth == 0)
				AbstractMemberNotImplemented(node, baseTypeRef, baseProperty);
		}

		private bool ResolveAsImplementationOf(IProperty baseProperty, Property property)
		{
			if (!TypeSystemServices.CheckOverrideSignature(GetEntity(property).GetParameters(), baseProperty.GetParameters()))
				return false;

			ProcessPropertyImplementation(property, baseProperty);
			AssertValidPropertyImplementation(property, baseProperty);
			return true;
		}

		private void AssertValidPropertyImplementation(Property p, IProperty baseProperty)
		{
			if (baseProperty.Type != p.Type.Entity)
				Error(CompilerErrorFactory.ConflictWithInheritedMember(p, p.FullName, baseProperty.FullName));

			AssertValidInterfaceImplementation(p, baseProperty);
		}

		private void ProcessPropertyImplementation(Property p, IProperty baseProperty)
		{
			if (p.Type == null) p.Type = CodeBuilder.CreateTypeReference(baseProperty.Type);
			ProcessPropertyAccessor(p, p.Getter, baseProperty.GetGetMethod());
			ProcessPropertyAccessor(p, p.Setter, baseProperty.GetSetMethod());
		}

		private static void ProcessPropertyAccessor(Property p, Method accessor, IMethod method)
		{
			if (accessor == null)
				return;

			accessor.Modifiers |= TypeMemberModifiers.Virtual;

			if (p.ExplicitInfo != null)
			{
				accessor.ExplicitInfo = p.ExplicitInfo.CloneNode();
				accessor.ExplicitInfo.Entity = method;
				accessor.Visibility = TypeMemberModifiers.Private;
			}
		}

		void ResolveAbstractEvent(ClassDefinition node, TypeReference baseTypeRef, IEvent baseEvent)
		{
			var ev = node.Members[baseEvent.Name] as Event;
			if (ev != null)
			{
				ProcessEventImplementation(ev, baseEvent);
				return;
			}

			if (CheckInheritsInterfaceImplementation(node, baseEvent))
				return;

			foreach (SimpleTypeReference parent in node.BaseTypes)
			{
				if (_classDefinitionList.Contains(parent.Name))
				{
					_depth++;
					ResolveAbstractEvent(_classDefinitionList[parent.Name] as ClassDefinition, baseTypeRef, baseEvent);
					_depth--;
				}
			}

			if (_depth == 0)
			{
				TypeMember conflicting;
				if (null == ev && null != (conflicting = node.Members[baseEvent.Name]))
				{
					//we've got a non-resolved conflicting member
					Error(CompilerErrorFactory.ConflictWithInheritedMember(conflicting, conflicting.FullName, baseEvent.FullName));
					return;
				}
				AddStub(node, CodeBuilder.CreateAbstractEvent(baseTypeRef.LexicalInfo, baseEvent));
				AbstractMemberNotImplemented(node, baseTypeRef, baseEvent);
			}
		}

		private void ProcessEventImplementation(Event ev, IEvent baseEvent)
		{
			Method add = ev.Add;
			if (add != null)
			{
				add.Modifiers |= TypeMemberModifiers.Final | TypeMemberModifiers.Virtual;
			}

			Method remove = ev.Remove;
			if (remove != null)
			{
				remove.Modifiers |= TypeMemberModifiers.Final | TypeMemberModifiers.Virtual;
			}

			Method raise = ev.Remove;
			if (raise != null)
			{
				raise.Modifiers |= TypeMemberModifiers.Final | TypeMemberModifiers.Virtual;
			}

			AssertValidInterfaceImplementation(ev, baseEvent);
			_context.TraceInfo("{0}: Event {1} implements {2}", ev.LexicalInfo, ev, baseEvent);
		}

		void ResolveAbstractMethod(ClassDefinition node, TypeReference baseTypeRef, IMethod baseMethod)
		{
			if (baseMethod.IsSpecialName)
				return;
			
			foreach (Method method in GetAbstractMethodImplementationCandidates(node, baseMethod))
				if (ResolveAsImplementationOf(baseMethod, method))
					return;

			// FIXME: this will fail with InvalidCastException on a base type that's a GenericTypeReference!
			foreach(SimpleTypeReference parent in node.BaseTypes)
			{
				if(_classDefinitionList.Contains(parent.Name))
				{
					_depth++;
					ResolveAbstractMethod(_classDefinitionList[parent.Name] as ClassDefinition, baseTypeRef, baseMethod);
					_depth--;
				}
			}

			if (CheckInheritsInterfaceImplementation(node, baseMethod))
				return;

			if (_depth == 0)
			{			
				if (!AbstractMemberNotImplemented(node, baseTypeRef, baseMethod))
				{
					//BEHAVIOR < 0.7.7: no stub, mark class as abstract
					AddStub(node, CodeBuilder.CreateAbstractMethod(baseTypeRef.LexicalInfo, baseMethod));
				}
			}
		}

		private bool ResolveAsImplementationOf(IMethod baseMethod, Method method)
		{
			if (!TypeSystemServices.CheckOverrideSignature(GetEntity(method), baseMethod))
				return false;

			ProcessMethodImplementation(method, baseMethod);

			if (!method.IsOverride && !method.IsVirtual)
				method.Modifiers |= TypeMemberModifiers.Virtual;

			AssertValidInterfaceImplementation(method, baseMethod);
			TraceImplements(method, baseMethod);
			return true;
		}

		private void ProcessMethodImplementation(Method method, IMethod baseMethod)
		{
			IMethod methodEntity = GetEntity(method);
			CallableSignature baseSignature = TypeSystemServices.GetOverriddenSignature(baseMethod, methodEntity);
			if (IsUnknown(methodEntity.ReturnType))
				method.ReturnType = CodeBuilder.CreateTypeReference(baseSignature.ReturnType);
			else if (baseSignature.ReturnType != methodEntity.ReturnType)
				Error(CompilerErrorFactory.ConflictWithInheritedMember(method, method.FullName, baseMethod.FullName));
		}

		private void TraceImplements(TypeMember member, IEntity baseMember)
		{
			_context.TraceInfo("{0}: Member {1} implements {2}", member.LexicalInfo, member, baseMember);
		}

		private bool IsUnknown(IType type)
		{
			return TypeSystem.TypeSystemServices.IsUnknown(type);
		}

		private IEnumerable<Method> GetAbstractMethodImplementationCandidates(TypeDefinition node, IMethod baseMethod)
		{
			return GetAbstractMemberImplementationCandidates<Method>(node, baseMethod);
		}

		private IEnumerable<Property> GetAbstractPropertyImplementationCandidates(TypeDefinition node, IProperty baseProperty)
		{
			return GetAbstractMemberImplementationCandidates<Property>(node, baseProperty);
		}

		private IEnumerable<TMember> GetAbstractMemberImplementationCandidates<TMember>(TypeDefinition node, IMember baseEntity)
			where TMember : TypeMember, IExplicitMember		
		{
			var candidates = new List<TMember>();
			foreach (TypeMember m in node.Members)
			{
				var member = m as TMember;
				if (member != null && IsCandidateMemberImplementationFor(baseEntity, m))
					candidates.Add(member);
			}

			// BOO-1031: Move explicitly implemented candidates to top of list so that
			// they're used for resolution before non-explicit ones, if possible.
			// HACK: using IComparer<T> instead of Comparison<T> to workaround
			//       mono bug #399214.
			candidates.Sort(new ExplicitMembersFirstComparer<TMember>());
			return candidates;
		}

		private bool IsCandidateMemberImplementationFor(IMember baseMember, TypeMember candidate)
		{
			return candidate.Name == baseMember.Name
				&& IsCorrectExplicitMemberImplOrNoExplicitMemberAtAll(candidate, baseMember);
		}

		private sealed class ExplicitMembersFirstComparer<T> : IComparer<T>
			where T : IExplicitMember
		{
			public int Compare(T lhs, T rhs)
			{
				if (lhs.ExplicitInfo != null && rhs.ExplicitInfo == null) return -1;
				if (lhs.ExplicitInfo == null && rhs.ExplicitInfo != null) return 1;
				return 0;
			}
		}

		private bool IsCorrectExplicitMemberImplOrNoExplicitMemberAtAll(TypeMember member, IMember entity)
		{
			ExplicitMemberInfo info = ((IExplicitMember)member).ExplicitInfo;
			if (info == null)
				return true;
			if (info.Entity != null)
				return false; // already bound to another member
			return entity.DeclaringType == GetType(info.InterfaceType);
		}
		
		//returns true if a stub has been created, false otherwise.
		//TODO: add entity argument to the method to not need return type?
		bool AbstractMemberNotImplemented(ClassDefinition node, TypeReference baseTypeRef, IMember member)
		{
			if (IsValueType(node))
			{
				Error(CompilerErrorFactory.ValueTypeCantHaveAbstractMember(baseTypeRef, node.FullName, GetAbstractMemberSignature(member)));
				return false;
			}
			if (!node.IsAbstract)
			{
				//BEHAVIOR >= 0.7.7:	(see BOO-789 for details)
				//create a stub for this not implemented member
				//it will raise a NotImplementedException if called at runtime
				TypeMember m = CodeBuilder.CreateStub(node, member);
				CompilerWarning warning = null;
				if (null != m)
				{
					warning = CompilerWarningFactory.AbstractMemberNotImplementedStubCreated(baseTypeRef,
										node.FullName, GetAbstractMemberSignature(member));
					if (m.NodeType != NodeType.Property || null == node.Members[m.Name])
						AddStub(node, m);
				}
				else
				{
					warning = CompilerWarningFactory.AbstractMemberNotImplemented(baseTypeRef,
										node.FullName, GetAbstractMemberSignature(member));
					_newAbstractClasses.AddUnique(node);
				}
				Warnings.Add(warning);
				return (null != m);
			}
			return false;
		}

		private static bool IsValueType(ClassDefinition node)
		{
			return ((IType)node.Entity).IsValueType;
		}

		private string GetAbstractMemberSignature(IMember member)
		{
			IMethod method = member as IMethod;
			return method != null
				? TypeSystemServices.GetSignature(method)
				: member.FullName;
		}

		void ResolveInterfaceMembers(ClassDefinition node,
			TypeReference baseTypeRef,
			IType baseType)
		{
			foreach (IType entity in baseType.GetInterfaces())
				ResolveInterfaceMembers(node, baseTypeRef, entity);
			
			foreach (IMember entity in baseType.GetMembers())
			{
				if (_explicitMembers.Contains(entity))
					continue;
				ResolveAbstractMember(node, baseTypeRef, entity);
			}
		}
		
		void ResolveAbstractMembers(ClassDefinition node, TypeReference baseTypeRef, IType baseType)
		{
			foreach (IEntity member in baseType.GetMembers())
			{
				switch (member.EntityType)
				{
					case EntityType.Method:
					{
						IMethod method = (IMethod)member;
						if (method.IsAbstract)
							ResolveAbstractMethod(node, baseTypeRef, method);
						break;
					}
					
					case EntityType.Property:
					{
						IProperty property = (IProperty)member;
						if (IsAbstract(property))
						{
							ResolveAbstractProperty(node, baseTypeRef, property);
						}
						break;
					}

					case EntityType.Event:
					{
						IEvent ev = (IEvent)member;
						if (ev.IsAbstract)
						{
							ResolveAbstractEvent(node, baseTypeRef, ev);
						}
						break;
					}
					
				}
			}
		}

		private static bool IsAbstract(IProperty property)
		{
			return IsAbstractAccessor(property.GetGetMethod()) ||
			       IsAbstractAccessor(property.GetSetMethod());
		}

		private static bool IsAbstractAccessor(IMethod accessor)
		{
			return null != accessor && accessor.IsAbstract;
		}

		void ResolveAbstractMember(ClassDefinition node, TypeReference baseTypeRef, IMember member)
		{
			switch (member.EntityType)
			{
				case EntityType.Method:
				{
					ResolveAbstractMethod(node, baseTypeRef, (IMethod)member);
					break;
				}
				
				case EntityType.Property:
				{
					ResolveAbstractProperty(node, baseTypeRef, (IProperty)member);
					break;
				}

				case EntityType.Event:
				{
					ResolveAbstractEvent(node, baseTypeRef, (IEvent)member);
					break;
				}
				
				default:
				{
					NotImplemented(baseTypeRef, "abstract member: " + member);
					break;
				}
			}
		}
		
		void ProcessNewAbstractClasses()
		{
			foreach (ClassDefinition node in _newAbstractClasses)
				node.Modifiers |= TypeMemberModifiers.Abstract;
		}

		void AddStub(TypeDefinition node, TypeMember stub)
		{
			node.Members.Add(stub);
		}

		void AssertValidInterfaceImplementation(TypeMember node, IMember baseMember)
		{
			if (!baseMember.DeclaringType.IsInterface)
				return;

			IExplicitMember explicitNode = node as IExplicitMember;
			if (null != explicitNode && null != explicitNode.ExplicitInfo)
				return; //node is an explicit interface impl

			if (node.Visibility != TypeMemberModifiers.Public)
				Errors.Add(CompilerErrorFactory.InterfaceImplementationMustBePublicOrExplicit(node, baseMember));
		}

		public void Reify(TypeMember node)
		{
			Visit(node);
			var method = node as Method;
			if (method != null)
			{
				ReifyMethod(method);
				return;
			}
			var @event = node as Event;
			if (@event != null)
			{
				ReifyEvent(@event);
				return;
			}

			var property = node as Property;
			if (property != null)
				ReifyProperty(property);
		}

		private void ReifyProperty(Property property)
		{
			foreach (var baseProperty in InheritedAbstractMembersOf(property.DeclaringType).OfType<IProperty>())
				if (ResolveAsImplementationOf(baseProperty, property))
					return;
		}

		private void ReifyEvent(Event @event)
		{
			foreach (var baseEvent in InheritedAbstractMembersOf(@event.DeclaringType).OfType<IEvent>())
				if (baseEvent.Name == @event.Name)
				{
					ProcessEventImplementation(@event, baseEvent);
					break;
				}
		}

		private void ReifyMethod(Method method)
		{
			foreach (var baseMethod in InheritedAbstractMembersOf(method.DeclaringType).OfType<IMethod>())
				if (ResolveAsImplementationOf(baseMethod, method))
					return;
		}

		private IEnumerable<IMember> InheritedAbstractMembersOf(TypeDefinition typeDefinition)
		{
			var type = GetType(typeDefinition);
			foreach (var baseType in type.GetInterfaces())
				foreach (IMember member in baseType.GetMembers())
					if (IsAbstract(member))
						yield return member;
			foreach (IMember member in type.BaseType.GetMembers())
				if (IsAbstract(member))
					yield return member;
		}

		private static bool IsAbstract(IMember member)
		{
			switch (member.EntityType)
			{
				case EntityType.Method:
					return ((IMethod) member).IsAbstract;
				case EntityType.Property:
					return IsAbstract((IProperty) member);
				case EntityType.Event:
					return ((IEvent) member).IsAbstract;
				default:
					return false;
			}
		}
	}
}

