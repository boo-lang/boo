#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Compiler.Steps
{
	using System;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;
	
	[Serializable]
	public class BindBaseTypes : AbstractNamespaceSensitiveVisitorCompilerStep
	{	
		public BindBaseTypes()
		{
		}
		
		override public void Run()
		{			
			NameResolutionService.Reset();
			Visit(CompileUnit.Modules);
		}
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{
			EnterNamespace((INamespace)GetEntity(module));
			Visit(module.Members);
			LeaveNamespace();
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{			
			Visit(node.Members);
			ResolveBaseTypes(new Boo.Lang.List(), node);
			CheckBaseTypes(node);
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			ResolveBaseTypes(new Boo.Lang.List(), node);
			CheckInterfaceBaseTypes(node);
		}
		
		void CheckBaseTypes(ClassDefinition node)
		{
			IType baseClass = null;
			foreach (TypeReference baseType in node.BaseTypes)
			{				
				IType baseInfo = GetType(baseType);
				if (baseInfo.IsClass)
				{
					if (null != baseClass)
					{
						Error(
						    CompilerErrorFactory.ClassAlreadyHasBaseType(baseType,
								node.Name,
								baseClass.FullName)
							); 
					}
					else
					{
						baseClass = baseInfo;
					}
				}
			}
			
			if (null == baseClass)
			{
				node.BaseTypes.Insert(0, CreateTypeReference(TypeSystemServices.ObjectType)	);
			}
		}
		
		void CheckInterfaceBaseTypes(InterfaceDefinition node)
		{
			foreach (TypeReference baseType in node.BaseTypes)
			{
				IType tag = GetType(baseType);
				if (!tag.IsInterface)
				{
					Error(CompilerErrorFactory.InterfaceCanOnlyInheritFromInterface(baseType, node.FullName, tag.FullName));
				}
			}
		}
		
		void ResolveBaseTypes(Boo.Lang.List visited, TypeDefinition node)
		{
			visited.Add(node);
			
			int removed = 0;
			int index = 0;
			foreach (SimpleTypeReference type in node.BaseTypes.ToArray())
			{                            
				NameResolutionService.ResolveSimpleTypeReference(type);
				IType entity = type.Entity as IType;
				
				if (null != entity)
				{
					InternalType internalType = entity as InternalType;
					if (null != internalType)
					{
						if (visited.Contains(internalType.TypeDefinition))
						{							
							Error(CompilerErrorFactory.InheritanceCycle(type, internalType.FullName));
							node.BaseTypes.RemoveAt(index-removed);
							++removed;
						}
						else
						{
							ResolveBaseTypes(visited, internalType.TypeDefinition);
						}
					}
				}
				
				++index;
			}
		}
	}
}
