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

using System;
using System.Collections;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler;

namespace Boo.Lang.Compiler.Steps
{
	public class NormalizeStatementModifiers : AbstractTransformerCompilerStep
	{
		override public void Run()
		{
			Visit(CompileUnit.Modules);
		}
		
		override public void OnModule(Module node)
		{
			Visit(node.Members);
		}
		
		void LeaveTypeDefinition(TypeDefinition node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		override public void LeaveEnumDefinition(EnumDefinition node)
		{
			LeaveTypeDefinition(node);
		}
		
		override public void LeaveInterfaceDefinition(InterfaceDefinition node)
		{
			LeaveTypeDefinition(node);
		}
		
		override public void LeaveClassDefinition(ClassDefinition node)
		{
			LeaveTypeDefinition(node);
			
			if (!node.HasConstructor)
			{				
				node.Members.Add(AstUtil.CreateConstructor(node, TypeMemberModifiers.Public));
			}
		}
		
		override public void LeaveField(Field node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Protected;
			}
		}
		
		override public void LeaveProperty(Property node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		override public void LeaveMethod(Method node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		override public void LeaveConstructor(Constructor node)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}		
		
		override public void LeaveUnpackStatement(UnpackStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveExpressionStatement(ExpressionStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveRaiseStatement(RaiseStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveReturnStatement(ReturnStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveBreakStatement(BreakStatement node)
		{
			LeaveStatement(node);
		}
		
		override public void LeaveContinueStatement(ContinueStatement node)
		{
			LeaveStatement(node);
		}
		
		public void LeaveStatement(Statement node)
		{
			if (null != node.Modifier)
			{
				switch (node.Modifier.Type)
				{
					case StatementModifierType.If:
					{	
						IfStatement stmt = new IfStatement(node.Modifier.LexicalInfo);
						stmt.Condition = node.Modifier.Condition;
						stmt.TrueBlock = new Block();						
						stmt.TrueBlock.Statements.Add(node);						
						node.Modifier = null;
						
						ReplaceCurrentNode(stmt);
						
						break;
					}
					
					case StatementModifierType.Unless:
					{
						UnlessStatement stmt = new UnlessStatement(node.Modifier.LexicalInfo);
						stmt.Condition = node.Modifier.Condition;
						stmt.Block.Statements.Add(node);
						node.Modifier = null;
						
						ReplaceCurrentNode(stmt);
						break;
					}
					
					case StatementModifierType.While:
					{
						WhileStatement stmt = new WhileStatement(node.Modifier.LexicalInfo);
						stmt.Condition = node.Modifier.Condition;
						stmt.Block.Statements.Add(node);
						node.Modifier = null;
						
						ReplaceCurrentNode(stmt);
						break;
					}
						
					default:
					{							
						Errors.Add(CompilerErrorFactory.NotImplemented(node, string.Format("modifier {0} supported", node.Modifier.Type)));
						break;
					}
				}
			}
		}
		
		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			if (UnaryOperatorType.UnaryNegation == node.Operator)
			{
				if (NodeType.IntegerLiteralExpression == node.Operand.NodeType)
				{
					IntegerLiteralExpression integer = (IntegerLiteralExpression)node.Operand;
					integer.Value *= -1;
					integer.LexicalInfo = node.LexicalInfo;
					ReplaceCurrentNode(integer);
				}
			}
		}
	}
}
