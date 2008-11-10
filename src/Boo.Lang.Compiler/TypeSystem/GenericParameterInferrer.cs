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
using System.IO;

namespace Boo.Lang.Compiler.TypeSystem
{
	class GenericParameterInferrer : TypeInferrer
	{
		IMethod _genericMethod;
		ExpressionCollection _arguments;

		public GenericParameterInferrer(CompilerContext context, IMethod genericMethod, ExpressionCollection arguments)
		{
			_context = context;
			_genericMethod = genericMethod;
			_arguments = arguments;

			InitializeTypeParameters(GenericMethod.GenericInfo.GenericParameters);
			InitializeDependencies(
				GenericMethod.GenericInfo.GenericParameters,
				GenericMethod.CallableType.GetSignature());
		}

		public IMethod GenericMethod
		{
			get { return _genericMethod; }
		}

		public ExpressionCollection Arguments
		{
			get { return _arguments;  }
		}

		public bool Run()
		{
			InferenceStart();

			if (Arguments.Count != GenericMethod.GetParameters().Length)
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
			CallableSignature definitionSignature = GenericMethod.CallableType.GetSignature();
			for (int i = 0; i < Arguments.Count; i++)
			{
				Infer(
					definitionSignature.Parameters[i].Type, 
					Arguments[i].ExpressionType,
					TypeInference.AllowCovariance);
			}
		}

		/// <summary>
		/// Performs inference on implicitly typed callables whose input types have already been inferred.
		/// </summary>
		/// <remarks>
		/// Corresponds to the second phase in generic parameter inference according to the C# 3.0 spec.
		/// </remarks>
		private void InferCallables()
		{
			// TODO
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
				_context.TraceVerbose("Generic parameter {0} fixed to {1}.", genericParameter.Name, inferredType.ResultingType);
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
				", ", Array.ConvertAll<Expression, string>(
				      	_arguments.ToArray(),
				      	delegate(Expression e) { return e.ToString(); }));

			_context.TraceVerbose("Attempting to infer generic type arguments for {0} from argument list ({1}).", _genericMethod, argumentsString);
		}

		private bool InferenceComplete(bool successfully)
		{
			_context.TraceVerbose(
				"Generic type inference for {0} {1}.", 
				_genericMethod,
				successfully ? "succeeded" : "failed");

			return successfully;
		}
	}
}
