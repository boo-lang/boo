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
	
	public class StricterErrorChecking : AbstractVisitorCompilerStep
	{	
		override public void Run()
		{
			Visit(CompileUnit);
		}

		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			if (AstUtil.IsTargetOfMethodInvocation(node)) return;
			if (AstUtil.IsTargetOfMemberReference(node)) return;
			Error(CompilerErrorFactory.InvalidSuper(node));
		}

		public override void LeaveReturnStatement(ReturnStatement node)
		{
			if (null == node.Expression) return;
			CheckExpressionType(node.Expression);
		}

		public override void LeaveYieldStatement(YieldStatement node)
		{
			if (null == node.Expression) return;
			CheckExpressionType(node.Expression);
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			CheckExpressionType(node.Right);
			if (BinaryOperatorType.ReferenceEquality == node.Operator)
			{
				if (IsTypeReference(node.Right))
				{
					Warnings.Add(
						CompilerWarningFactory.IsInsteadOfIsa(node));
				}
			}
		}
		
		bool IsTypeReference(Expression node)
		{
			return (NodeType.TypeofExpression == node.NodeType) ||
				(
					node is ReferenceExpression &&
					node.Entity is IType);
		}
		
		override public void OnGotoStatement(GotoStatement node)
		{			
			LabelStatement target = ((InternalLabel)node.Label.Entity).LabelStatement; 
					
			int gotoDepth = ContextAnnotations.GetTryBlockDepth(node);
			int targetDepth = ContextAnnotations.GetTryBlockDepth(target);
			if (gotoDepth < targetDepth)
			{
				BranchError(node, target);
			}
			else if (gotoDepth == targetDepth)
			{
				Node gotoParent = AstUtil.GetParentTryExceptEnsure(node);
				Node labelParent = AstUtil.GetParentTryExceptEnsure(target);
				if (gotoParent != labelParent)
				{
					BranchError(node, target);
				}
			}
		}
		
		void BranchError(GotoStatement node, LabelStatement target)
		{
			Node parent = AstUtil.GetParentTryExceptEnsure(target);
			switch (parent.NodeType)
			{
				case NodeType.TryStatement:
				{
					Error(CompilerErrorFactory.CannotBranchIntoTry(node.Label));
					break;
				}
				
				case NodeType.ExceptionHandler:
				{
					Error(CompilerErrorFactory.CannotBranchIntoExcept(node.Label));
					break;
				}
				
				case NodeType.Block:
				{
					Error(CompilerErrorFactory.CannotBranchIntoEnsure(node.Label));
					break;
				}
			}
		}
		
		override public void LeaveMethod(Method node)
		{
			InternalMethod derived = (InternalMethod)node.Entity;
			IMethod super = derived.Overriden;
			if (null != super)
			{
				TypeMemberModifiers derivedAccess = TypeSystemServices.GetAccess(derived);
				TypeMemberModifiers superAccess = TypeSystemServices.GetAccess(super);
				if (derivedAccess < superAccess)
				{
					Error(CompilerErrorFactory.DerivedMethodCannotReduceAccess(
								node,
								derived.FullName,
								super.FullName,
								derivedAccess,
								superAccess));
				}
			}
			
			CheckUnusedLocals(node);
			CheckAbstractMethodCantHaveBody(node);
		}

		private void CheckAbstractMethodCantHaveBody(Method node)
		{
			if (node.IsAbstract)
			{
				if (node.Body.Statements.Count > 0)
				{
					Error(CompilerErrorFactory.AbstractMethodCantHaveBody(node, node.FullName));
				}
			}
		}

		void CheckUnusedLocals(Method node)
		{
			foreach (Local local in node.Locals)
			{
				InternalLocal entity = (InternalLocal)local.Entity;
				if (!entity.IsPrivateScope && !entity.IsUsed)
				{
					Warnings.Add(CompilerWarningFactory.UnusedLocalVariable(local, local.Name));
				}
			}
		}
		
		override public void LeaveConstructor(Constructor node)
		{
			if (node.IsStatic)
			{
				if (!node.IsPublic)
				{
					Error(CompilerErrorFactory.StaticConstructorMustBePublic(node));
				}
				if (node.Parameters.Count != 0)
				{
					Error(CompilerErrorFactory.StaticConstructorCannotDeclareParameters(node));
				}
			}
			CheckUnusedLocals(node);
		}
		
		override public void LeaveMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (IsAddressOfBuiltin(node.Target))
			{
				if (!IsSecondArgumentOfDelegateConstructor(node))
				{
					Error(CompilerErrorFactory.AddressOfOutsideDelegateConstructor(node.Target));
				}
			}
		}
		
		bool IsSecondArgumentOfDelegateConstructor(Expression node)
		{                 
			MethodInvocationExpression mie = node.ParentNode as MethodInvocationExpression;
			if (null != mie)
			{
				if (IsDelegateConstructorInvocation(mie))
				{
					return mie.Arguments[1] == node;
				}
			}
			return false;
		}
		
		bool IsDelegateConstructorInvocation(MethodInvocationExpression node)
		{
			IConstructor constructor = node.Target.Entity as IConstructor;
			if (null != constructor)
			{
				return constructor.DeclaringType is ICallableType;
			}
			return false;
		}
		
		bool IsAddressOfBuiltin(Expression node)
		{
			return BuiltinFunction.AddressOf == node.Entity;
		}

		void CheckExpressionType(Expression node)
		{
			IType type = node.ExpressionType;
			if (type != TypeSystemServices.VoidType) return;
			Error(CompilerErrorFactory.InvalidExpressionType(node, type.FullName));
		}
	}
}
