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
    internal enum InheritsImplementationStyle
    {
        None,
        IsVirtual,
        IsLocal,
        IsExternal
    }

	public class ProcessInheritedAbstractMembers : AbstractVisitorCompilerStep, ITypeMemberReifier
	{
		private List<TypeDefinition> _newAbstractClasses;
		private Set<IEntity> _explicitMembers;

        public override void Run()
		{	
			_newAbstractClasses = new List<TypeDefinition>();
			Visit(CompileUnit.Modules);
			ProcessNewAbstractClasses();
		}

        public override void Dispose()
		{
			_newAbstractClasses = null;
			base.Dispose();
		}

        public override void OnProperty(Property node)
		{
			if (node.IsAbstract && null == node.Type)
				node.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType);

			var explicitInfo = node.ExplicitInfo;
			if (null != explicitInfo)
			{
				Visit(explicitInfo);
				if (null != explicitInfo.Entity)
					ProcessPropertyImplementation(node, (IProperty) explicitInfo.Entity);
			}
		}

        public override void OnMethod(Method node)
		{
			if (node.IsAbstract && null == node.ReturnType)
				node.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);

			var explicitInfo = node.ExplicitInfo;
			if (null != explicitInfo)
			{
				Visit(explicitInfo);
				if (null != explicitInfo.Entity)
					ProcessMethodImplementation(node, (IMethod) explicitInfo.Entity);
			}
		}

        public override void OnExplicitMemberInfo(ExplicitMemberInfo node)
		{
			var member = (TypeMember)node.ParentNode;
			if (!CheckExplicitMemberValidity((IExplicitMember)member))
				return;

			var interfaceType = GetEntity(node.InterfaceType);
			var baseMember = FindBaseMemberOf((IMember) member.Entity, interfaceType);
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
			var node = (Node) member;
			var explicitMember = (IMember)GetEntity(node);
			var declaringType = explicitMember.DeclaringType;
			if (!declaringType.IsClass)
			{
				Error(CompilerErrorFactory.InvalidTypeForExplicitMember(node, declaringType));
				return false;
			}

			var targetInterface = GetType(member.ExplicitInfo.InterfaceType);
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

		public override void OnInterfaceDefinition(InterfaceDefinition node)
		{
			if (WasVisited(node))
				return;
			MarkVisited(node);

			base.OnInterfaceDefinition(node);
		}

		public override void OnClassDefinition(ClassDefinition node)
		{
			if (WasVisited(node))
				return;
			MarkVisited(node);

			base.OnClassDefinition(node);

			ProcessBaseTypes(node, GetType(node), null);
		}

		/// <summary>
		/// This method scans all inherited types (classes and interfaces) and checks if abstract members of base types
		/// were implemented in current class definition. If abstract member is not implemented then stub is created.
		/// Stubs are created in two cases:
		/// 1) if any member of base interfaces is not implemented 
		/// 2) if any member of base types is not implemented and current class definition is not abstract.
		/// </summary>
		/// <param name="originalNode"></param>
		/// <param name="currentType"></param>
		/// <param name="rootBaseType"></param>
		private void ProcessBaseTypes(ClassDefinition originalNode, IType currentType, TypeReference rootBaseType)
		{
			//First method call iterates through BaseTypes of ClassDefinition.
			//Following recursive calls work with IType. Using IType is necessary to process external types (no ClassDefinition).
			if (rootBaseType == null)
			{
				//Executing method first time.
				//Checking all interfaces and base type
				_explicitMembers = null;
				foreach (var baseTypeRef in originalNode.BaseTypes)
				{
					var baseType = GetType(baseTypeRef);
					EnsureRelatedNodeWasVisited(originalNode, baseType);

					if (baseType.IsInterface)
					{
						if (_explicitMembers == null) _explicitMembers = ExplicitlyImplementedMembersOn(originalNode);
						ResolveInterfaceMembers(originalNode, baseType, baseTypeRef);
					}
					//Do not resolve abstract members if class is abstract
					else if (!IsAbstract(GetType(originalNode)) && IsAbstract(baseType))
						ResolveAbstractMembers(originalNode, baseType, baseTypeRef);
				}
			}
			else
			{
				//This is recursive call. Checking base type only. 
				//Don't need to check interfaces because they must be already implemented in the base type.
				if (currentType.BaseType != null && IsAbstract(currentType.BaseType))
					ResolveAbstractMembers(originalNode, currentType.BaseType, rootBaseType);
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

        private IMember GetBaseImplememtation(ClassDefinition node, IMember abstractMember)
        {
            foreach (var baseTypeRef in node.BaseTypes)
            {
                IType type = GetType(baseTypeRef);
                if (type.IsInterface)
                    continue;

                IMember implementation = FindBaseMemberOf(abstractMember, type);
                if (implementation != null)
                    return implementation;
            }
            return null;
        }

		/// <summary>
		/// This function checks for inheriting implementations from EXTERNAL classes only.
		/// </summary>
        private InheritsImplementationStyle CheckInheritsImplementation(ClassDefinition node, IMember abstractMember)
		{
            IMember implementation = GetBaseImplememtation(node, abstractMember);
            if (implementation != null && !IsAbstract(implementation))
			{
                if (IsVirtualImplementation(implementation, abstractMember))
			        return InheritsImplementationStyle.IsVirtual;
                if (abstractMember is IExternalEntity)
                    return InheritsImplementationStyle.IsExternal;
                return InheritsImplementationStyle.IsLocal;
			}
            return InheritsImplementationStyle.None;
		}

		private IMember FindBaseMemberOf(IMember member, IType inType)
		{
			if (member.DeclaringType == inType) return null;

			foreach (var candidate in ImplementationCandidatesFor(member, inType))
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

        private static bool IsVirtualImplementation(IEntity member, IEntity candidate)
        {
            switch (member.EntityType)
            {
                case EntityType.Method:
                    return ((IMethod) candidate).IsVirtual;
                case EntityType.Event:
                    return true;
                case EntityType.Property:
                    if (CheckVirtualPropImpl((IProperty)candidate, (IProperty)member))
                        return true;
                    break;
            }
            return false;
        }

	    private static bool CheckVirtualPropImpl(IProperty candidate, IProperty member)
	    {
            if (candidate.GetGetMethod() != null && !member.GetGetMethod().IsVirtual)
	            return false;
            if (candidate.GetSetMethod() != null && !member.GetSetMethod().IsVirtual)
                return false;
	        return true;
	    }

	    private IEnumerable<IMember> ImplementationCandidatesFor(IMember abstractMember, IType inBaseType)
		{
			while (inBaseType != null)
			{
				foreach (var candidate in inBaseType.GetMembers())
				{
					if (candidate.EntityType != abstractMember.EntityType)
						continue;

					if (candidate.EntityType == EntityType.Field)
						continue;

					string candidateName = abstractMember.DeclaringType.IsInterface && abstractMember.EntityType != EntityType.Method
					                       	? SimpleNameOf(candidate)
					                       	: candidate.Name;
					if (candidateName == abstractMember.Name)
						yield return (IMember)candidate;
				}
				inBaseType = inBaseType.BaseType;
			}
		}

		private string SimpleNameOf(IEntity candidate)
		{
			//candidate.Name == "CopyTo"
			//vs
			//candidate.Name == "System.ICollection.CopyTo"
			var temp = candidate.FullName.Split('.');
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

			if (HasGetter(target) && !HasGetter(impl)) return false;
			if (HasSetter(target) && !HasSetter(impl)) return false;

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

			var internalType = type as AbstractInternalType;
			if (null != internalType)
				return _newAbstractClasses.Contains(internalType.TypeDefinition);
			return false;
		}

		void ResolveAbstractProperty(ClassDefinition node, IProperty baseProperty, TypeReference rootBaseType)
		{			
			foreach (var p in GetAbstractPropertyImplementationCandidates(node, baseProperty))
			{
				if (!ResolveAsImplementationOf(baseProperty, p))
					continue;

				//fully-implemented?
				if (!HasGetter(baseProperty) || (HasGetter(baseProperty) && null != p.Getter))
					if (!HasSetter(baseProperty) || (HasSetter(baseProperty) && null != p.Setter))
						return;
			}

		    var style = CheckInheritsImplementation(node, baseProperty);
		    switch (style)
		    {
		        case InheritsImplementationStyle.None:
		            AbstractMemberNotImplemented(node, rootBaseType, baseProperty);;
		            break;
		        case InheritsImplementationStyle.IsVirtual:
		            break;
		        case InheritsImplementationStyle.IsLocal:
		        case InheritsImplementationStyle.IsExternal:
                    ResolveNonVirtualPropertyImplementation(node, rootBaseType, baseProperty, style);
		            break;
		    }
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
				Error(CompilerErrorFactory.ConflictWithInheritedMember(p, GetEntity(p), baseProperty));

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

			if (CheckInheritsImplementation(node, baseEvent) != InheritsImplementationStyle.None)
				return;

			TypeMember conflicting;
			if (null != (conflicting = node.Members[baseEvent.Name]))
			{
				//we've got a non-resolved conflicting member
				Error(CompilerErrorFactory.ConflictWithInheritedMember(conflicting, (IMember)GetEntity(conflicting), baseEvent));
				return;
			}
			AddStub(node, CodeBuilder.CreateAbstractEvent(baseTypeRef.LexicalInfo, baseEvent));
			AbstractMemberNotImplemented(node, baseTypeRef, baseEvent);
			
		}

		private void ProcessEventImplementation(Event ev, IEvent baseEvent)
		{
			MakeVirtualFinal(ev.Add);
			MakeVirtualFinal(ev.Remove);
			MakeVirtualFinal(ev.Raise);
			AssertValidInterfaceImplementation(ev, baseEvent);
			Context.TraceInfo("{0}: Event {1} implements {2}", ev.LexicalInfo, ev, baseEvent);
		}

		private static void MakeVirtualFinal(Method method)
		{
			if (method == null) return;
			method.Modifiers |= TypeMemberModifiers.Final | TypeMemberModifiers.Virtual;
		}

		void ResolveAbstractMethod(ClassDefinition node, IMethod baseAbstractMethod, TypeReference rootBaseType)
		{
			if (baseAbstractMethod.IsSpecialName)
				return;
			
			foreach (Method method in GetAbstractMethodImplementationCandidates(node, baseAbstractMethod))
				if (ResolveAsImplementationOf(baseAbstractMethod, method))
					return;

		    var style = CheckInheritsImplementation(node, baseAbstractMethod);
		    switch (style)
		    {
		        case InheritsImplementationStyle.None:
		            break;
		        case InheritsImplementationStyle.IsVirtual:
		            return;
		        case InheritsImplementationStyle.IsLocal:
		        case InheritsImplementationStyle.IsExternal:
		            ResolveNonVirtualMethodImplementation(node, rootBaseType, baseAbstractMethod, style);
		            break;
		    }

			if (!AbstractMemberNotImplemented(node, rootBaseType, baseAbstractMethod))
			{
				//BEHAVIOR < 0.7.7: no stub, mark class as abstract
				AddStub(node, CodeBuilder.CreateAbstractMethod(rootBaseType.LexicalInfo, baseAbstractMethod));
			}
		}

	    private void ResolveNonVirtualPropertyImplementation(ClassDefinition node, TypeReference rootBaseType,
	        IProperty baseAbstractProperty, InheritsImplementationStyle style)
	    {
            if (!((IType)rootBaseType.Entity).IsInterface)
                return;
            var impl = (IProperty) GetBaseImplememtation(node, baseAbstractProperty);
            var abstractAccessor = baseAbstractProperty.GetGetMethod();
            if (abstractAccessor != null)
            {
                var getter = impl.GetGetMethod();
                if (!getter.IsVirtual)
                    ResolveNonVirtualMethodImplementation(node, getter, style);
            }
            abstractAccessor = baseAbstractProperty.GetSetMethod();
            if (abstractAccessor != null)
            {
                var setter = impl.GetGetMethod();
                if (!setter.IsVirtual)
                    ResolveNonVirtualMethodImplementation(node, setter, style);
            }
	    }

	    private void ResolveNonVirtualMethodImplementation(ClassDefinition node, TypeReference rootBaseType,
            IMethod baseAbstractMethod, InheritsImplementationStyle style)
	    {
            if (!((IType)rootBaseType.Entity).IsInterface)
                return;
            var impl = (IMethod)GetBaseImplememtation(node, baseAbstractMethod);
            ResolveNonVirtualMethodImplementation(node, impl, style);
        }

        private void ResolveNonVirtualMethodImplementation(ClassDefinition node, IMethod resolvedMethod,
            InheritsImplementationStyle style)
        {
            if (style == InheritsImplementationStyle.IsLocal)
            {
                var method = ((InternalMethod)resolvedMethod).Method;
                method.Modifiers |= TypeMemberModifiers.Virtual | TypeMemberModifiers.Final;
            }
            else
            {
                var coverMethod = CodeBuilder.CreateMethodFromPrototype(resolvedMethod,
                    TypeMemberModifiers.Public | TypeMemberModifiers.Virtual | TypeMemberModifiers.Final | TypeMemberModifiers.New);
                var superCall = CodeBuilder.CreateSuperMethodInvocation(resolvedMethod);
                foreach (var param in resolvedMethod.GetParameters())
                    superCall.Arguments.Add(CodeBuilder.CreateReference(param));
                if (resolvedMethod.ReturnType == TypeSystemServices.VoidType)
                    coverMethod.Body.Add(superCall);
                else coverMethod.Body.Add(new ReturnStatement(superCall));
                node.Members.Add(coverMethod);
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
				Error(CompilerErrorFactory.ConflictWithInheritedMember(method, methodEntity, baseMethod));
		}

		private void TraceImplements(TypeMember member, IEntity baseMember)
		{
			Context.TraceInfo("{0}: Member {1} implements {2}", member.LexicalInfo, member, baseMember);
		}

		private static bool IsUnknown(IType type)
		{
			return TypeSystemServices.IsUnknown(type);
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
				Error(CompilerErrorFactory.ValueTypeCantHaveAbstractMember(baseTypeRef, GetType(node), member));
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
					warning = CompilerWarningFactory.AbstractMemberNotImplementedStubCreated(baseTypeRef, GetType(node), member);
					if (m.NodeType != NodeType.Property || null == node.Members[m.Name])
						AddStub(node, m);
				}
				else
				{
					warning = CompilerWarningFactory.AbstractMemberNotImplemented(baseTypeRef, GetType(node), member);
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

		void ResolveInterfaceMembers(ClassDefinition node, IType baseType, TypeReference rootBaseType)
		{
			foreach (IType entity in baseType.GetInterfaces())
				ResolveInterfaceMembers(node, entity, rootBaseType);
			
			foreach (IMember entity in baseType.GetMembers())
			{
				if (_explicitMembers.Contains(entity))
					continue;
				ResolveAbstractMember(node, entity, rootBaseType);
			}
		}
		
		void ResolveAbstractMembers(ClassDefinition node, IType baseType, TypeReference rootBaseType)
		{
			foreach (IEntity member in baseType.GetMembers())
			{
				switch (member.EntityType)
				{
					case EntityType.Method:
					{
						var method = (IMethod)member;
						if (method.IsAbstract)
							ResolveAbstractMethod(node, method, rootBaseType);
						break;
					}
					
					case EntityType.Property:
					{
						var property = (IProperty)member;
						if (IsAbstract(property))
							ResolveAbstractProperty(node, property, rootBaseType);
						break;
					}

					case EntityType.Event:
					{
						var ev = (IEvent)member;
						if (ev.IsAbstract)
							ResolveAbstractEvent(node, rootBaseType, ev);
						break;
					}
				}
			}

			ProcessBaseTypes(node, baseType, rootBaseType);
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

		void ResolveAbstractMember(ClassDefinition node, IMember member, TypeReference rootBaseType)
		{
			switch (member.EntityType)
			{
				case EntityType.Method:
				{
					ResolveAbstractMethod(node, (IMethod)member, rootBaseType);
					break;
				}
				
				case EntityType.Property:
				{
					ResolveAbstractProperty(node, (IProperty)member, rootBaseType);
					break;
				}

				case EntityType.Event:
				{
					ResolveAbstractEvent(node, rootBaseType, (IEvent)member);
					break;
				}
				
				default:
				{
					NotImplemented(rootBaseType, "abstract member: " + member);
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

		public TypeMember Reify(TypeMember node)
		{
			Visit(node);
			var method = node as Method;
			if (method != null)
			{
				ReifyMethod(method);
				return node;
			}
			var @event = node as Event;
			if (@event != null)
			{
				ReifyEvent(@event);
				return node;
			}

			var property = node as Property;
			if (property != null)
				ReifyProperty(property);

			return node;
		}

		private void ReifyProperty(Property property)
		{
			foreach (var baseProperty in InheritedAbstractMembersOf(property.DeclaringType).OfType<IProperty>())
				if (IsCandidateMemberImplementationFor(baseProperty, property) && ResolveAsImplementationOf(baseProperty, property))
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
				if (IsCandidateMemberImplementationFor(baseMethod, method) && ResolveAsImplementationOf(baseMethod, method))
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

