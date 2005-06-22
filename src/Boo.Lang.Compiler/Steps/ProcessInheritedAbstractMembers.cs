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

namespace Boo.Lang.Compiler.Steps
{
	using System.Diagnostics;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	public class ProcessInheritedAbstractMembers : AbstractVisitorCompilerStep
	{
		private Boo.Lang.List _newAbstractClasses;

		public ProcessInheritedAbstractMembers()
		{
		}

		override public void Run()
		{	
			_newAbstractClasses = new List();
			Visit(CompileUnit.Modules);
			ProcessNewAbstractClasses();
		}

		override public void Dispose()
		{
			_newAbstractClasses = null;
			base.Dispose();
		}

		override public void OnProperty(Property node)
		{
			if (node.IsAbstract)
			{
				if (null == node.Type)
				{
					node.Type = CodeBuilder.CreateTypeReference(TypeSystemServices.ObjectType);
				}
			}
		}

		override public void OnMethod(Method node)
		{
			if (node.IsAbstract)
			{
				if (null == node.ReturnType)
				{
					node.ReturnType = CodeBuilder.CreateTypeReference(TypeSystemServices.VoidType);
				}
			}
		}

		override public void LeaveInterfaceDefinition(InterfaceDefinition node)
		{
			MarkVisited(node);
		}

		override public void LeaveClassDefinition(ClassDefinition node)
		{
			MarkVisited(node);
			foreach (TypeReference baseTypeRef in node.BaseTypes)
			{	
				IType baseType = GetType(baseTypeRef);
				EnsureRelatedNodeWasVisited(baseType);
				if (baseType.IsInterface)
				{
					ResolveInterfaceMembers(node, baseTypeRef, baseType);
				}
				else
				{
					if (IsAbstract(baseType))
					{
						ResolveAbstractMembers(node, baseTypeRef, baseType);
					}
				}
			}
		}

		private void MarkVisited(TypeDefinition node)
		{
			node[this] = true;
		}

		private void EnsureRelatedNodeWasVisited(IType type)
		{
			AbstractInternalType internalType = type as AbstractInternalType;
			if (null != internalType)
			{
				TypeDefinition node = internalType.TypeDefinition;
				if (!node.ContainsAnnotation(this))
				{
					Visit(node);
				}
			}
		}

		bool IsAbstract(IType type)
		{
			if (type.IsAbstract)
			{
				return true;
			}
			
			AbstractInternalType internalType = type as AbstractInternalType;
			if (null != internalType)
			{
				return _newAbstractClasses.Contains(internalType.TypeDefinition);
			}
			return false;
		}

		IProperty GetPropertyEntity(TypeMember member)
		{
			return (IProperty)GetEntity(member);
		}
		
		void ResolveClassAbstractProperty(ClassDefinition node,
			TypeReference baseTypeRef,
			IProperty entity)
		{
			TypeMember member = node.Members[entity.Name];
			if (null != member && NodeType.Property == member.NodeType &&
				TypeSystemServices.CheckOverrideSignature(entity.GetParameters(), GetPropertyEntity(member).GetParameters()))
			{
				Property p = (Property)member;
				if (null != p.Getter)
				{
					p.Getter.Modifiers |= TypeMemberModifiers.Virtual;
				}
				if (null != p.Setter)
				{
					p.Setter.Modifiers |= TypeMemberModifiers.Virtual;
				}
			}
			else
			{
				if (null == member)
				{
					node.Members.Add(CreateAbstractProperty(baseTypeRef, entity));
					AbstractMemberNotImplemented(node, baseTypeRef, entity);
				}
				else
				{
					NotImplemented(baseTypeRef, "member name conflict: " + entity);
				}
			}
		}
		
		Property CreateAbstractProperty(TypeReference reference, IProperty property)
		{
			Debug.Assert(0 == property.GetParameters().Length);
			Property p = CodeBuilder.CreateProperty(property.Name, property.Type);
			p.Modifiers |= TypeMemberModifiers.Abstract;
			
			IMethod getter = property.GetGetMethod();
			if (getter != null)
			{
				p.Getter = CodeBuilder.CreateAbstractMethod(reference.LexicalInfo, getter);
			}
			
			IMethod setter = property.GetSetMethod();
			if (setter != null)
			{
				p.Setter = CodeBuilder.CreateAbstractMethod(reference.LexicalInfo, setter);
			}
			return p;
		}
		
		void ResolveAbstractMethod(ClassDefinition node,
			TypeReference baseTypeRef,
			IMethod entity)
		{
			if (entity.IsSpecialName)
			{
				return;
			}
			
			foreach (TypeMember member in node.Members)
			{
				if (entity.Name == member.Name &&
					NodeType.Method == member.NodeType &&
					(
					((Method)member).ExplicitInfo == null ||
					GetType(baseTypeRef) == GetType(((Method)member).ExplicitInfo.InterfaceType) ||
					GetType(baseTypeRef).IsSubclassOf(GetType(((Method)member).ExplicitInfo.InterfaceType))
					))
				{
					Method method = (Method)member;
					if (TypeSystemServices.CheckOverrideSignature((IMethod)GetEntity(method), entity))
					{
						// TODO: check return type here
						if (IsUnknown(method.ReturnType))
						{
							method.ReturnType = CodeBuilder.CreateTypeReference(entity.ReturnType);
						}
						if (!method.IsOverride && !method.IsVirtual)
						{
							method.Modifiers |= TypeMemberModifiers.Virtual;
						}
						
						_context.TraceInfo("{0}: Method {1} implements {2}", method.LexicalInfo, method, entity);
						return;
					}
				}
			}
			
			node.Members.Add(CodeBuilder.CreateAbstractMethod(baseTypeRef.LexicalInfo, entity));
			AbstractMemberNotImplemented(node, baseTypeRef, entity);
		}

		bool IsUnknown(TypeReference typeRef)
		{
			return Unknown.Default == typeRef.Entity;
		}
		
		void AbstractMemberNotImplemented(ClassDefinition node, TypeReference baseTypeRef, IMember member)
		{
			if (!node.IsAbstract)
			{
				Warnings.Add(
					CompilerWarningFactory.AbstractMemberNotImplemented(baseTypeRef,
					node.FullName, member.FullName));
				_newAbstractClasses.AddUnique(node);
			}
		}
		
		void ResolveInterfaceMembers(ClassDefinition node,
			TypeReference baseTypeRef,
			IType baseType)
		{
			foreach (IType entity in baseType.GetInterfaces())
			{
				ResolveInterfaceMembers(node, baseTypeRef, entity);
			}
			
			foreach (IMember entity in baseType.GetMembers())
			{
				ResolveAbstractMember(node, baseTypeRef, entity);
			}
		}
		
		void ResolveAbstractMembers(ClassDefinition node,
			TypeReference baseTypeRef,
			IType baseType)
		{
			foreach (IEntity member in baseType.GetMembers())
			{
				switch (member.EntityType)
				{
					case EntityType.Method:
					{
						IMethod method = (IMethod)member;
						if (method.IsAbstract)
						{
							ResolveAbstractMethod(node, baseTypeRef, method);
						}
						break;
					}
					
					case EntityType.Property:
					{
						IProperty property = (IProperty)member;
						if (IsAbstractAccessor(property.GetGetMethod()) ||
							IsAbstractAccessor(property.GetSetMethod()))
						{
							ResolveClassAbstractProperty(node, baseTypeRef, property);
						}
						break;
					}
				}
			}
		}
		
		bool IsAbstractAccessor(IMethod accessor)
		{
			if (null != accessor)
			{
				return accessor.IsAbstract;
			}
			return false;
		}
		
		void ResolveAbstractMember(ClassDefinition node,
			TypeReference baseTypeRef,
			IMember member)
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
					ResolveClassAbstractProperty(node, baseTypeRef, (IProperty)member);
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
			{
				node.Modifiers |= TypeMemberModifiers.Abstract;
			}
		}
	}
}
