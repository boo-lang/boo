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

using System.Collections.Generic;

namespace Boo.Lang.Compiler.TypeSystem.Generics
{
	class TypeInferrer : AbstractCompilerComponent 
	{
		Dictionary<IGenericParameter, InferredType> _inferredTypes = new Dictionary<IGenericParameter, InferredType>();

		public TypeInferrer()
		{
		}

		public TypeInferrer(IEnumerable<IGenericParameter> typeParameters)
		{
			InitializeTypeParameters(typeParameters);
		}

		public void InitializeTypeParameters(IEnumerable<IGenericParameter> typeParameters)
		{
			foreach (IGenericParameter typeParameter in typeParameters)
			{
				InferredTypes.Add(typeParameter, new InferredType());
			}
		}

		protected IDictionary<IGenericParameter, InferredType> InferredTypes
		{
			get { return _inferredTypes; }
		}

		/// <summary>
		/// Finalizes the inference by attempting to fix all inferred types.
		/// </summary>
		/// <returns>Whether the inference was completed successfully.</returns>
		public bool FinalizeInference()
		{
			foreach (InferredType inferredType in InferredTypes.Values)
			{
				if (!inferredType.Fixed) inferredType.Fix();
			}

			foreach (InferredType inferredType in InferredTypes.Values)
			{
				if (!inferredType.Fixed) return false;
			}	
			return true;
		}

		public IType GetInferredType(IGenericParameter gp)
		{
			if (InferredTypes.ContainsKey(gp))
			{
				return InferredTypes[gp].ResultingType;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Attempts to infer the type of generic parameters that occur in a formal parameter type
		/// according to its actual argument type. 
		/// </summary>
		/// <returns>False if inference failed; otherwise, true. </returns>
		public bool Infer(IType formalType, IType actualType)
		{
			return Infer(formalType, actualType, TypeInference.AllowCovariance);
		}

		/// <summary>
		/// Attempts to infer the type of generic parameters that occur in a formal parameter type
		/// according to its actual argument type. 
		/// </summary>
		/// <returns>False if inference failed; otherwise, true. </returns>
		protected bool Infer(IType formalType, IType actualType, TypeInference inference)
		{
			// Skip unspecified actual types
			if (actualType == null)
				return true;

			IGenericParameter gp = formalType as IGenericParameter;
			if (null != gp)
			{
				return InferGenericParameter(gp, actualType, inference);
			}

			ICallableType callableType = formalType as ICallableType;
			if (null != callableType)
			{
				return InferCallableType(callableType, actualType, inference);
			}

			if (formalType.ConstructedInfo != null)
			{
				return InferConstructedType(formalType, actualType, inference);
			}

			IArrayType arrayType = formalType as IArrayType;
			if (null != arrayType)
			{
				return InferArrayType(arrayType, actualType, inference);
			}

			if (formalType.IsByRef)
			{
				return Infer(formalType.ElementType, actualType, inference);
			}

			return InferSimpleType(formalType, actualType, inference);
		}

		protected virtual bool InferGenericParameter(IGenericParameter formalType, IType actualType, TypeInference inference)
		{
			if (InferredTypes.ContainsKey(formalType))
			{
				InferredType inferredType = InferredTypes[formalType];
				if ((inference & TypeInference.AllowContravariance) != TypeInference.AllowContravariance)
				{
					inferredType.ApplyLowerBound(actualType);
				}
				if ((inference & TypeInference.AllowCovariance) != TypeInference.AllowCovariance)
				{
					inferredType.ApplyUpperBound(actualType);
				}
			}

			return true;
		}

		private bool InferCallableType(ICallableType formalType, IType actualType, TypeInference inference)
		{
			ICallableType callableActualType = actualType as ICallableType;
			if (callableActualType == null) return false;

			CallableSignature formalSignature = formalType.GetSignature();
			CallableSignature actualSignature = callableActualType.GetSignature();

			if (formalSignature.AcceptVarArgs)
			{
				if (actualSignature.Parameters.Length < formalSignature.Parameters.Length)
					return false;
			}
			else if (formalSignature.Parameters.Length != actualSignature.Parameters.Length)
			{
				return false;
			}

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

		private bool InferConstructedType(IType formalType, IType actualType, TypeInference inference)
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
			if (inference == TypeInference.Exact && actualType != constructedActualType)
			{
				return false;
			}

			for (int i = 0; i < formalType.ConstructedInfo.GenericArguments.Length; ++i)
			{
				bool inferenceSuccessful = Infer(
					formalType.ConstructedInfo.GenericArguments[i],
					constructedActualType.ConstructedInfo.GenericArguments[i],
					TypeInference.Exact); // Generic arguments must match exactly, no variance allowed

				if (!inferenceSuccessful) return false;
			}
			return true;
		}

		private bool InferArrayType(IArrayType formalType, IType actualType, TypeInference inference)
		{
			IArrayType actualArrayType = actualType as IArrayType;
			return
				(actualArrayType != null) &&
				(actualArrayType.Rank == formalType.Rank) &&
				(Infer(formalType.ElementType, actualType.ElementType, inference));
		}

		private bool InferSimpleType(IType formalType, IType actualType, TypeInference inference)
		{
			// Inference has no effect on formal parameter types that are not generic parameters
			return true;
		}

		private TypeInference Invert(TypeInference inference)
		{
			switch (inference)
			{
				case TypeInference.AllowCovariance:
					return TypeInference.AllowContravariance;

				case TypeInference.AllowContravariance:
					return TypeInference.AllowCovariance;

				default:
					return TypeInference.Exact;
			}
		}
	}

	enum TypeInference
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
