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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class HuntDucks : ProcessMethodBodies
	{
		protected IType _runtimeServices;
		protected IMethod RuntimeServices_Invoke;
		protected IMethod RuntimeServices_SetProperty;
		protected IMethod RuntimeServices_GetProperty;
		
		override protected void InitializeMemberCache()
		{
			base.InitializeMemberCache();
			_runtimeServices = TypeSystemServices.Map(typeof(Boo.Lang.RuntimeServices));
			RuntimeServices_Invoke = ResolveMethod(_runtimeServices, "Invoke");
			RuntimeServices_SetProperty = ResolveMethod(_runtimeServices, "SetProperty");
			RuntimeServices_GetProperty = ResolveMethod(_runtimeServices, "GetProperty");
		}
		
		override protected void ProcessBuiltinInvocation(BuiltinFunction function, MethodInvocationExpression node)
		{
			if (IsQuackBuiltin(function))
			{
				ProcessQuackInvocation(node);				
			}	
			else
			{
				base.ProcessBuiltinInvocation(function, node);
			}
		}
		
		override protected void ProcessAssignment(BinaryExpression node)
		{
			if (IsQuackBuiltin(node.Left.Entity))
			{
				ProcessQuackPropertySet(node);
			}
			else
			{
				base.ProcessAssignment(node);
			}
		}
		
		override protected void ProcessMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (TypeSystemServices.DuckType == node.Target.ExpressionType)
			{
				if (AstUtil.IsTargetOfMethodInvocation(node) || 
					AstUtil.IsLhsOfAssignment(node))
				{
					Bind(node, BuiltinFunction.Quack);
				}
				else
				{
					ProcessQuackPropertyGet(node);
				}
			}
			else
			{
				base.ProcessMemberReferenceExpression(node);
			}
		}
		
		bool IsQuackBuiltin(IEntity entity)
		{
			return BuiltinFunction.Quack == entity;
		}
		
		void ProcessQuackPropertyGet(MemberReferenceExpression node)
		{
			MethodInvocationExpression mie = CreateMethodInvocation(
												RuntimeServices_GetProperty,
												node.Target,
												CreateStringLiteral(node.Name),
												CreateNullLiteral());
			BindExpressionType(mie, TypeSystemServices.DuckType);
			node.ParentNode.Replace(node, mie);
		}
		
		void ProcessQuackPropertySet(BinaryExpression node)
		{
			MemberReferenceExpression target = (MemberReferenceExpression)node.Left;
			
			MethodInvocationExpression mie = CreateMethodInvocation(
												RuntimeServices_SetProperty,
												target.Target,
												CreateStringLiteral(target.Name),
												node.Right);
			
			BindExpressionType(mie, TypeSystemServices.DuckType);
			node.ParentNode.Replace(node, mie);
		}
		
		void ProcessQuackInvocation(MethodInvocationExpression node)
		{
			MemberReferenceExpression target = (MemberReferenceExpression)node.Target;
			node.Target = CreateMemberReference(
								CreateReference(node.LexicalInfo, _runtimeServices),
								RuntimeServices_Invoke);
			
			Expression args = CreateObjectArray(node.Arguments);
			node.Arguments.Clear();
			node.Arguments.Add(target.Target);
			node.Arguments.Add(CreateStringLiteral(target.Name));
			node.Arguments.Add(args);
			BindExpressionType(node, TypeSystemServices.DuckType);
		}
		
		NullLiteralExpression CreateNullLiteral()
		{
			NullLiteralExpression expression = new NullLiteralExpression();
			BindExpressionType(expression, Null.Default);
			return expression;
		}
	}
}
