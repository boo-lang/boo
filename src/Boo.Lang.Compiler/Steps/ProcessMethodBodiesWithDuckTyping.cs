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
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class ProcessMethodBodiesWithDuckTyping : ProcessMethodBodies
	{
		protected IType _runtimeServices;
		protected IMethod RuntimeServices_Invoke;
		protected IMethod RuntimeServices_InvokeBinaryOperator;
		protected IMethod RuntimeServices_InvokeUnaryOperator;
		protected IMethod RuntimeServices_SetProperty;
		protected IMethod RuntimeServices_GetProperty;
		
		override protected void InitializeMemberCache()
		{
			base.InitializeMemberCache();
			_runtimeServices = TypeSystemServices.Map(typeof(Boo.Lang.RuntimeServices));
			RuntimeServices_Invoke = ResolveMethod(_runtimeServices, "Invoke");
			RuntimeServices_InvokeBinaryOperator = ResolveMethod(_runtimeServices, "InvokeBinaryOperator");
			RuntimeServices_InvokeUnaryOperator = ResolveMethod(_runtimeServices, "InvokeUnaryOperator");
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
			if (IsDuckTyped(node.Target))
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
		
		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			if (IsDuckTyped(node.Operand) &&
			   node.Operator == UnaryOperatorType.UnaryNegation)
			{
				MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
						RuntimeServices_InvokeUnaryOperator,
						CodeBuilder.CreateStringLiteral(
							GetMethodNameForOperator(node.Operator)),
							node.Operand);							
				BindExpressionType(mie, TypeSystemServices.DuckType);
			
				node.ParentNode.Replace(
					node,
					mie);
			}
			else
			{
				base.LeaveUnaryExpression(node);
			}
		}
		
		override protected void BindBinaryExpression(BinaryExpression node)
		{
			if ((IsDuckTyped(node.Left) || IsDuckTyped(node.Right)))
			{
				if (IsOverloadableOperator(node.Operator))
				{
					MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
							RuntimeServices_InvokeBinaryOperator,
							CodeBuilder.CreateStringLiteral(
								GetMethodNameForOperator(node.Operator)),
								node.Left, node.Right);							
					BindExpressionType(mie, TypeSystemServices.DuckType);
				
					node.ParentNode.Replace(
						node, 
						mie);
				}
				else if (BinaryOperatorType.Or == node.Operator ||
				         BinaryOperatorType.And == node.Operator)
				{
					BindExpressionType(node, TypeSystemServices.DuckType);
				}
				else
				{
					base.BindBinaryExpression(node);
				}
			}
			else
			{
				base.BindBinaryExpression(node);
			}
		}
		
		bool IsOverloadableOperator(BinaryOperatorType op)
		{
			switch (op)
			{
				case BinaryOperatorType.Addition:
				case BinaryOperatorType.Subtraction:
				case BinaryOperatorType.Multiply:
				case BinaryOperatorType.Division:
				case BinaryOperatorType.Modulus:
				case BinaryOperatorType.Exponentiation:
				case BinaryOperatorType.LessThan:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.GreaterThanOrEqual:
				case BinaryOperatorType.Match:
				case BinaryOperatorType.NotMatch:
				case BinaryOperatorType.Member:
				case BinaryOperatorType.NotMember:
				case BinaryOperatorType.BitwiseOr:
				case BinaryOperatorType.BitwiseAnd:
				{
					return true;
				}
			}
			return false;
		}
		
		bool IsDuckTyped(Expression expression)
		{
			return TypeSystemServices.DuckType == expression.ExpressionType;
		}
		
		bool IsQuackBuiltin(IEntity entity)
		{
			return BuiltinFunction.Quack == entity;
		}
		
		void ProcessQuackPropertyGet(MemberReferenceExpression node)
		{
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
												RuntimeServices_GetProperty,
												node.Target,
												CodeBuilder.CreateStringLiteral(node.Name));
			BindExpressionType(mie, TypeSystemServices.DuckType);
			node.ParentNode.Replace(node, mie);
		}
		
		void ProcessQuackPropertySet(BinaryExpression node)
		{
			MemberReferenceExpression target = (MemberReferenceExpression)node.Left;
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
												RuntimeServices_SetProperty,
												target.Target,
												CodeBuilder.CreateStringLiteral(target.Name),
												node.Right);
			BindExpressionType(mie, TypeSystemServices.DuckType);
			node.ParentNode.Replace(node, mie);
		}
		
		void ProcessQuackInvocation(MethodInvocationExpression node)
		{
			MemberReferenceExpression target = (MemberReferenceExpression)node.Target;
			node.Target = CodeBuilder.CreateMemberReference(
								CodeBuilder.CreateReference(node.LexicalInfo, _runtimeServices),
								RuntimeServices_Invoke);
			
			Expression args = CodeBuilder.CreateObjectArray(node.Arguments);
			node.Arguments.Clear();
			node.Arguments.Add(target.Target);
			node.Arguments.Add(CodeBuilder.CreateStringLiteral(target.Name));
			node.Arguments.Add(args);
			BindExpressionType(node, TypeSystemServices.DuckType);
		}		
	}
}
