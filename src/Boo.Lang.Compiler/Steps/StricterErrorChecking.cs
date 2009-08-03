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

using Boo.Lang.Compiler.TypeSystem.Internal;

namespace Boo.Lang.Compiler.Steps
{	
	using System;
	using System.Collections.Generic;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Ast;
	using Boo.Lang.Compiler.TypeSystem;
	using Boo.Lang.Compiler.Util;
	
	public class StricterErrorChecking : AbstractNamespaceSensitiveVisitorCompilerStep
	{	
		Dictionary<string,TypeDefinition> _types = new Dictionary<string,TypeDefinition>();
		
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
			return parameters[0].Type == TypeSystemServices.StringType.MakeArrayType(1);
		}

		private bool IsValidEntryPointReturnType(IType type)
		{
			return type == TypeSystemServices.VoidType
				|| type == TypeSystemServices.IntType;
		}

		override public void LeaveTypeDefinition(TypeDefinition node)
		{
			_safeVars.Clear(); //clear type var cache (CheckAmbiguousVariableNames)

			if (node.NodeType == NodeType.Module)
				return;

			string qualifiedName = node.QualifiedName;
			if (node.HasGenericParameters)
			{
				qualifiedName += "`" + node.GenericParameters.Count;
			}

			if (_types.ContainsKey(qualifiedName))
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

		public override void OnCustomStatement(CustomStatement node)
		{
			Error(CompilerErrorFactory.InvalidNode(node));
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

			//check that the assignment or comparison is meaningful
			if (BinaryOperatorType.Assign == node.Operator
			    || AstUtil.GetBinaryOperatorKind(node) == BinaryOperatorKind.Comparison)
			{
				if (AreSameExpressions(node.Left, node.Right))
				{
					Warnings.Add(
						(BinaryOperatorType.Assign == node.Operator)
						? CompilerWarningFactory.AssignmentToSameVariable(node)
						: CompilerWarningFactory.ComparisonWithSameVariable(node)
					);
				}
				else if (BinaryOperatorType.Assign != node.Operator
				         && AreConstantExpressions(node.Left, node.Right))
				{
					Warnings.Add(
						CompilerWarningFactory.ConstantExpression(node)
					);
				}
			}
		}

		public override void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			if (node.ContainsAnnotation(ConstantFolding.FoldedExpression))
				Warnings.Add(
					CompilerWarningFactory.ConstantExpression(node));
		}

		public override void LeaveIfStatement(IfStatement node)
		{
			CheckNotConstant(node.Condition);
		}

		public override void LeaveUnlessStatement(UnlessStatement node)
		{
			CheckNotConstant(node.Condition);
		}

		void CheckNotConstant(Expression node)
		{
			if (IsConstant(node))
				Warnings.Add(
					CompilerWarningFactory.ConstantExpression(node));
		}

		protected virtual void LeaveExplodeExpression(UnaryExpression node)
		{	
			if (!IsLastArgumentOfVarArgInvocation(node))
			{
				Error(CompilerErrorFactory.ExplodeExpressionMustMatchVarArgCall(node));
			}
		}

		static bool AreSameExpressions(Expression a, Expression b)
		{
			if (a.NodeType != b.NodeType)
				return false;

			switch (a.NodeType)
			{
				case NodeType.ReferenceExpression:
					ReferenceExpression are = (ReferenceExpression) a;
					ReferenceExpression bre = (ReferenceExpression) b;
					if (are.Name != bre.Name)
						return false;
					return true;

				case NodeType.MemberReferenceExpression:
					MemberReferenceExpression amre = (MemberReferenceExpression) a;
					MemberReferenceExpression bmre = (MemberReferenceExpression) b;
					if (amre.Name != bmre.Name)
						return false;
					if (!AreSameExpressions(amre.Target, bmre.Target))
						return false;
					return true;

				case NodeType.SelfLiteralExpression:
				case NodeType.SuperLiteralExpression:
					return true;
			}
			return false;
		}

		static bool AreConstantExpressions(Expression a, Expression b)
		{
			return IsConstant(a) && IsConstant(b);
		}

		static bool IsConstant(Expression e)
		{
			if (e.NodeType == NodeType.UnaryExpression)
				return IsConstant(((UnaryExpression)e).Operand);
			if (e.NodeType == NodeType.BinaryExpression) {
				BinaryExpression be = (BinaryExpression)e;
				if (AstUtil.GetBinaryOperatorKind(be) == BinaryOperatorKind.Logical)
					return IsConstant(be.Left) && IsConstant(be.Right);
			}

			if ((e as LiteralExpression) != null
			    && !e.ContainsAnnotation(ConstantFolding.FoldedExpression))
				return true;

			if (IsImplicitCallable(e))
				return true;

			if (IsConstantInternalField(e.Entity as IField, e))
				return true;

			return false;
		}

		static bool IsConstantInternalField(IField f, Expression e)
		{
			if (null == f)
				return false;

			InternalField field = e.Entity as InternalField;
			if (field != null && field.Field.IsStatic && field.IsLiteral)
				return true; //static 'literal' final

			return false;
		}

		static bool IsImplicitCallable(Expression e)
		{
			return e is ReferenceExpression && (null != (e.Entity as IMethod));
		}

		static bool IsLastArgumentOfVarArgInvocation(UnaryExpression node)
		{
			MethodInvocationExpression parent = node.ParentNode as MethodInvocationExpression;
			if (null == parent) return false;
			if (parent.Arguments.Last != node) return false;
			ICallableType type = parent.Target.ExpressionType as ICallableType;
			if (null != type) return type.GetSignature().AcceptVarArgs;
			
			IMethod method = TypeSystemServices.GetOptionalEntity(parent.Target) as IMethod;
			return null != method && method.AcceptVarArgs;
		}

		static bool IsTypeReference(Expression node)
		{
			if (NodeType.TypeofExpression == node.NodeType) return true;
			return node.Entity is IType
				&& (node is ReferenceExpression || node is GenericReferenceExpression);
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
			CheckNotFinalizer(node);
			CheckImplicitReturn(node);
			CheckAmbiguousVariableNames(node);
		}

		private void CheckNotFinalizer(Method node)
		{
			if (node.Name == "Finalize"
				&& !node.IsSynthetic
				&& node.IsOverride
				&& 0 == node.Parameters.Count
				&& 0 == node.GenericParameters.Count)
			{
				Warnings.Add(
					CompilerWarningFactory.OverridingFinalizeIsBadPractice(node));
			}
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
				if (!node.Body.IsEmpty)
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

		Set<string> _safeVars = new Set<string>();

		void CheckAmbiguousVariableNames(Method node)
		{
			if (null == node.DeclaringType || null == node.DeclaringType.Entity)
				return;
			InternalClass klass = node.DeclaringType.Entity as InternalClass;
			if (null == klass || null == klass.BaseType)
				return;

			if (Parameters.DisabledWarnings.Contains("BCW0025"))
				return;

			klass = klass.BaseType as InternalClass;
			foreach (Local local in node.Locals)
			{
				if (null == local.Entity || ((InternalLocal) local.Entity).IsExplicit)
					continue;

				//check in the cache if variable is safe (the frequent case)
				if (_safeVars.Contains(local.Name))
					return;

				//navigate down the base types
				bool safe = true;
				while (null != klass)
				{
					Field field = klass.TypeDefinition.Members[local.Name] as Field;
					if (null != field && field.IsPrivate) {
						safe = false;
						Warnings.Add(CompilerWarningFactory.AmbiguousVariableName(local, local.Name, klass.Name));
						break; //no need to go further down
					}
					klass = klass.BaseType as InternalClass;
				}

				if (safe) //this var is safe for all methods of the current type
					_safeVars.Add(local.Name);
			}
		}

		void CheckImplicitReturn(Method node)
		{
			if (Parameters.DisabledWarnings.Contains("BCW0023"))
				return;

			if (null == node.ReturnType
			    || null == node.ReturnType.Entity
			    || node.ReturnType.Entity == TypeSystemServices.VoidType
			    || node.Body.IsEmpty
			    || ((InternalMethod)node.Entity).IsGenerator
			    || node.Name == "ExpandImpl") //ignore old-style macros
				return;

			if (!AstUtil.AllCodePathsReturnOrRaise(node.Body))
				Warnings.Add(CompilerWarningFactory.ImplicitReturn(node));
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

		override public void LeaveExceptionHandler(ExceptionHandler node)
		{
			if (null != node.Declaration.Type.Entity
			    && ((IType)node.Declaration.Type.Entity).FullName == "System.Exception"
			    && !string.IsNullOrEmpty(node.Declaration.Name))
			{
				if (null != NameResolutionService.ResolveTypeName(new SimpleTypeReference(node.Declaration.Name)))
					Warnings.Add(CompilerWarningFactory.AmbiguousExceptionName(node));
			}
		}

		public override void OnRELiteralExpression(RELiteralExpression node)
		{
			int options = (int) AstUtil.GetRegexOptions(node);
		}

		static bool IsSecondArgumentOfDelegateConstructor(Expression node)
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
		
		static bool IsDelegateConstructorInvocation(MethodInvocationExpression node)
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
