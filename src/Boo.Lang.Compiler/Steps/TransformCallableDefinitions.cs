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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	
	public class TransformCallableDefinitions : AbstractTransformerCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void OnMethod(Method node)
		{
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{
			Visit(node.Members);
		}
		
		override public void OnCallableDefinition(CallableDefinition node)
		{
			if (null == node.ReturnType)
			{
				node.ReturnType = CreateTypeReference(TypeSystemServices.VoidType);
			}
			
			foreach (ParameterDeclaration parameter in node.Parameters)
			{
				if (null == parameter.Type)
				{
					parameter.Type = CreateTypeReference(TypeSystemServices.ObjectType);
				}
			}
			
			ClassDefinition cd = TypeSystemServices.CreateCallableDefinition(node.Name);			
			cd.LexicalInfo = node.LexicalInfo;
			cd.Members.Add(CreateInvokeMethod(node));
			
			ReplaceCurrentNode(cd);			
		}
		
		Method CreateInvokeMethod(CallableDefinition node)
		{
			Method method = new Method("Invoke");
			method.Modifiers = TypeMemberModifiers.Public|TypeMemberModifiers.Virtual;
			method.ImplementationFlags = MethodImplementationFlags.Runtime;
			method.Parameters = node.Parameters;
			method.ReturnType = node.ReturnType;
			return method;
		}
	}
}
