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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;

	public class CheckIdentifiers : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		internal static bool IsValidName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return false;
			char c = name[0];
			return char.IsLetter(c) || c=='(' || c=='_' || c == '$';
		}
		
		private void CheckName(Node node, string name)
		{
			if (!IsValidName(name))
			{
				Errors.Add(CompilerErrorFactory.InvalidName(node, name));
			}
		}

		private void CheckParameterUniqueness(Method method)
		{
			Boo.Lang.List parameters = new Boo.Lang.List();
			foreach (ParameterDeclaration parameter in method.Parameters)
			{
				if (parameters.Contains(parameter.Name))
				{
					Errors.Add(
						CompilerErrorFactory.DuplicateParameterName(
							parameter, parameter.Name, GetEntity(method).ToString()));
				}				
				parameters.Add(parameter.Name);
			}
		}

		override public void OnNamespaceDeclaration(NamespaceDeclaration node)
		{
			CheckName(node,node.Name);
		}

		override public void OnReferenceExpression(ReferenceExpression node)
		{
			CheckName(node,node.Name);
		}
		
		override public void OnSimpleTypeReference(SimpleTypeReference node)
		{
			CheckName(node,node.Name);
		}
		
		override public void OnGenericTypeReference(GenericTypeReference node)
		{
			CheckName(node,node.Name);
		}
		
		override public void OnGenericTypeDefinitionReference(GenericTypeDefinitionReference node)
		{
			CheckName(node,node.Name);
		}
		
		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			CheckName(node,node.Name);
		}
		
		override public void LeaveLabelStatement(LabelStatement node)
		{
			CheckName(node,node.Name);
		}
		
		override public void LeaveDeclaration(Declaration node)
		{
			// Special exemption made for anonymous exception handlers
			if(!(node.ParentNode is ExceptionHandler) ||
			   ((node.ParentNode as ExceptionHandler).Flags 
			    & ExceptionHandlerFlags.Anonymous) == ExceptionHandlerFlags.None)
			{
				CheckName(node,node.Name);
			}
		}
		
		override public void LeaveAttribute(Attribute node)
		{
			CheckName(node,node.Name);
		}

		public override void LeaveConstructor(Constructor node)
		{
			CheckParameterUniqueness(node);
		}

		override public void LeaveMethod(Method node)
		{
			CheckParameterUniqueness(node);
			CheckName(node, node.Name);
		}

		override public void LeaveParameterDeclaration(ParameterDeclaration node)
		{
			CheckName(node,node.Name);
		}

		override public void LeaveImport(Import node)
		{
			if (null != node.Alias)
				CheckName(node,node.Alias.Name);
		}
		
		override public void LeaveClassDefinition(ClassDefinition node)
		{
			CheckName(node,node.Name);
		}
		
		override public void LeaveStructDefinition(StructDefinition node)
		{
			CheckName(node,node.Name);
		}

		override public void LeaveInterfaceDefinition(InterfaceDefinition node)
		{
			CheckName(node,node.Name);
		}

		override public void LeaveEnumDefinition(EnumDefinition node)
		{
			CheckName(node,node.Name);

			foreach (EnumMember member in node.Members)
			{
				if (member.Initializer.NodeType != NodeType.IntegerLiteralExpression)
					Errors.Add(
						CompilerErrorFactory.EnumMemberMustBeConstant(member));
			}
		}
		
		override public void LeaveEvent(Event node)
		{
			CheckName(node,node.Name);
		}

		override public void LeaveField(Field node)
		{
			CheckName(node,node.Name);
		}
		
		override public void LeaveProperty(Property node)
		{
			CheckName(node,node.Name);
		}
		
		override public void LeaveEnumMember(EnumMember node)
		{
			CheckName(node,node.Name);
		}
		
	}
}
