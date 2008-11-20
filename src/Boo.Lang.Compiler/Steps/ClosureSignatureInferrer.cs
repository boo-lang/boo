using System;
using System.Collections.Generic;
using System.Text;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	class ClosureSignatureInferrer
	{
		private BlockExpression _closure;
		private IType[] _inputTypes;

		public ClosureSignatureInferrer(BlockExpression closure)
		{
			_closure = closure;
			InitializeInputTypes();
		}

		public BlockExpression Closure
		{
			get { return _closure; }
		}

		public IType[] ParameterTypes
		{
			get { return _inputTypes; }
		}

		protected BooCodeBuilder CodeBuilder
		{
			get { return CompilerContext.Current.CodeBuilder; }
		}

		public MethodInvocationExpression MethodInvocationContext
		{
			get 
			{
				MethodInvocationExpression mie = Closure.ParentNode as MethodInvocationExpression;
				if (mie != null && mie.Arguments.ContainsNode(Closure)) return mie;
				return null;
			}
		}

		private void InitializeInputTypes()
		{
			_inputTypes = Array.ConvertAll<ParameterDeclaration, IType>(
				Closure.Parameters.ToArray(),
				delegate(ParameterDeclaration pd) { return pd.Type == null ? null : pd.Type.Entity as IType; });
		}

		public void InferInputTypes()
		{
			ICallableType contextType = (
				GetTypeFromMethodInvocationContext() ??
				GetTypeFromDeclarationContext() ??
				GetTypeFromBinaryExpressionContext() ??
				GetTypeFromCastContext()) as ICallableType;

			if (contextType != null)
			{
				InferInputTypesFromContextType(contextType);
			}
		}

		private IType GetTypeFromBinaryExpressionContext()
		{
			BinaryExpression binary = Closure.ParentNode as BinaryExpression;
			if (binary == null || Closure != binary.Right) return null;
			return binary.Left.ExpressionType;
		}

		private IType GetTypeFromDeclarationContext()
		{
			DeclarationStatement ds = Closure.ParentNode as DeclarationStatement;
			if (ds == null || ds.Declaration.Type == null) return null;
			return ds.Declaration.Type.Entity as IType;
		}

		private IType GetTypeFromMethodInvocationContext()
		{
			if (MethodInvocationContext == null) return null;

			IMethod method = MethodInvocationContext.Target.Entity as IMethod;
			if (method == null) return null;

			int argumentIndex = MethodInvocationContext.Arguments.IndexOf(Closure);
			
			IParameter[] parameters = method.GetParameters();
			
			if (argumentIndex < parameters.Length) return parameters[argumentIndex].Type;
			if (method.AcceptVarArgs) return parameters[parameters.Length - 1].Type;
			return null;
		}

		private IType GetTypeFromCastContext()
		{
			TryCastExpression tryCast = Closure.ParentNode as TryCastExpression;
			if (tryCast != null) return tryCast.Type.Entity as IType;

			CastExpression cast = Closure.ParentNode as CastExpression;
			if (cast != null) return cast.Type.Entity as IType;

			return null;
		}

		private void InferInputTypesFromContextType(ICallableType type)
		{
			CallableSignature sig = type.GetSignature();
			for (int i = 0; i < Math.Min(ParameterTypes.Length, sig.Parameters.Length); i++)
			{
				if (ParameterTypes[i] != null) continue;
				ParameterTypes[i] = sig.Parameters[i].Type;
			}
		}

		public bool HasUntypedInputParameters()
		{
			return Array.IndexOf(ParameterTypes, null) != -1;
		}

		public void AddMissingParameterTypes()
		{
			for (int i = 0; i < ParameterTypes.Length; i++)
			{
				ParameterDeclaration parameter = Closure.Parameters[i];

				if (parameter.Type == null && ParameterTypes[i] != null)
				{
					parameter.Type = CodeBuilder.CreateTypeReference(ParameterTypes[i]);
				}
			}
		}
	}
}
