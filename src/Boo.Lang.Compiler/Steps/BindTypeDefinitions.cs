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
	using System.Collections;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.TypeSystem;
	
	[Serializable]
	public class BindTypeDefinitions : AbstractTransformerCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}
		
		override public void OnModule(Boo.Lang.Compiler.Ast.Module node)
		{			
			Visit(node.Members);
		}
		
		override public void LeaveStructDefinition(StructDefinition node)
		{
			ClassDefinition cd = new ClassDefinition(node.LexicalInfo);			
			cd.Name = node.Name;
			cd.Attributes = node.Attributes;
			cd.Modifiers = node.Modifiers;
			cd.Members = node.Members;
			cd.BaseTypes = node.BaseTypes;
			cd.BaseTypes.Insert(0, CodeBuilder.CreateTypeReference(TypeSystemServices.ValueTypeType));
			NormalizeVisibility(cd);
			foreach (TypeMember member in cd.Members)
			{
				NormalizeVisibility(member);
			}			 
			cd.Entity = new InternalClass(TypeSystemServices, node);						
			ReplaceCurrentNode(cd);
		}
		
		override public void OnClassDefinition(ClassDefinition node)
		{	
			if (null == node.Entity)
			{				
				node.Entity = new InternalClass(TypeSystemServices, node);				
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
			node.Entity = new InternalInterface(TypeSystemServices, node);
		}
		
		override public void OnEnumDefinition(EnumDefinition node)
		{
			if (null != node.Entity)
			{
				return;
			}
			
			NormalizeVisibility(node);
			node.Entity = new InternalEnum(TypeSystemServices, node);			
			
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
