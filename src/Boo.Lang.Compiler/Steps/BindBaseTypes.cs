#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
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
			Accept(CompileUnit.Modules);
		}
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module module)
		{
			EnterNamespace((INamespace)GetTag(module));
			Accept(module.Members);
			LeaveNamespace();
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{			
			Accept(node.Members);
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
				TypeReferenceTag tag = type.Tag as TypeSystem.TypeReferenceTag;
				
				if (null != tag)
				{
					InternalType internalType = tag.Type as InternalType;
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
