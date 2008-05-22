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
	using System;
	using System.Collections;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	
	public class StricterErrorChecking : AbstractVisitorCompilerStep
	{	
		Hashtable _types = new Hashtable();
		
		int _ensureBlock;
		
		override public void Run()
		{
			Visit(CompileUnit);
		}
		
		override public void Dispose()
		{
			base.Dispose();
			_types.Clear();
		}

		public override void LeaveCompileUnit(CompileUnit node)
		{
			CheckEntryPoint();
		}

		private void CheckEntryPoint()
		{
			Method method = ContextAnnotations.GetEntryPoint(Context);
			if (null == method) return;

			IMethod entity = (IMethod)TypeSystemServices.GetEntity(method);
			if (IsValidEntryPointReturnType(entity.ReturnType) && IsValidEntryPointParameterList(entity.GetParameters())) return;

			Errors.Add(CompilerErrorFactory.InvalidEntryPoint(method));
		}

		private bool IsValidEntryPointParameterList(IParameter[] parameters)
		{
			if (parameters.Length == 0) return true;
			if (parameters.Length != 1) return false;
			return parameters[0].Type == TypeSystemServices.GetArrayType(TypeSystemServices.StringType, 1);
		}

		private bool IsValidEntryPointReturnType(IType type)
		{
			return type == TypeSystemServices.VoidType
				|| type == TypeSystemServices.IntType;
		}

		override public void LeaveClassDefinition(ClassDefinition node)
		{
			LeaveTypeDefinition(node);
		}
		
		override public void LeaveInterfaceDefinition(InterfaceDefinition node)
		{
			LeaveTypeDefinition(node);
		}
		
		override public void LeaveEnumDefinition(EnumDefinition node)
		{
			LeaveTypeDefinition(node);
		}
		
		void LeaveTypeDefinition(TypeDefinition node)
		{
			string qualifiedName = node.QualifiedName;
			if (node.HasGenericParameters)
			{
				qualifiedName += "`" + node.GenericParameters.Count;
			}

			if (_types.Contains(qualifiedName))
			{
				Errors.Add(CompilerErrorFactory.NamespaceAlreadyContainsMember(node, GetNamespace(node), node.Name));
				return;
			}
			_types.Add(qualifiedName, node); 
		}
		
		string GetNamespace(TypeDefinition node)
		{
			NamespaceDeclaration ns = node.EnclosingNamespace;
			return ns == null ? "" : ns.Name;
		}

		override public void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			if (AstUtil.IsTargetOfMethodInvocation(node)) return;
			if (AstUtil.IsTargetOfMemberReference(node)) return;
			Error(CompilerErrorFactory.InvalidSuper(node));
		}
		
		bool InsideEnsure
		{
			get { return _ensureBlock > 0; }
		}
		
		public override void OnTryStatement(TryStatement node)
		{
			Visit(node.ProtectedBlock);
			Visit(node.ExceptionHandlers);

			EnterEnsureBlock();
			Visit(node.FailureBlock);
			Visit(node.EnsureBlock);
			LeaveEnsureBlock();
		}
		
		private void EnterEnsureBlock()
		{
			++_ensureBlock;
		}
		
		private void LeaveEnsureBlock()
		{
			--_ensureBlock;
		}

		public override void LeaveReturnStatement(ReturnStatement node)
		{
			if (InsideEnsure)
			{
				Error(CompilerErrorFactory.CantReturnFromEnsure(node));
			}
			if (null == node.Expression) return;
			CheckExpressionType(node.Expression);
		}

		public override void LeaveYieldStatement(YieldStatement node)
		{
			if (null == node.Expression) return;
			CheckExpressionType(node.Expression);
		}

		public override void LeaveExpressionInterpolationExpression(ExpressionInterpolationExpression node)
		{
			foreach (Expression e in node.Expressions)
			{
				CheckExpressionType(e);
			}
		}

		public override void LeaveUnaryExpression(UnaryExpression node)
		{
			switch (node.Operator)
			{
				case UnaryOperatorType.Explode:
					LeaveExplodeExpression(node);
					break;
			}
		}

        public override void LeaveGeneratorExpression(GeneratorExpression node)
        {
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

		protected virtual void LeaveExplodeExpression(UnaryExpression node)
		{	
			if (!IsLastArgumentOfVarArgInvocation(node))
			{
				Error(CompilerErrorFactory.ExplodeExpressionMustMatchVarArgCall(node));
			}
		}

		private bool IsLastArgumentOfVarArgInvocation(UnaryExpression node)
		{
			MethodInvocationExpression parent = node.ParentNode as MethodInvocationExpression;
			if (null == parent) return false;
			if (parent.Arguments.Count == 0 || node != parent.Arguments[-1]) return false;
			ICallableType type = parent.Target.ExpressionType as ICallableType;
			if (null != type) return type.GetSignature().AcceptVarArgs;
			
			IMethod method = TypeSystemServices.GetOptionalEntity(parent.Target) as IMethod;
			return null != method && method.AcceptVarArgs;
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
					
			int gotoDepth = AstAnnotations.GetTryBlockDepth(node);
			int targetDepth = AstAnnotations.GetTryBlockDepth(target);
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
			CheckValidExtension(node);
		}

		private void CheckValidExtension(Method node)
		{
			IMethod method = GetEntity(node);
			if (!method.IsExtension) return;

			IType extendedType = method.GetParameters()[0].Type;
			IEntity entity = NameResolutionService.Resolve(extendedType, method.Name, EntityType.Method);
			if (null == entity) return;
			IMethod conflicting = FindConflictingMember(method, entity);
			if (null == conflicting || !conflicting.IsPublic) return;

			Error(CompilerErrorFactory.MemberNameConflict(node, extendedType.ToString(), TypeSystemServices.GetSignature(conflicting, false)));
		}

		private IMethod FindConflictingMember(IMethod extension, IEntity entity)
		{
			if (EntityType.Ambiguous == entity.EntityType) return FindConflictingMember(extension, ((Ambiguous)entity).Entities);

			IMethod method = (IMethod)entity;
			if (IsConflictingMember(extension, method)) return method;
			return null;
		}

		private IMethod FindConflictingMember(IMethod extension, IEntity[] methods)
		{
			foreach (IMethod m in methods)
			{
				if (IsConflictingMember(extension, m)) return m;
			}
			return null;
		}

		private bool IsConflictingMember(IMethod extension, IMethod method)
		{
			IParameter[] xp = extension.GetParameters();
			IParameter[] mp = method.GetParameters();
			if (mp.Length != (xp.Length-1)) return false;
			for (int i=0; i<mp.Length; ++i)
			{
				if (xp[i+1].Type != mp[i].Type) return false;
			}
			return true;
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
				// _ is a commonly accepted dummy variable for unused items
				if (local.Name == "_")
					continue;
				
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
				if (!node.IsPrivate)
				{
					Error(CompilerErrorFactory.StaticConstructorMustBePrivate(node));
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
		
		override public void LeaveConditionalExpression(ConditionalExpression node)
		{
			CheckExpressionType(node.TrueValue);
			CheckExpressionType(node.FalseValue);
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

	    static bool IsAddressOfBuiltin(Expression node)
		{
			return BuiltinFunction.AddressOf == node.Entity;
		}

		void CheckExpressionType(Expression node)
		{
			IType type = node.ExpressionType;
			if (type != TypeSystemServices.VoidType) return;
			Error(CompilerErrorFactory.InvalidExpressionType(node, type.ToString()));
		}
	}
}
