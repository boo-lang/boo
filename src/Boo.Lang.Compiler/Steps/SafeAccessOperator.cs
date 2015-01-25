#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;


namespace Boo.Lang.Compiler.Steps
{
	// Desugarizes the safe access operator.
	//
	//  foo?
	//  (foo is not null)
	//
	//  foo?.bar?
	//  ((foo.bar if foo is not null else null) is not null)
	//
	//  foo?.bar
	//  (foo.bar if foo != null else null)
	//
	//  foo?.bar?.baz
	//  (foo.bar.baz if (foo.bar if foo is not null else null) is not null else null)
	//
	//  foo?.bar?[2]
	//  (foo.bar[2] if (foo.bar if foo is not null else null) is not null else null)
	//
	//  foo?.bar?()
	//  (foo.bar() if (foo.bar if foo is not null else null) is not null else null)
	public class SafeAccessOperator : AbstractTransformerCompilerStep
	{

		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			if (node.Operator == UnaryOperatorType.SafeAccess)
			{
				// target references should already be resolved, so just evaluate as existential
				var notnull = CodeBuilder.CreateNotNullTest(node.Operand);
				ReplaceCurrentNode(notnull);
			}
		}

		override public void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			var tern = ProcessTargets(node);
			if (tern != null)
			{
				ReplaceCurrentNode(tern);
				return;
			}
			base.OnMemberReferenceExpression(node);
		}

		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			var tern = ProcessTargets(node);
			if (tern != null)
			{
				Visit(node.Arguments);
				ReplaceCurrentNode(tern);
				return;
			}
			base.OnMethodInvocationExpression(node);
		}

		override public void OnSlicingExpression(SlicingExpression node)
		{
			var tern = ProcessTargets(node);
			if (tern != null) 
			{
				Visit(node.Indices);
				ReplaceCurrentNode(tern);
			} 
			base.OnSlicingExpression(node);
		}

		protected bool IsTargetable(Node node)
		{
			return (node.NodeType == NodeType.MemberReferenceExpression ||
			        node.NodeType == NodeType.MethodInvocationExpression ||
			        node.NodeType == NodeType.SlicingExpression);
		}

		protected bool IsSafeAccess(Expression node)
		{
			var ue = node as UnaryExpression;
			return ue != null && ue.Operator == UnaryOperatorType.SafeAccess;
		}

		protected Expression ProcessTargets(Expression node)
		{
			// Look for safe access operators in the targets chain
			UnaryExpression ue = null;
			Expression target = node;
			Expression nextTarget = null;
			while (IsTargetable(target))
			{
				if (target is MemberReferenceExpression)
				{
					nextTarget = ((MemberReferenceExpression)target).Target;
				} 
				else if (target is SlicingExpression)
				{
					nextTarget = ((SlicingExpression)target).Target;
				}
				else
				{
					nextTarget = ((MethodInvocationExpression)target).Target;
				}

				if (IsSafeAccess(nextTarget))
				{
					ue = (UnaryExpression)nextTarget;
					break;
				}

				target = nextTarget;
			}

			// No safe access operator was found
			if (ue == null)
			{
				return null;
			}

			// Target the safe access to a temporary variable
			var tmp = new ReferenceExpression(node.LexicalInfo, Context.GetUniqueName("safe"));
			tmp.IsSynthetic = true;

			// Make sure preceding operators are processed
			MemberReferenceExpression mre = null;
			SlicingExpression se = null;
			MethodInvocationExpression mie = null;
			if (null != (mre = target as MemberReferenceExpression))
			{
				Visit(mre.Target);
				mre.Target = tmp.CloneNode();
			} 
			else if (null != (se = target as SlicingExpression))
			{
				Visit(se.Target);
				se.Target = tmp.CloneNode();
			}
			else if (null != (mie = target as MethodInvocationExpression))
			{
				Visit(mie.Target);
				mie.Target = tmp.CloneNode();
			}

			// Convert the target into a ternary operation 
			var tern = new ConditionalExpression(node.LexicalInfo);
			tern.Condition = new BinaryExpression(
				BinaryOperatorType.ReferenceInequality,
				CodeBuilder.CreateAssignment(tmp, ue.Operand),
				CodeBuilder.CreateNullLiteral()
				);
			tern.TrueValue = node;
			tern.FalseValue = CodeBuilder.CreateNullLiteral();

			return tern;
		}
	}
}

