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

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using System.Diagnostics;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class CheckInterfaceImplementations : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			Visit(node.Members, NodeType.ClassDefinition);
			
			foreach (TypeReference baseType in node.BaseTypes)
			{
				IType tag = GetType(baseType);
				if (tag.IsInterface)
				{
					ResolveClassInterfaceMembers(node, baseType, tag);
				}
			}
		}
		
		void ResolveClassInterfaceProperty(ClassDefinition node,
											Boo.Lang.Compiler.Ast.TypeReference interfaceReference,
											IProperty tag)
		{
			TypeMember member = node.Members[tag.Name];
			if (null != member && NodeType.Property == member.NodeType)
			{
				if (tag.Type == GetType(member))
				{
					if (CheckPropertyAccessors(tag, (IProperty)GetEntity(member)))
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
				}
			}
			else
			{
				if (null == member)
				{
					node.Members.Add(CreateAbstractProperty(interfaceReference, tag));
					node.Modifiers |= TypeMemberModifiers.Abstract;
					Warnings.Add(
						CompilerWarningFactory.AbstractMemberNotImplemented(interfaceReference,
																					node.FullName, tag.FullName)); 
				}
				else
				{
					// error
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
		
		void ResolveClassInterfaceMethod(ClassDefinition node,
										TypeReference interfaceReference,
										IMethod tag)
		{			
			if (tag.IsSpecialName)
			{
				return;
			}
			
			TypeMember member = node.Members[tag.Name];
			if (null != member && NodeType.Method == member.NodeType)
			{							
				Method method = (Method)member;
				if (TypeSystemServices.CheckOverrideSignature((IMethod)GetEntity(method), tag))
				{
					// TODO: check return type here
					if (!method.IsOverride && !method.IsVirtual)
					{
						method.Modifiers |= TypeMemberModifiers.Virtual;
					}
					
					_context.TraceInfo("{0}: Method {1} implements {2}", method.LexicalInfo, method, tag);
					return;
				}
			}
			
			node.Members.Add(CodeBuilder.CreateAbstractMethod(interfaceReference.LexicalInfo, tag));
			node.Modifiers |= TypeMemberModifiers.Abstract;
		}
		
		void ResolveClassInterfaceMembers(ClassDefinition node,
											TypeReference interfaceReference,
											IType interfaceInfo)
		{			
			foreach (IType tag in interfaceInfo.GetInterfaces())
			{
				ResolveClassInterfaceMembers(node, interfaceReference, tag);
			}
			
			foreach (IMember tag in interfaceInfo.GetMembers())
			{
				switch (tag.EntityType)
				{
					case EntityType.Method:
					{
						ResolveClassInterfaceMethod(node, interfaceReference, (IMethod)tag);
						break;
					}
					
					case EntityType.Property:
					{
						ResolveClassInterfaceProperty(node, interfaceReference, (IProperty)tag);
						break;
					}
					
					default:
					{
						NotImplemented(interfaceReference, "interface member: " + tag);
						break;
					}
				}
			}
		}		
		
		bool CheckPropertyAccessors(IProperty expected, IProperty actual)
		{			
			return CheckPropertyAccessor(expected.GetGetMethod(), actual.GetGetMethod()) &&
				CheckPropertyAccessor(expected.GetSetMethod(), actual.GetSetMethod());
		}
		
		bool CheckPropertyAccessor(IMethod expected, IMethod actual)
		{			
			if (null != expected)
			{								
				if (null == actual ||
					!TypeSystemServices.CheckOverrideSignature(expected, actual))
				{
					return false;
				}
			}
			return true;
		}
		
		override public void OnModule(Module node)
		{
			Visit(node.Members, NodeType.ClassDefinition);
		}
	}
}
