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


using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// </summary>
	public class ExpandDuckTypedExpressions : AbstractTransformerCompilerStep
	{
		protected IMethod RuntimeServices_Invoke;
		protected IMethod RuntimeServices_InvokeCallable;
		protected IMethod RuntimeServices_InvokeBinaryOperator;
		protected IMethod RuntimeServices_InvokeUnaryOperator;
		protected IMethod RuntimeServices_SetProperty;
		protected IMethod RuntimeServices_GetProperty;
		protected IMethod RuntimeServices_SetSlice;
		protected IMethod RuntimeServices_GetSlice;
		protected IType _duckTypingServicesType;
		
		public ExpandDuckTypedExpressions()
		{
		}

		public override void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			InitializeDuckTypingServices();
		}
		
		protected virtual void InitializeDuckTypingServices()
		{
			_duckTypingServicesType = GetDuckTypingServicesType();
			RuntimeServices_Invoke = ResolveInvokeMethod();
			RuntimeServices_InvokeCallable = ResolveMethod(_duckTypingServicesType, "InvokeCallable");
			RuntimeServices_InvokeBinaryOperator = ResolveMethod(_duckTypingServicesType, "InvokeBinaryOperator");
			RuntimeServices_InvokeUnaryOperator = ResolveMethod(_duckTypingServicesType, "InvokeUnaryOperator");
			RuntimeServices_SetProperty = ResolveSetPropertyMethod();
			RuntimeServices_GetProperty = ResolveGetPropertyMethod();
			RuntimeServices_SetSlice = ResolveMethod(_duckTypingServicesType, "SetSlice");
			RuntimeServices_GetSlice = ResolveMethod(_duckTypingServicesType, "GetSlice");
		}

		protected virtual IMethod ResolveInvokeMethod()
		{
			return ResolveMethod(_duckTypingServicesType, "Invoke");
		}

		protected virtual IMethod ResolveGetPropertyMethod()
		{
			return ResolveMethod(_duckTypingServicesType, "GetProperty");
		}

		protected virtual IMethod ResolveSetPropertyMethod()
		{
			return ResolveMethod(_duckTypingServicesType, "SetProperty");
		}

		protected virtual IType GetDuckTypingServicesType()
		{
			return TypeSystemServices.Map(typeof(Boo.Lang.Runtime.RuntimeServices));
		}

		protected virtual IMethod GetGetPropertyMethod()
		{
			return RuntimeServices_GetProperty;
		}

		protected virtual IMethod GetSetPropertyMethod()
		{
			return RuntimeServices_SetProperty;
		}

		protected IMethod ResolveMethod(IType type, string name)
		{
			IMethod method = NameResolutionService.ResolveMethod(type, name);
			if (null == method) throw new System.ArgumentException(string.Format("Method '{0}' not found in type '{1}'", type, name));
			return method;
		}

		public override void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}

		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (!TypeSystemServices.IsDuckTyped(node.Target))
			{
				base.OnMethodInvocationExpression(node);
				return;
			}

			if (TypeSystemServices.IsQuackBuiltin(node.Target))
			{
				ExpandQuackInvocation(node);
				return;
			}

			base.OnMethodInvocationExpression(node);
			
			if(node.GetAncestor(NodeType.Constructor) == null 
				|| (node.Target.NodeType != NodeType.SelfLiteralExpression
			    	&& node.Target.NodeType != NodeType.SuperLiteralExpression)
				|| TypeSystemServices.GetOptionalEntity(node.Target) as IConstructor == null)
				ExpandCallableInvocation(node);
		}

		private void ExpandCallableInvocation(MethodInvocationExpression node)
		{
			MethodInvocationExpression invoke = CodeBuilder.CreateMethodInvocation(
				node.LexicalInfo,
				RuntimeServices_InvokeCallable,
				node.Target,
				CodeBuilder.CreateObjectArray(node.Arguments));

			Replace(invoke);
		}

		override public void LeaveSlicingExpression(SlicingExpression node)
		{
			if (!TypeSystemServices.IsDuckTyped(node.Target)) return;
			if (AstUtil.IsLhsOfAssignment(node)) return;

			// todo
			// a[foo]
			// RuntimeServices.GetSlice(a, "", (foo,))

			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				node.LexicalInfo,
				RuntimeServices_GetSlice,
				GetSlicingTarget(node),
				CodeBuilder.CreateStringLiteral(GetSlicingMemberName(node)),
				GetArrayForIndices(node));
			
			Replace(mie);
		}

		private static string GetSlicingMemberName(SlicingExpression node)
		{
			if (NodeType.MemberReferenceExpression == node.Target.NodeType)
			{
				MemberReferenceExpression mre = ((MemberReferenceExpression)node.Target);
				return mre.Name;
			}
			return "";
		}

		private static Expression GetSlicingTarget(SlicingExpression node)
		{
			Expression target = node.Target;
			if (NodeType.MemberReferenceExpression == target.NodeType)
			{
				MemberReferenceExpression mre = ((MemberReferenceExpression)target);
				return mre.Target;
			}
			return target;
		}

		private ArrayLiteralExpression GetArrayForIndices(SlicingExpression node)
		{
			ArrayLiteralExpression args = new ArrayLiteralExpression();
			foreach (Slice index in node.Indices)
			{
				if (AstUtil.IsComplexSlice(index))
				{
					throw CompilerErrorFactory.NotImplemented(index, "complex slice for duck");
				}
				args.Items.Add(index.Begin);
			}
			BindExpressionType(args, TypeSystemServices.ObjectArrayType);
			return args;
		}

		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			if (TypeSystemServices.IsDuckTyped(node.Operand) &&
				node.Operator == UnaryOperatorType.UnaryNegation)
			{
				MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
					node.LexicalInfo,
					RuntimeServices_InvokeUnaryOperator,
					CodeBuilder.CreateStringLiteral(
					AstUtil.GetMethodNameForOperator(node.Operator)),
					node.Operand);
				
				Replace(mie);
			}
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign == node.Operator)
			{
				ProcessAssignment(node);
				return;
			}

			if (!AstUtil.IsOverloadableOperator(node.Operator)) return;
			if (!TypeSystemServices.IsDuckTyped(node.Left) && !TypeSystemServices.IsDuckTyped(node.Right)) return;

			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				node.LexicalInfo,
				RuntimeServices_InvokeBinaryOperator,
				CodeBuilder.CreateStringLiteral(
				AstUtil.GetMethodNameForOperator(node.Operator)),
				node.Left, node.Right);
			Replace(mie);
		}

		private void ProcessAssignment(BinaryExpression node)
		{
			if (NodeType.SlicingExpression == node.Left.NodeType)
			{
				SlicingExpression slice = (SlicingExpression)node.Left;
				if (TypeSystemServices.IsDuckTyped(slice.Target))
				{
					ProcessDuckSlicingPropertySet(node);
				}
			}
			else if (TypeSystemServices.IsQuackBuiltin(node.Left))
			{
				ProcessQuackPropertySet(node);
			}
		}

		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (!TypeSystemServices.IsQuackBuiltin(node)) return;
			
			if (AstUtil.IsLhsOfAssignment(node)
				|| AstUtil.IsTargetOfSlicing(node)) return;

			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				node.LexicalInfo,
				GetGetPropertyMethod(),
				node.Target,
				CodeBuilder.CreateStringLiteral(node.Name));
			Replace(mie);
		}

		void ProcessDuckSlicingPropertySet(BinaryExpression node)
		{
			SlicingExpression slice = (SlicingExpression)node.Left;

			ArrayLiteralExpression args = GetArrayForIndices(slice);
			args.Items.Add(node.Right);
			
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				node.LexicalInfo,
				RuntimeServices_SetSlice,
				GetSlicingTarget(slice),
				CodeBuilder.CreateStringLiteral(GetSlicingMemberName(slice)),
				args);
			Replace(mie);
		}

		void ProcessQuackPropertySet(BinaryExpression node)
		{
			MemberReferenceExpression target = (MemberReferenceExpression)node.Left;
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				node.LexicalInfo,
				GetSetPropertyMethod(),
				target.Target,
				CodeBuilder.CreateStringLiteral(target.Name),
				node.Right);
			Replace(mie);
		}

		protected virtual void ExpandQuackInvocation(MethodInvocationExpression node)
		{
			ExpandQuackInvocation(node, RuntimeServices_Invoke);
		}
		
		protected virtual void ExpandQuackInvocation(MethodInvocationExpression node, IMethod runtimeInvoke)
		{
			Visit(node.Arguments);
			Visit(node.NamedArguments);

			MemberReferenceExpression target = node.Target as MemberReferenceExpression;
			if (target == null) return;

			ExpandMemberInvocation(node, target, runtimeInvoke);
		}

		private void ExpandMemberInvocation(MethodInvocationExpression node, MemberReferenceExpression target, IMethod runtimeInvoke)
		{
			target.Target = (Expression)VisitNode(target.Target);
			node.Target = CodeBuilder.CreateMemberReference(runtimeInvoke);
			
			Expression args = CodeBuilder.CreateObjectArray(node.Arguments);
			node.Arguments.Clear();
			node.Arguments.Add(target.Target);
			node.Arguments.Add(CodeBuilder.CreateStringLiteral(target.Name));
			node.Arguments.Add(args);
		}

		private void BindDuck(Expression node)
		{
			BindExpressionType(node, TypeSystemServices.DuckType);
		}

		void Replace(Expression node)
		{
			BindDuck(node);
			ReplaceCurrentNode(node);
		}
	}
}
