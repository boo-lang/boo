#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;
using Boo.Lang.Ast;
using Boo.Lang.Compiler;

namespace Boo.Lang.Compiler.Pipeline
{
	public class AstNormalizationStep : AbstractTransformerCompilerStep
	{
		public const string MainModuleMethodName = "__Main__";
		
		public override void Run()
		{
			Switch(CompileUnit.Modules);
		}
		
		public override void OnModule(Module node, ref Module resultingNode)
		{
			ClassDefinition moduleClass = new ClassDefinition();
			
			int removed = 0;			
			TypeMember[] members = node.Members.ToArray();
			for (int i=0; i<members.Length; ++i)
			{
				TypeMember member = members[i];
				if (member.NodeType == NodeType.Method)
				{
					member.Modifiers |= TypeMemberModifiers.Static;
					node.Members.RemoveAt(i-removed);
					moduleClass.Members.Add(member);
					++removed;
				}				
			}		
			
			if (node.Globals.Statements.Count > 0)
			{
				Method method = new Method(node.Globals.LexicalInfo);
				method.ReturnType = CreateBoundTypeReference(BindingManager.VoidTypeBinding);
				method.Body = node.Globals;
				method.Name = MainModuleMethodName;
				method.Modifiers = TypeMemberModifiers.Static | TypeMemberModifiers.Private;				
				moduleClass.Members.Add(method);
				
				node.Globals = null;
				AstAnnotations.SetEntryPoint(CompileUnit, method);
			}
			
			if (moduleClass.Members.Count > 0)
			{
				moduleClass.Members.Add(CreateConstructor(node, TypeMemberModifiers.Private));
			
				moduleClass.Name = BuildModuleClassName(node);
				moduleClass.Modifiers = TypeMemberModifiers.Public |
										TypeMemberModifiers.Final |
										TypeMemberModifiers.Transient;
				node.Members.Add(moduleClass);
				
				AstAnnotations.SetModuleClass(node, moduleClass);
			}
			
			Switch(node.Members);
		}
		
		public override void LeaveClassDefinition(ClassDefinition node, ref ClassDefinition resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
			
			if (!node.HasConstructor)
			{				
				node.Members.Add(CreateConstructor(node, TypeMemberModifiers.Public));
			}
		}
		
		public override void LeaveField(Field node, ref Field resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Protected;
			}
		}
		
		public override void LeaveProperty(Property node, ref Property resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		public override void LeaveMethod(Method node, ref Method resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}
		
		public override void LeaveConstructor(Constructor node, ref Constructor resultingNode)
		{
			if (!node.IsVisibilitySet)
			{
				node.Modifiers |= TypeMemberModifiers.Public;
			}
		}		
		
		public override void LeaveExpressionStatement(ExpressionStatement node, ref Statement resultingNode)
		{
			LeaveStatement(node, ref resultingNode);
		}
		
		public override void LeaveRaiseStatement(RaiseStatement node, ref Statement resultingNode)
		{
			LeaveStatement(node, ref resultingNode);
		}
		
		public override void LeaveReturnStatement(ReturnStatement node, ref Statement resultingNode)
		{
			LeaveStatement(node, ref resultingNode);
		}
		
		public void LeaveStatement(Statement node, ref Statement resultingNode)
		{
			if (null != node.Modifier)
			{
				switch (node.Modifier.Type)
				{
					case StatementModifierType.If:
					{	
						IfStatement stmt = new IfStatement();
						stmt.LexicalInfo = node.Modifier.LexicalInfo;
						stmt.Expression = node.Modifier.Condition;
						stmt.TrueBlock = new Block();						
						stmt.TrueBlock.Statements.Add(node);						
						node.Modifier = null;
						
						resultingNode = stmt;
						
						break;
					}
						
					default:
					{							
						Errors.Add(CompilerErrorFactory.NotImplemented(node, "only if supported"));
						break;
					}
				}
			}
		}
		
		public override void LeaveUnaryExpression(UnaryExpression node, ref Expression resultingNode)
		{
			if (UnaryOperatorType.UnaryNegation == node.Operator)
			{
				if (NodeType.IntegerLiteralExpression == node.Operand.NodeType)
				{
					IntegerLiteralExpression integer = (IntegerLiteralExpression)node.Operand;
					integer.Value *= -1;
					integer.LexicalInfo = node.LexicalInfo;
					resultingNode = integer;
				}
			}
		}
		
		Constructor CreateConstructor(Node lexicalInfoProvider, TypeMemberModifiers modifiers)
		{
			Constructor constructor = new Constructor(lexicalInfoProvider.LexicalInfo);
			constructor.Name = "constructor";
			constructor.Modifiers = modifiers;
			return constructor;
		}
		
		string BuildModuleClassName(Module node)
		{
			string name = node.Name;
			return name.Substring(0, 1).ToUpper() + name.Substring(1) + "Module";
		}
	}
}
