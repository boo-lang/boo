#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Steps
{
	using System;
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
			//node.Members.Add(CreateAbstractProperty(interfaceReference, tag));
			//node.Modifiers |= TypeMemberModifiers.Abstract;
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
			
			node.Members.Add(CreateAbstractMethod(interfaceReference, tag));
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
		
		Method CreateAbstractMethod(Node sourceNode, IMethod baseMethod)
		{
			Method method = new Method(sourceNode.LexicalInfo);
			method.Name = baseMethod.Name;
			method.Modifiers = TypeMemberModifiers.Public | TypeMemberModifiers.Abstract;
			
			IParameter[] parameters = baseMethod.GetParameters();
			for (int i=0; i<parameters.Length; ++i)
			{
				method.Parameters.Add(new ParameterDeclaration("arg" + i, CreateTypeReference(parameters[i].Type)));
			}
			method.ReturnType = CreateTypeReference(baseMethod.ReturnType);
			
			Bind(method, new InternalMethod(TypeSystemServices, method));
			return method;
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
