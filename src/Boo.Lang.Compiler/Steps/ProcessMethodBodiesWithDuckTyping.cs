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
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;

	public class ProcessMethodBodiesWithDuckTyping : ProcessMethodBodies
	{
		protected virtual bool DuckyMode
		{
			get { return _context.Parameters.Ducky; }
		}
		
		override protected IEntity CantResolveAmbiguousMethodInvocation(MethodInvocationExpression node, IEntity[] entities)
		{
			if (!DuckyMode || CallableResolutionService.ValidCandidates.Count == 0)
			{				
				return base.CantResolveAmbiguousMethodInvocation(node, entities);
			}
			
			// ok, we have valid method invocation matches, let's 
			// let the runtime decide which method is the best
			// match
			NormalizeMethodInvocationTarget(node);
			BindQuack(node.Target);
			BindDuck(node);
			return null;
		}
		
		void NormalizeMethodInvocationTarget(MethodInvocationExpression node)
		{
			if (node.Target.NodeType != NodeType.ReferenceExpression) return;
			
			node.Target = MemberReferenceFromReference(
							(ReferenceExpression)node.Target,
							((CallableResolutionService.Candidate)CallableResolutionService.ValidCandidates[0]).Method);
		}
		
		override protected void ProcessBuiltinInvocation(BuiltinFunction function, MethodInvocationExpression node)
		{
			if (TypeSystemServices.IsQuackBuiltin(function))
			{
				BindDuck(node);
			}
			else
			{
				base.ProcessBuiltinInvocation(function, node);
			}
		}
		
		override protected void ProcessAssignment(BinaryExpression node)
		{
			if (TypeSystemServices.IsQuackBuiltin(node.Left.Entity))
			{
				BindDuck(node);
			}
			else
			{
				ProcessStaticallyTypedAssignment(node);
			}
		}

		virtual protected void ProcessStaticallyTypedAssignment(BinaryExpression node)
		{
			base.ProcessAssignment(node);
		}

		protected override bool ShouldRebindMember(IEntity entity)
		{
			// always rebind quack builtins (InPlace operators)
			return null == entity || TypeSystemServices.IsQuackBuiltin(entity);
		}

		protected override void NamedArgumentNotFound(IType type, ReferenceExpression name)
		{
			if (!TypeSystemServices.IsDuckType(type))
			{
				base.NamedArgumentNotFound(type, name);
				return;
			}

			BindQuack(name);
		}

		protected override void AddResolvedNamedArgumentToEval(MethodInvocationExpression eval, ExpressionPair pair, ReferenceExpression instance)
		{
			if (!TypeSystemServices.IsQuackBuiltin(pair.First))
			{
				base.AddResolvedNamedArgumentToEval(eval, pair, instance);
				return;
			}
			
			MemberReferenceExpression memberRef = new MemberReferenceExpression(
				pair.First.LexicalInfo,
				instance.CloneNode(),
				((ReferenceExpression)pair.First).Name);
			BindQuack(memberRef);
			
			eval.Arguments.Add(
				CodeBuilder.CreateAssignment(
					pair.First.LexicalInfo,
					memberRef,
					pair.Second));
		}
		
		override protected void MemberNotFound(MemberReferenceExpression node, INamespace ns)
		{
			if (IsDuckTyped(node.Target))
			{	
				BindQuack(node);
			}
			else
			{
				base.MemberNotFound(node, ns);
			}
		}

		protected virtual bool IsDuckTyped(Expression e)
		{
			return TypeSystemServices.IsDuckTyped(e);
		}

		protected void BindQuack(Expression node)
		{
			Bind(node, BuiltinFunction.Quack);
			BindDuck(node);
		}

		protected void BindDuck(Expression node)
		{
			BindExpressionType(node, TypeSystemServices.DuckType);
		}

		override protected bool ProcessMethodInvocationWithInvalidParameters(MethodInvocationExpression node, IMethod targetMethod)
		{
			if (!TypeSystemServices.IsSystemObject(targetMethod.DeclaringType))
				return false;

			MemberReferenceExpression target = node.Target as MemberReferenceExpression;
			if (null == target) return false;
			if (!IsDuckTyped(target.Target)) return false;

			BindQuack(node.Target);
			BindDuck(node);
			return true;
		}

		
		override protected void ProcessInvocationOnUnknownCallableExpression(MethodInvocationExpression node)
		{
			if (IsDuckTyped(node.Target))
			{
				BindDuck(node);
			}
			else
			{
				base.ProcessInvocationOnUnknownCallableExpression(node);
			}
		}
		
		override public void LeaveSlicingExpression(SlicingExpression node)
		{
			if (IsDuckTyped(node.Target) && !HasDefaultMember(node.Target))
			{
				BindDuck(node);
			}
			else
			{
				base.LeaveSlicingExpression(node);
			}
		}
		
		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			if (IsDuckTyped(node.Operand) &&
			   node.Operator == UnaryOperatorType.UnaryNegation)
			{
				BindDuck(node);
			}
			else
			{
				base.LeaveUnaryExpression(node);
			}
		}

		protected override bool ResolveRuntimeOperator(BinaryExpression node, string operatorName, MethodInvocationExpression mie)
		{			
			if (IsDuckTyped(node.Left)
				|| IsDuckTyped(node.Right))
			{
				if (AstUtil.IsOverloadableOperator(node.Operator)
					|| BinaryOperatorType.Or == node.Operator
					|| BinaryOperatorType.And == node.Operator)
				{
					BindDuck(node);
					return true;
				}
			}
			return base.ResolveRuntimeOperator(node, operatorName, mie);
		}
		
		protected override void CheckBuiltinUsage(ReferenceExpression node, IEntity entity)
		{
			if (TypeSystemServices.IsQuackBuiltin(entity)) return;
			base.CheckBuiltinUsage(node, entity);
		}

		bool HasDefaultMember(Expression expression)
		{
			IType type = GetExpressionType(expression);
			return null != type && null != TypeSystemServices.GetDefaultMember(type);
		}
	}
}

