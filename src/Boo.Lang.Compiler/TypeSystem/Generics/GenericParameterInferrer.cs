#region license
// Copyright (c) 2004, 2005, 2006, 2007 Rodrigo B. de Oliveira (rbo@acm.org)
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
using Boo.Lang.Compiler.Ast;

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	class GenericParameterInferrer : TypeInferrer
	{
		IMethod _genericMethod; 
		TypedArgument[] _typedArguments;
		
		IDictionary<BlockExpression, List<InferredType>> _closureDependencies = new Dictionary<BlockExpression, List<InferredType>>();
		BlockExpression _currentClosure = null;
		
		public delegate void ResolveClosureEventHandler(GenericParameterInferrer inferrer, BlockExpression closure, ICallableType formalType);
		public event ResolveClosureEventHandler ResolveClosure;

		public GenericParameterInferrer(CompilerContext context, IMethod genericMethod, ExpressionCollection arguments)
		{
			_genericMethod = genericMethod;

			Initialize(context);
			InitializeArguments(arguments);
			InitializeTypeParameters(GenericMethod.GenericInfo.GenericParameters);
			InitializeDependencies(
				GenericMethod.GenericInfo.GenericParameters,
				GenericMethod.CallableType.GetSignature());
			InitializeClosureDependencies();
		}

		private void InitializeClosureDependencies()
		{
			foreach (TypedArgument argument in Arguments)
			{
				BlockExpression closure = argument.Expression as BlockExpression;
				if (closure == null) continue;

				// ICallableType callableType = closure.ExpressionType as ICallableType;
				ICallableType callableType = argument.FormalType as ICallableType;
				if (callableType == null) continue;
				
				TypeCollector collector = new TypeCollector(delegate(IType t) 
				{
					IGenericParameter gp = t as IGenericParameter;
					return gp != null && InferredTypes.ContainsKey(gp); 
				});

				foreach (IType inputType in GetParameterTypes(callableType.GetSignature()))
				{
					collector.Visit(inputType);
				}

				foreach (IGenericParameter gp in collector.Matches)
				{
					RecordClosureDependency(closure, gp);
				}
			}
		}

		public IMethod GenericMethod
		{
			get { return _genericMethod; }
		}

		private TypedArgument[] Arguments
		{
			get { return _typedArguments; }
		}

		private void InitializeArguments(ExpressionCollection arguments)
		{
			_typedArguments = new TypedArgument[arguments.Count];
			CallableSignature methodSignature = GenericMethod.CallableType.GetSignature();
			int count = Math.Min(arguments.Count, methodSignature.Parameters.Length);
			IType formalType = null;

			for (int i = 0; i < count; i++)
			{
				formalType = methodSignature.Parameters[i].Type;
				if (GenericMethod.AcceptVarArgs && i == count - 1)
					formalType = formalType.GetElementType();
				_typedArguments[i] = new TypedArgument(arguments[i], formalType);
			}

			for (int i = count; i < arguments.Count; i++)
			{
				_typedArguments[i] = new TypedArgument(arguments[i], GenericMethod.AcceptVarArgs ? formalType : null);
			}
		}

		public bool Run()
		{
			InferenceStart();

			if (GenericMethod.AcceptVarArgs)
			{
				if (Arguments.Length < GenericMethod.GetParameters().Length)
					return InferenceComplete(false);
			}
			else if (Arguments.Length != GenericMethod.GetParameters().Length)
			{
				return InferenceComplete(false);
			}

			InferExplicits();

			while (HasUnfixedTypes())
			{
				bool wasFixed = FixAll(HasNoDependencies) || FixAll(HasDependantsAndBounds);
				if (!wasFixed)
				{
					return InferenceComplete(false);
				}
				InferCallables();
			};

			return InferenceComplete(true);
		}

		/// <summary>
		/// Performs inference on explicitly typed arguments.
		/// </summary>
		/// <remarks>
		/// Corresponds to the first phase in generic parameter inference according to the C# 3.0 spec.
		/// </remarks>
		private void InferExplicits()
		{
			foreach (TypedArgument typedArgument in Arguments)
			{
				_currentClosure = typedArgument.Expression as BlockExpression;

				Infer(
					typedArgument.FormalType,
					typedArgument.Expression.ExpressionType,
					TypeInference.AllowCovariance);
			}
		}

		protected override bool InferGenericParameter(IGenericParameter formalType, IType actualType, TypeInference inference)
		{
			// Generic parameters occuring in closures are a result of untyped input types and should be skipped
			if (_currentClosure != null && actualType == formalType) return true;

			return base.InferGenericParameter(formalType, actualType, inference);
		}

		private void RecordClosureDependency(BlockExpression closure, IGenericParameter genericParameter)
		{
			if (!_closureDependencies.ContainsKey(closure))
			{
				_closureDependencies.Add(closure, new List<InferredType>());
			}

			_closureDependencies[closure].AddUnique(InferredTypes[genericParameter]);
		}

		/// <summary>
		/// Performs inference on implicitly typed callables whose input types have already been inferred.
		/// </summary>
		/// <remarks>
		/// Corresponds to the second phase in generic parameter inference according to the C# 3.0 spec.
		/// </remarks>
		private void InferCallables()
		{
			foreach (TypedArgument argument in Arguments)
			{
				BlockExpression closure = argument.Expression as BlockExpression;
				if (closure == null) continue;

				if (CanResolveClosure(closure))
				{
					ResolveClosure(this, closure, argument.FormalType as ICallableType);
					_closureDependencies.Remove(closure);
					Infer(argument.FormalType, closure.ExpressionType);
				}
			}
		}

		private bool CanResolveClosure(BlockExpression closure)
		{
			if (ResolveClosure == null) return false; 

			// Only resolve closures that depend on generic parameters, where all the dependencies are fixed
			if (!_closureDependencies.ContainsKey(closure)) return false;

			foreach (InferredType dependency in _closureDependencies[closure])
			{
				if (!dependency.Fixed) return false;
			}
			return true;
		}

		private bool FixAll(Predicate<InferredType> predicate)
		{
			bool wasFixed = false;
			foreach (KeyValuePair<IGenericParameter, InferredType> kvp in InferredTypes)
			{
				IGenericParameter gp = kvp.Key;
				InferredType inferredType = kvp.Value;
				
				if (!inferredType.Fixed && predicate(inferredType))
				{
					wasFixed |= Fix(gp, inferredType);
				}
			}
			return wasFixed;
		}

		private bool Fix(IGenericParameter genericParameter, InferredType inferredType)
		{
			if (inferredType.Fix())
			{
				Context.TraceVerbose("Generic parameter {0} fixed to {1}.", genericParameter.Name, inferredType.ResultingType);
				return true;
			}
			return false;
		}

		private bool HasUnfixedTypes()
		{
			foreach (InferredType inferredType in InferredTypes.Values)
			{
				if (!inferredType.Fixed) return true;
			}
			return false;
		}

		private bool HasNoDependencies(InferredType inferredType)
		{
			return !inferredType.HasDependencies;
		}

		private bool HasDependantsAndBounds(InferredType inferredType)
		{
			return inferredType.HasDependants && inferredType.HasBounds;
		}

		public IType[] GetInferredTypes()
		{
			return Array.ConvertAll<IGenericParameter, IType>(
				GenericMethod.GenericInfo.GenericParameters,
				GetInferredType);
		}

		private void InitializeDependencies(IGenericParameter[] genericParameters, CallableSignature signature)
		{
			IType[] parameterTypes = GetParameterTypes(signature);

			foreach (IType parameterType in parameterTypes)
			{
				ICallableType callableParameterType = parameterType as ICallableType;
				if (callableParameterType == null) continue;
				CalculateDependencies(callableParameterType.GetSignature());
			}
		}

		private void CalculateDependencies(CallableSignature signature)
		{
			foreach (IType inputType in GetParameterTypes(signature))
			{
				CalculateDependencies(inputType, signature.ReturnType);
			}
		}

		private void CalculateDependencies(IType inputType, IType outputType)
		{
			foreach (IGenericParameter dependant in FindGenericParameters(outputType))
			{
				foreach (IGenericParameter dependee in FindGenericParameters(inputType))
				{
					SetDependency(dependant, dependee);
				}
			}
		}

		private IEnumerable<IGenericParameter> FindGenericParameters(IType type)
		{
			foreach (IGenericParameter gp in GenericsServices.FindGenericParameters(type))
			{
				if (!InferredTypes.ContainsKey(gp)) continue;
				yield return gp;
			}
		}

		private void SetDependency(IGenericParameter dependant, IGenericParameter dependee)
		{
			InferredTypes[dependant].SetDependencyOn(InferredTypes[dependee]);
		}

		private IType[] GetParameterTypes(CallableSignature signature)
		{
			return Array.ConvertAll<IParameter, IType>(
				signature.Parameters,
				delegate(IParameter p) { return p.Type; });
		}

		private void InferenceStart()
		{
			string argumentsString = string.Join(
				", ", Array.ConvertAll<TypedArgument, string>(
				      	Arguments,
				      	delegate(TypedArgument arg) { return arg.Expression.ToString(); }));

			Context.TraceVerbose("Attempting to infer generic type arguments for {0} from argument list ({1}).", _genericMethod, argumentsString);
		}

		private bool InferenceComplete(bool successfully)
		{
			Context.TraceVerbose(
				"Generic type inference for {0} {1}.", 
				_genericMethod,
				successfully ? "succeeded" : "failed");

			return successfully;
		}

		private struct TypedArgument
		{
			public TypedArgument(Expression expression, IType formalType)
			{
				Expression = expression;
				FormalType = formalType;
			}

			public Expression Expression;
			public IType FormalType;
		}
	}
}