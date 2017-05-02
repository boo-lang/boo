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


using System;
using System.Collections.Generic;
using System.Text;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps
{
	class ClosureSignatureInferrer
	{
		private readonly BlockExpression _closure;
	    private readonly Node _parent;
		private readonly Expression _asyncParent;
		private IType[] _inputTypes;

		public ClosureSignatureInferrer(BlockExpression closure)
		{
			_closure = closure;
		    var parent = closure.ParentNode;
			if (parent.NodeType == NodeType.AsyncBlockExpression)
			{
				_asyncParent = (Expression)parent;
				parent = parent.ParentNode;
			}
		    _parent = parent;
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
			get { return My<BooCodeBuilder>.Instance; }
		}

		public MethodInvocationExpression MethodInvocationContext
		{
			get 
			{
                MethodInvocationExpression mie = _parent as MethodInvocationExpression;
				if (mie != null && 
					(mie.Arguments.Contains(Closure) || (_asyncParent != null && mie.Arguments.Contains(_asyncParent))))
					return mie;
				return null;
			}
		}

		private void InitializeInputTypes()
		{
			_inputTypes = Array.ConvertAll<ParameterDeclaration, IType>(
				Closure.Parameters.ToArray(),
				delegate(ParameterDeclaration pd) { return pd.Type == null ? null : pd.Type.Entity as IType; });
		}

		public ICallableType InferCallableType()
		{
			ICallableType contextType = (
				GetTypeFromMethodInvocationContext() ??
				GetTypeFromDeclarationContext() ??
				GetTypeFromBinaryExpressionContext() ??
				GetTypeFromCastContext() /*??
                GetTypeFromAsyncContext() */) as ICallableType;

			return contextType;
		}

/*	    private IType GetTypeFromAsyncContext()
	    {
	        var async = Closure.ParentNode as AsyncBlockExpression;
	        if (async == null)
	            return null;

	        var oldClosure = _closure;
	        _closure = async;
	        try
	        {
	            var unwrappedType = InferCallableType();
                if (unwrappedType == null)
                    return null;
                async.WrappedType = WrapType(unwrappedType);
                return unwrappedType;
	        }
	        finally
	        {
	            _closure = oldClosure;
	        }
	    }

        private static IType WrapType(ICallableType unwrappedType)
	    {
            return My<TypeSystemServices>.Instance.GenericTaskType.GenericInfo.ConstructType(unwrappedType);
	    }
        */
		private IType GetTypeFromBinaryExpressionContext()
		{
            BinaryExpression binary = _parent as BinaryExpression;
			if (binary == null || Closure != binary.Right) return null;
			return binary.Left.ExpressionType;
		}

		private IType GetTypeFromDeclarationContext()
		{
			TypeReference tr = null;
            DeclarationStatement ds = _parent as DeclarationStatement;
			if (ds != null)
			{
				tr = ds.Declaration.Type;
			}

            Field fd = _parent as Field;
			if (fd != null)
			{
				tr = fd.Type;
			}

			if (tr != null) return tr.Entity as IType;
			return null;
		}

		private IType GetTypeFromMethodInvocationContext()
		{
			if (MethodInvocationContext == null) return null;

			IMethod method = MethodInvocationContext.Target.Entity as IMethod;
			if (method == null)
			{
				if (MethodInvocationContext.Target.Entity.EntityType == EntityType.Ambiguous)
				{
					AstAnnotations.MarkAmbiguousSignature(MethodInvocationContext);
					AstAnnotations.MarkAmbiguousSignature(_closure);
				}
				return null;
			}

			int argumentIndex = MethodInvocationContext.Arguments.IndexOf(Closure);
			if (argumentIndex == -1 && _asyncParent != null)
				argumentIndex = MethodInvocationContext.Arguments.IndexOf(_asyncParent);
			IParameter[] parameters = method.GetParameters();
			
			if (argumentIndex < parameters.Length) return parameters[argumentIndex].Type;
			if (method.AcceptVarArgs) return parameters[parameters.Length - 1].Type;
			return null;
		}

		private IType GetTypeFromCastContext()
		{
            TryCastExpression tryCast = _parent as TryCastExpression;
			if (tryCast != null) return tryCast.Type.Entity as IType;

            CastExpression cast = _parent as CastExpression;
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
	}
}
