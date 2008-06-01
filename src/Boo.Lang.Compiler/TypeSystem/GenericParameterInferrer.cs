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
	class GenericParameterInferrer
	{
		CompilerContext _context;
		IMethod _genericMethod;
		ExpressionCollection _arguments;
		Dictionary<IGenericParameter, InferredType> _inferredTypes = new Dictionary<IGenericParameter, InferredType>();

		public GenericParameterInferrer(CompilerContext context, IMethod genericMethod, ExpressionCollection arguments)
		{
			_context = context;
			_genericMethod = genericMethod;
			_arguments = arguments;

			InitializeInferredTypes(GenericMethod.GenericInfo.GenericParameters);
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
					Inference.AllowCovariance);
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

		/// <summary>
		/// Attempts to infer the type of generic parameters that occur in a formal parameter type
		/// according to its actual argument type. 
		/// </summary>
		/// <returns>False if inference failed; otherwise, true. </returns>
		private bool Infer(IType formalType, IType actualType, Inference inference)
		{
			// Skip unspecified actual types
			if (actualType == null) return true;

			if (formalType is IGenericParameter)
			{
				return InferGenericParameter((IGenericParameter)formalType, actualType, inference);
			}

			if (formalType is ICallableType)
			{
				return InferCallableType((ICallableType)formalType, actualType, inference);
			}

			if (formalType.ConstructedInfo != null)
			{
				return InferConstructedType(formalType, actualType, inference);
			}

			if (formalType is IArrayType)
			{
				return InferArrayType((IArrayType)formalType, actualType, inference);
			}

			return InferSimpleType(formalType, actualType, inference);
		}

		private bool InferGenericParameter(IGenericParameter formalType, IType actualType, Inference inference)
		{
			if (_inferredTypes.ContainsKey(formalType))
			{
				InferredType inferredType = _inferredTypes[formalType];
				if ((inference & Inference.AllowContravariance) != Inference.AllowContravariance)
				{
					inferredType.ApplyLowerBound(actualType);
				}
				if ((inference & Inference.AllowCovariance) != Inference.AllowCovariance)
				{
					inferredType.ApplyUpperBound(actualType);
				}
			}

			return true;
		}

		private bool InferCallableType(ICallableType formalType, IType actualType, Inference inference)
		{
			ICallableType callableActualType = actualType as ICallableType;
			if (callableActualType == null) return false;

			CallableSignature formalSignature = formalType.GetSignature();
			CallableSignature actualSignature = callableActualType.GetSignature();

			// TODO: expand actual signature when it involves varargs?
			if (formalSignature.Parameters.Length != actualSignature.Parameters.Length) return false;

			// Infer return type, maintaining inference direction
			if (!Infer(formalSignature.ReturnType, actualSignature.ReturnType, inference))
			{
				return false;
			}

			// Infer parameter types, inverting inference direction
			for (int i = 0; i < formalSignature.Parameters.Length; ++i)
			{
				bool inferenceSuccessful = Infer(
					formalSignature.Parameters[i].Type,
					actualSignature.Parameters[i].Type,
					Invert(inference));

				if (!inferenceSuccessful) return false;
			}
			return true;
		}

		private bool InferConstructedType(IType formalType, IType actualType, Inference inference)
		{
			// look for a single occurance of the formal 
			// constructed type in the actual type's hierarchy 
			IType constructedActualType = GenericsServices.FindConstructedType(
				actualType,
				formalType.ConstructedInfo.GenericDefinition);

			if (constructedActualType == null)
			{
				return false;
			}

			// Exact inference requires the constructed occurance to be
			// the actual type itself
			if (inference == Inference.Exact && actualType != constructedActualType)
			{
				return false;
			}

			for (int i = 0; i < formalType.ConstructedInfo.GenericArguments.Length; ++i)
			{
				bool inferenceSuccessful = Infer(
					formalType.ConstructedInfo.GenericArguments[i],
					constructedActualType.ConstructedInfo.GenericArguments[i],
					Inference.Exact); // Generic arguments must match exactly, no variance allowed

				if (!inferenceSuccessful) return false;
			}
			return true;
		}

		private bool InferArrayType(IArrayType formalType, IType actualType, Inference inference)
		{
			IArrayType actualArrayType = actualType as IArrayType;
			return	
				(actualArrayType != null) && 
				(actualArrayType.GetArrayRank() == formalType.GetArrayRank()) &&
				(Infer(formalType.GetElementType(), actualType.GetElementType(), inference));
		}

		private bool InferSimpleType(IType formalType, IType actualType, Inference inference)
		{
			// Inference has no effect on formal parameter types that are not generic parameters
			return true;
		}

		private bool FixAll(Predicate<InferredType> predicate)
		{
			bool wasFixed = false;
			foreach (KeyValuePair<IGenericParameter, InferredType> kvp in _inferredTypes)
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
			foreach (InferredType inferredType in _inferredTypes.Values)
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

		private IType GetInferredType(IGenericParameter gp)
		{
			if (_inferredTypes.ContainsKey(gp))
			{
				return _inferredTypes[gp].ResultingType;
			}
			else
			{
				return null;
			}
		}

		public IType[] GetInferredTypes()
		{
			return Array.ConvertAll<IGenericParameter, IType>(
				GenericMethod.GenericInfo.GenericParameters,
				GetInferredType);
		}

		private void InitializeInferredTypes(IEnumerable<IGenericParameter> parameters)
		{
			foreach (IGenericParameter gp in parameters)
			{
				_inferredTypes[gp] = new InferredType();
			}
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
			IGenericParameter genericParameter = type as IGenericParameter;
			if (genericParameter != null && _inferredTypes.ContainsKey(genericParameter))
			{
				yield return genericParameter;
			}

			if (type is IArrayType)
			{
				foreach (IGenericParameter gp in FindGenericParameters(type.GetElementType())) yield return gp;
				yield break;
			}

			if (type.ConstructedInfo != null)
			{
				foreach (IType typeArgument in type.ConstructedInfo.GenericArguments)
				{
					foreach (IGenericParameter gp in FindGenericParameters(typeArgument)) yield return gp;
				}
				yield break;
			}

			ICallableType callableType = type as ICallableType;
			if (callableType != null)
			{
				CallableSignature signature = callableType.GetSignature();
				foreach (IGenericParameter gp in FindGenericParameters(signature.ReturnType)) yield return gp;
				foreach (IParameter parameter in signature.Parameters)
				{
					foreach (IGenericParameter gp in FindGenericParameters(parameter.Type)) yield return gp;
				}
				yield break;
			}
		}

		private void SetDependency(IGenericParameter dependant, IGenericParameter dependee)
		{
			_inferredTypes[dependant].SetDependencyOn(_inferredTypes[dependee]);
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

		private Inference Invert(Inference inference)
		{
			switch (inference)
			{
				case Inference.AllowCovariance:
					return Inference.AllowContravariance;

				case Inference.AllowContravariance:
					return Inference.AllowCovariance;

				default:
					return Inference.Exact;
			}
		}

		enum Inference
		{
			/// <summary>
			/// The type parameter must be set to the exact actual type.
			/// </summary>
			Exact = 0,

			/// <summary>
			/// The type parameter can be set to a supertype of the actual type.
			/// </summary>
			AllowCovariance = 1,

			/// <summary>
			/// The type parameter is allowed to be set to a type derived from the actual type.
			/// </summary>
			AllowContravariance = 2
		}
	}
}
