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
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;
	
	[Serializable]
	public class BindTypeDefinitions : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module node)
		{			
			Visit(node.Members);
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{	
			if (null == node.Entity)
			{				
				node.Entity = new InternalType(TypeSystemServices, node);				
			}			
			
			NormalizeVisibility(node);
			Visit(node.Members);
		}
		
		override public void OnInterfaceDefinition(InterfaceDefinition node)
		{
			if (null != node.Entity)
			{
				return;
			}			
			
			NormalizeVisibility(node);
			node.Entity = new InternalType(TypeSystemServices, node);
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			if (null != node.Entity)
			{
				return;
			}
			
			NormalizeVisibility(node);
			node.Entity = new EnumType(TypeSystemServices, node);			
			
			long lastValue = 0;
			foreach (EnumMember member in node.Members)
			{
				if (null == member.Initializer)
				{
					member.Initializer = new IntegerLiteralExpression(lastValue);
				}
				lastValue = member.Initializer.Value + 1;
				
				if (null == member.Entity)
				{
					member.Entity = new InternalEnumMember(TypeSystemServices, member);
				}
			}
		}
		
		void NormalizeVisibility(TypeMember node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		override public void OnMethod(Method method)
		{
		}
		
		override public void OnProperty(Property property)
		{
		}
		
		override public void OnField(Field field)
		{
		}
	}
}
